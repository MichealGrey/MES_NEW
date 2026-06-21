using System.Windows;
using System.Windows.Controls;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Views;

public partial class MaterialDialogWindow : Window
{
    public MaterialInfo Material { get; }

    public MaterialDialogWindow(MaterialInfo material)
    {
        Material = material;
        InitializeComponent();

        txtMaterialId.Text = material.MaterialId;
        txtMaterialName.Text = material.MaterialName;
        txtMaterialCode.Text = material.MaterialCode;
        txtSupplier.Text = material.Supplier;
        txtUnit.Text = material.Unit;
        txtMinStock.Text = material.MinStock.ToString();
        txtDescription.Text = material.Description;

        foreach (ComboBoxItem item in cmbMaterialType.Items)
            if (item.Content?.ToString() == material.MaterialType) { cmbMaterialType.SelectedItem = item; break; }
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtMaterialName.Text))
        {
            MessageBox.Show("物料名称不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Material.MaterialId = txtMaterialId.Text.Trim();
        Material.MaterialName = txtMaterialName.Text.Trim();
        Material.MaterialCode = txtMaterialCode.Text.Trim();
        Material.MaterialType = (cmbMaterialType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
        Material.Supplier = txtSupplier.Text.Trim();
        Material.Unit = txtUnit.Text.Trim();
        Material.Description = txtDescription.Text.Trim();

        if (double.TryParse(txtMinStock.Text, out var minStock))
            Material.MinStock = minStock;

        DialogResult = true;
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
