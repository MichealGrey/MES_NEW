using System.Windows;
using System.Windows.Controls;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Views;

public partial class AlarmRuleDialogWindow : Window
{
    public AlarmRule AlarmRule { get; }

    public AlarmRuleDialogWindow(AlarmRule rule)
    {
        AlarmRule = rule;
        InitializeComponent();
        txtRuleId.Text = rule.RuleId;
        txtRuleName.Text = rule.RuleName;
        txtAlarmType.Text = rule.AlarmType;
        txtCondition.Text = rule.Condition;
        txtNotifyRoles.Text = rule.NotifyRoles;
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtRuleName.Text))
        {
            MessageBox.Show("规则名称不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        AlarmRule.RuleId = txtRuleId.Text.Trim();
        AlarmRule.RuleName = txtRuleName.Text.Trim();
        AlarmRule.AlarmType = txtAlarmType.Text.Trim();
        AlarmRule.Condition = txtCondition.Text.Trim();
        AlarmRule.NotifyRoles = txtNotifyRoles.Text.Trim();

        DialogResult = true;
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}

public partial class ScrapRuleDialogWindow : Window
{
    public ScrapRule ScrapRule { get; }

    public ScrapRuleDialogWindow(ScrapRule rule)
    {
        ScrapRule = rule;
        InitializeComponent();
        txtRuleId.Text = rule.RuleId;
        txtRouteId.Text = rule.RouteId;
        txtStepCode.Text = rule.StepCode;
        txtMaxScrapQty.Text = rule.MaxScrapQty.ToString();
        txtMaxScrapPercent.Text = rule.MaxScrapPercent.ToString("F1");

        foreach (ComboBoxItem item in cmbApprovalLevel.Items)
            if (item.Content?.ToString() == rule.ApprovalLevel) { cmbApprovalLevel.SelectedItem = item; break; }
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtRouteId.Text))
        {
            MessageBox.Show("路线ID不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        ScrapRule.RuleId = txtRuleId.Text.Trim();
        ScrapRule.RouteId = txtRouteId.Text.Trim();
        ScrapRule.StepCode = txtStepCode.Text.Trim();
        ScrapRule.ApprovalLevel = (cmbApprovalLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;

        if (int.TryParse(txtMaxScrapQty.Text, out var qty)) ScrapRule.MaxScrapQty = qty;
        if (double.TryParse(txtMaxScrapPercent.Text, out var pct)) ScrapRule.MaxScrapPercent = pct;

        DialogResult = true;
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
