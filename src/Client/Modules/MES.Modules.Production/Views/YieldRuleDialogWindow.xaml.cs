using System.Windows;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Views;

public partial class YieldRuleDialogWindow : Window
{
    public YieldRule YieldRule { get; }

    public YieldRuleDialogWindow(YieldRule rule)
    {
        YieldRule = rule;
        InitializeComponent();
        txtRouteId.Text = rule.RouteId;
        txtStepCode.Text = rule.StepCode;
        txtWarningYield.Text = rule.WarningYield.ToString("F1");
        txtAlarmYield.Text = rule.AlarmYield.ToString("F1");
        txtHoldYield.Text = rule.HoldYield.ToString("F1");
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtRouteId.Text))
        {
            MessageBox.Show("路线ID不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        YieldRule.RouteId = txtRouteId.Text.Trim();
        YieldRule.StepCode = txtStepCode.Text.Trim();

        if (double.TryParse(txtWarningYield.Text, out var w)) YieldRule.WarningYield = w;
        if (double.TryParse(txtAlarmYield.Text, out var a)) YieldRule.AlarmYield = a;
        if (double.TryParse(txtHoldYield.Text, out var h)) YieldRule.HoldYield = h;

        DialogResult = true;
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
