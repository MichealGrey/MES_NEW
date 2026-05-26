using System.Windows;
using System.Windows.Controls;

namespace MES.Modules.Production.Views
{
    public partial class HoldReasonDialog : Window
    {
        public string HoldReason { get; private set; } = string.Empty;
        public string? Remark { get; private set; }

        public HoldReasonDialog()
        {
            InitializeComponent();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var reason = (cmbReason.SelectedItem as ComboBoxItem)?.Content?.ToString()
                         ?? cmbReason.Text.Trim();

            if (string.IsNullOrWhiteSpace(reason))
            {
                MessageBox.Show("请输入或选择 Hold 原因", "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbReason.Focus();
                return;
            }

            HoldReason = reason;
            Remark = string.IsNullOrWhiteSpace(txtRemark.Text) ? null : txtRemark.Text.Trim();
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
