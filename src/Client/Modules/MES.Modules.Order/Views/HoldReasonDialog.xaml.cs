using System.Windows;

namespace MES.Modules.Order.Views;

public partial class HoldReasonDialog : Window
{
    public string HoldReason { get; set; } = string.Empty;
    public string Remark { get; set; } = string.Empty;

    public HoldReasonDialog()
    {
        InitializeComponent();
    }

    private void btnOk_Click(object sender, RoutedEventArgs e)
    {
        HoldReason = txtReason.Text.Trim();
        Remark = txtRemark.Text.Trim();
        DialogResult = true;
        Close();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
