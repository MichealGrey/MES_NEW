using System.Windows;
using System.Windows.Controls;
using MES.Shared.Services;
using MES.Shell.Services;
using ISessionService = MES.Shell.Services.ISessionService;

namespace MES.Shell.Views;

public partial class AccountSwitchDialog : Window
{
    private readonly IUserAuthenticationService _authService;
    private readonly ISessionService _sessionService;

    public bool SwitchSuccessful { get; private set; }

    public AccountSwitchDialog(IUserAuthenticationService authService, ISessionService sessionService)
    {
        _authService = authService;
        _sessionService = sessionService;
        InitializeComponent();
    }

    private async void btnSwitch_Click(object sender, RoutedEventArgs e)
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

        var result = await _sessionService.SwitchUserAsync(employeeId, password, _authService);
        if (result)
        {
            SwitchSuccessful = true;
            this.DialogResult = true;
            this.Close();
        }
        else
        {
            ShowError("工号或密码错误，或该账号已禁用");
        }
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        SwitchSuccessful = false;
        this.DialogResult = false;
        this.Close();
    }

    private void btnLock_Click(object sender, RoutedEventArgs e)
    {
        _sessionService.LockScreen();
        SwitchSuccessful = true;
        this.DialogResult = true;
        this.Close();
    }

    private void ShowError(string message)
    {
        txtError.Text = message;
        txtError.Visibility = Visibility.Visible;
    }
}
