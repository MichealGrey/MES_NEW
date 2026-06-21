using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MES.Shared.Services;
using MES.Shell.Services;
using ISessionService = MES.Shell.Services.ISessionService;

namespace MES.Shell.Views;

public partial class LockScreenView : Window
{
    private readonly IUserAuthenticationService _authService;
    private readonly ISessionService _sessionService;

    public bool Unlocked { get; private set; }
    public bool AccountSwitched { get; private set; }

    public LockScreenView(IUserAuthenticationService authService, ISessionService sessionService)
    {
        _authService = authService;
        _sessionService = sessionService;
        InitializeComponent();
    }

    private async void btnUnlock_Click(object sender, RoutedEventArgs e)
    {
        var password = txtPassword.Text;
        if (string.IsNullOrEmpty(password))
        {
            ShowError("请输入密码");
            return;
        }

        var result = _sessionService.UnlockScreen(password, _authService);
        if (result)
        {
            Unlocked = true;
            this.DialogResult = true;
            this.Close();
        }
        else
        {
            ShowError("密码错误");
        }
    }

    private void btnSwitch_Click(object sender, RoutedEventArgs e)
    {
        AccountSwitched = true;
        this.DialogResult = false;
        this.Close();
    }

    private void txtPassword_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            btnUnlock_Click(this, e);
    }

    private void ShowError(string message)
    {
        txtError.Text = message;
        txtError.Visibility = Visibility.Visible;
    }
}
