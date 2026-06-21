using System.Windows;
using System.Windows.Controls;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Views;

public partial class ProductDialogWindow : Window
{
    public ProductInfo Product { get; }

    public ProductDialogWindow(ProductInfo product)
    {
        Product = product;
        InitializeComponent();

        // Populate fields
        txtProductId.Text = product.ProductId;
        txtProductName.Text = product.ProductName;
        txtProductCode.Text = product.ProductCode;
        txtCustomerId.Text = product.CustomerId;
        txtCustomerName.Text = product.CustomerName;
        txtDescription.Text = product.Description;

        // Select package type
        foreach (ComboBoxItem item in cmbPackageType.Items)
        {
            if (item.Content?.ToString() == product.PackageType)
            {
                cmbPackageType.SelectedItem = item;
                break;
            }
        }
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtProductName.Text))
        {
            MessageBox.Show("产品名称不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Product.ProductId = txtProductId.Text.Trim();
        Product.ProductName = txtProductName.Text.Trim();
        Product.ProductCode = txtProductCode.Text.Trim();
        Product.CustomerId = txtCustomerId.Text.Trim();
        Product.CustomerName = txtCustomerName.Text.Trim();
        Product.PackageType = (cmbPackageType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
        Product.Description = txtDescription.Text.Trim();

        DialogResult = true;
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
