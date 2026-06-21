using Prism.Commands;
using Prism.Mvvm;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Repositories;
using MES.Shared.Services;
using MES.Shell.Services;
using System.Windows.Input;

using ISessionService = MES.Shell.Services.ISessionService;

namespace MES.Shell.ViewModels;

public class LoginViewModel : BindableBase
{
    private readonly IUserAuthenticationService _authService;
    private readonly ISessionService _sessionService;
    private readonly IRepository<MES.Infrastructure.Persistence.Entities.SysRole> _roleRepo;
    private readonly IRepository<MES.Infrastructure.Persistence.Entities.SysDepartment> _deptRepo;

    private string _employeeId = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private bool _hasError;
    private bool _rememberMe;

    /// <summary>
    /// 密码从 View 层通过 code-behind 同步写入（WPF PasswordBox 不支持绑定）
    /// </summary>
    public string Password { get; set; } = string.Empty;

    public string EmployeeId
    {
        get => _employeeId;
        set
        {
            SetProperty(ref _employeeId, value);
            HasError = false;
        }
    }

    /// <summary>是否勾选"记住密码"（仅记住工号，出于安全考虑不保存密码）</summary>
    public bool RememberMe
    {
        get => _rememberMe;
        set => SetProperty(ref _rememberMe, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            SetProperty(ref _isLoading, value);
            ((DelegateCommand)LoginCommand).RaiseCanExecuteChanged();
        }
    }

    public bool HasError
    {
        get => _hasError;
        set => SetProperty(ref _hasError, value);
    }

    private bool _loginSuccessful;

    /// <summary>登录是否成功（供 App.xaml.cs 判断）</summary>
    public bool LoginSuccessful
    {
        get => _loginSuccessful;
        set => SetProperty(ref _loginSuccessful, value);
    }

    public ICommand LoginCommand { get; }
    public ICommand CancelCommand { get; }

    public LoginViewModel(
        IUserAuthenticationService authService,
        ISessionService sessionService,
        IRepository<MES.Infrastructure.Persistence.Entities.SysRole> roleRepo,
        IRepository<MES.Infrastructure.Persistence.Entities.SysDepartment> deptRepo)
    {
        _authService = authService;
        _sessionService = sessionService;
        _roleRepo = roleRepo;
        _deptRepo = deptRepo;

        LoginCommand = new DelegateCommand(async () => await LoginAsync(), () => !IsLoading);
        CancelCommand = new DelegateCommand(OnCancel);
    }

    /// <summary>
    /// 设置错误消息（从 code-behind 调用）
    /// </summary>
    public void SetError(string message)
    {
        ErrorMessage = message;
        HasError = true;
    }

    /// <summary>
    /// 窗口加载后调用 — 恢复上次保存的工号
    /// </summary>
    public void LoadSavedCredentials()
    {
        var savedId = LoginSettings.LoadEmployeeId();
        if (!string.IsNullOrEmpty(savedId))
        {
            EmployeeId = savedId;
            RememberMe = true;
        }
    }

    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(EmployeeId))
        {
            ErrorMessage = "请输入工号";
            HasError = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "请输入密码";
            HasError = true;
            return;
        }

        IsLoading = true;
        HasError = false;

        try
        {
            var user = await _authService.AuthenticateAsync(EmployeeId.Trim(), Password);

            if (user == null)
            {
                ErrorMessage = "工号或密码错误";
                HasError = true;
                return;
            }

            // 登录成功 — 如果勾选了"记住密码"，保存工号
            if (RememberMe)
            {
                LoginSettings.Save(EmployeeId.Trim());
            }
            else
            {
                LoginSettings.Clear();
            }

            // 查询角色名称
            var role = await _roleRepo.GetByIdAsync(user.RoleId);
            var roleName = role?.RoleName ?? "未知角色";

            // 查询部门名称
            var dept = await _deptRepo.GetByIdAsync(user.DepartmentId);
            var deptName = dept?.DeptName ?? "未知部门";

            // 启动会话（加载权限）
            await _sessionService.StartSession(
                user.EmployeeId,
                user.DisplayName,
                user.RoleId,
                roleName,
                user.DepartmentId,
                deptName);
            LoginSuccessful = true;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"系统错误: {ex.Message}";
            HasError = true;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void OnCancel()
    {
        LoginSuccessful = false;
    }
}
