using System.Windows;
using System.Windows.Controls;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Views;

public partial class DefectCodeDialogWindow : Window
{
    public DefectCodeInfo DefectCode { get; }

    public DefectCodeDialogWindow(DefectCodeInfo defectCode)
    {
        DefectCode = defectCode;
        InitializeComponent();

        txtDefectCodeId.Text = defectCode.DefectCodeId;
        txtDefectText.Text = defectCode.DefectText;
        chkIsEnabled.IsChecked = defectCode.IsEnabled;

        SelectComboBoxItem(cmbDefectCategory, defectCode.DefectCategory);
        SelectComboBoxItem(cmbSeverity, defectCode.Severity);
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtDefectText.Text))
        {
            MessageBox.Show("缺陷描述不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        DefectCode.DefectCategory = (cmbDefectCategory.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
        DefectCode.Severity = (cmbSeverity.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Major";
        DefectCode.DefectText = txtDefectText.Text.Trim();
        DefectCode.IsEnabled = chkIsEnabled.IsChecked == true;

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
