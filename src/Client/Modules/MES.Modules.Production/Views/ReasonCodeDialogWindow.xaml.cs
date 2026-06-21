using System.Windows;
using System.Windows.Controls;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Views;

public partial class ReasonCodeDialogWindow : Window
{
    public ReasonCodeInfo ReasonCode { get; }

    public ReasonCodeDialogWindow(ReasonCodeInfo reasonCode)
    {
        ReasonCode = reasonCode;
        InitializeComponent();

        txtReasonCodeId.Text = reasonCode.ReasonCodeId;
        txtSubCategory.Text = reasonCode.SubCategory ?? string.Empty;
        txtReasonText.Text = reasonCode.ReasonText;
        chkIsEnabled.IsChecked = reasonCode.IsEnabled;

        SelectComboBoxItem(cmbCategory, reasonCode.Category);
        SelectComboBoxItem(cmbApplicableTo, reasonCode.ApplicableTo);
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtReasonText.Text))
        {
            MessageBox.Show("原因描述不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        ReasonCode.Category = (cmbCategory.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
        ReasonCode.SubCategory = txtSubCategory.Text.Trim();
        ReasonCode.ApplicableTo = (cmbApplicableTo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
        ReasonCode.ReasonText = txtReasonText.Text.Trim();
        ReasonCode.IsEnabled = chkIsEnabled.IsChecked == true;

        DialogResult = true;
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;

    private static void SelectComboBoxItem(ComboBox comboBox, string value)
    {
        foreach (ComboBoxItem item in comboBox.Items)
        {
            if (item.Content?.ToString() == value)
            {
                comboBox.SelectedItem = item;
                return;
            }
        }
    }
}
