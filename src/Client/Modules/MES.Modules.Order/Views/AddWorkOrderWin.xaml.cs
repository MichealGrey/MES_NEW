using System.Windows;
using System.Windows.Controls;
using MES.Domain.Production;
using MES.Modules.Order.Models;

namespace MES.Modules.Order.Views
{
    public partial class AddWorkOrderWin : Window
    {
        public string OrderNo { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string DieName { get; set; } = string.Empty;
        public PackageType PackageType { get; set; } = PackageType.BGA;
        public string RouteId { get; set; } = string.Empty;
        public int PlannedQty { get; set; } = 25;
        public int UnitQty { get; set; } = 25;
        public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Normal;
        public string CustomerName { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Line { get; set; } = string.Empty;
        public string SpecId { get; set; } = string.Empty;
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public double YieldTarget { get; set; } = 95.0;
        public string? Remark { get; set; }

        public AddWorkOrderWin()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtOrderNo.Text))
            {
                MessageBox.Show("请输入工单号", "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtOrderNo.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtProduct.Text))
            {
                MessageBox.Show("请输入产品名称", "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtProduct.Focus();
                return;
            }
            if (!int.TryParse(txtPlannedQty.Text, out int plannedQty) || plannedQty <= 0)
            {
                MessageBox.Show("计划量必须为正整数", "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPlannedQty.Focus();
                return;
            }

            OrderNo = txtOrderNo.Text.Trim();
            ProductId = txtProductId.Text.Trim();
            ProductName = txtProduct.Text.Trim();
            DieName = txtDieName.Text.Trim();
            PackageType = (cmbPackageType.SelectedItem as ComboBoxItem)?.Content?.ToString() switch
            {
                "BGA" => PackageType.BGA,
                "QFP" => PackageType.QFP,
                "QFN" => PackageType.QFN,
                "SOP" => PackageType.SOP,
                "CSP" => PackageType.CSP,
                "SiP" => PackageType.SiP,
                _ => PackageType.BGA
            };
            RouteId = (cmbRoute.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Split(':')[0]?.Trim() ?? string.Empty;
            PlannedQty = plannedQty;
            UnitQty = int.TryParse(txtUnitQty.Text, out int uq) ? uq : 25;
            Priority = (cmbPriority.SelectedItem as ComboBoxItem)?.Content?.ToString() switch
            {
                "Urgent" => WorkOrderPriority.Urgent,
                "High" => WorkOrderPriority.High,
                "Low" => WorkOrderPriority.Low,
                _ => WorkOrderPriority.Normal
            };
            CustomerName = txtCustomer.Text.Trim();
            Area = (cmbArea.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
            SpecId = txtSpecId.Text.Trim();
            PlannedStartDate = dpStartDate.SelectedDate;
            PlannedEndDate = dpEndDate.SelectedDate;
            YieldTarget = double.TryParse(txtYieldTarget.Text, out double yt) ? yt : 95.0;
            Remark = string.IsNullOrWhiteSpace(txtRemark.Text) ? null : txtRemark.Text.Trim();

            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
