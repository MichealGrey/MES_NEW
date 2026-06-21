using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MES.Shared.Services;
using MES.Shell.Services;
using ISessionService = MES.Shell.Services.ISessionService;

namespace MES.Shell.Views;

public partial class PermissionConfirmDialog : Window
{
    private readonly IUserAuthenticationService _authService;
    private readonly ISessionService _sessionService;
    private readonly IPermissionConfirmService _confirmService;

    public string OperationType { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public bool Confirmed { get; private set; }

    public PermissionConfirmDialog(
        IUserAuthenticationService authService,
        ISessionService sessionService,
        IPermissionConfirmService confirmService)
    {
        _authService = authService;
        _sessionService = sessionService;
        _confirmService = confirmService;
        InitializeComponent();
    }

    protected override void OnInitialized(System.EventArgs e)
    {
        base.OnInitialized(e);
        txtOperation.Text = $"当前操作: {OperationType}";
        txtLot.Text = $"批次: {LotId}";
        var requiredLevel = _confirmService.GetRequiredLevel(OperationType);
        var levelDesc = requiredLevel switch
        {
            "L1" => "操作员确认 (L1)",
            "L2" => "QA确认 (L2)",
            "L3" => "工程确认 (L3)",
            "L4" => "主管确认 (L4)",
            _ => requiredLevel
        };
        txtRequiredLevel.Text = $"所需权限: {levelDesc}";
    }

    private async void btnConfirm_Click(object sender, RoutedEventArgs e)
    {
        var employeeId = txtEmployeeId.Text?.Trim();
        var password = txtPassword.Text;

        if (string.IsNullOrEmpty(employeeId))
        {
            ShowError("请输入工号");
            return;
        }
        if (string.IsNullOrEmpty(password))
        {
            ShowError("请输入密码");
            return;
        }

        var requiredLevel = _confirmService.GetRequiredLevel(OperationType);
        var result = await _confirmService.ConfirmPermissionAsync(OperationType, employeeId, password, requiredLevel);

        if (result)
        {
            Confirmed = true;
            await _confirmService.RecordConfirmAsync(OperationType, employeeId, requiredLevel, true);
            this.DialogResult = true;
            this.Close();
        }
        else
        {
            ShowError("权限验证失败：工号/密码错误或权限级别不足");
            await _confirmService.RecordConfirmAsync(OperationType, employeeId, requiredLevel, false);
        }
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        Confirmed = false;
        this.DialogResult = false;
        this.Close();
    }

    private void txtPassword_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            btnConfirm_Click(this, e);
    }

    private void ShowError(string message)
    {
        txtError.Text = message;
        txtError.Visibility = Visibility.Visible;
    }
}
