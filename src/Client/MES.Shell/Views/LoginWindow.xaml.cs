using MES.Shell.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MES.Shell.Views;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;

        // 窗口加载后自动聚焦工号输入框
        Loaded += (s, e) =>
        {
            EmployeeIdBox.Focus();
            Keyboard.Focus(EmployeeIdBox);
        };

        // 全局 Enter 键处理（无论焦点在哪个控件）
        KeyDown += LoginWindow_KeyDown;

        // 登录成功后自动关闭模态窗口
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(LoginViewModel.LoginSuccessful) && _viewModel.LoginSuccessful)
            {
                DialogResult = true;
            }
        };
    }

    /// <summary>
    /// 全局 Enter 键 → 触发登录
    /// </summary>
    private void LoginWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && _viewModel.LoginCommand.CanExecute(null))
        {
            _viewModel.LoginCommand.Execute(null);
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            Close();
        }
    }

    /// <summary>
    /// 密码框内容变更时同步到 ViewModel（WPF PasswordBox 不支持绑定）
    /// </summary>
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        _viewModel.Password = PasswordBox.Password;
    }

    /// <summary>
    /// 获取登录结果（供 App.xaml.cs 判断是否继续启动）
    /// </summary>
    public bool IsLoginSuccessful => _viewModel.LoginSuccessful;

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}