using System.Windows;
using System.Windows.Controls;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Views;

public partial class CustomerDialogWindow : Window
{
    public CustomerInfo Customer { get; }

    public CustomerDialogWindow(CustomerInfo customer)
    {
        Customer = customer;
        InitializeComponent();

        txtCustomerId.Text = customer.CustomerId;
        txtCustomerName.Text = customer.CustomerName;
        txtCustomerCode.Text = customer.CustomerCode;
        txtContactPerson.Text = customer.ContactPerson ?? string.Empty;
        txtContactPhone.Text = customer.ContactPhone ?? string.Empty;
        txtEmail.Text = customer.Email ?? string.Empty;
        txtAddress.Text = customer.Address ?? string.Empty;
        txtCustomerPnPrefix.Text = customer.CustomerPnPrefix ?? string.Empty;
        txtDefaultPackingSpec.Text = customer.DefaultPackingSpec ?? string.Empty;
        txtDefaultOqcSpec.Text = customer.DefaultOqcSpec ?? string.Empty;

        SelectComboBoxItem(cmbQualityLevel, customer.QualityLevel);
        SelectComboBoxItem(cmbStatus, customer.Status);
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
        {
            MessageBox.Show("客户名称不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (string.IsNullOrWhiteSpace(txtCustomerCode.Text))
        {
            MessageBox.Show("客户代码不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Customer.CustomerName = txtCustomerName.Text.Trim();
        Customer.CustomerCode = txtCustomerCode.Text.Trim();
        Customer.ContactPerson = txtContactPerson.Text.Trim();
        Customer.ContactPhone = txtContactPhone.Text.Trim();
        Customer.Email = txtEmail.Text.Trim();
        Customer.Address = txtAddress.Text.Trim();
        Customer.CustomerPnPrefix = txtCustomerPnPrefix.Text.Trim();
        Customer.QualityLevel = (cmbQualityLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Industrial";
        Customer.DefaultPackingSpec = txtDefaultPackingSpec.Text.Trim();
        Customer.DefaultOqcSpec = txtDefaultOqcSpec.Text.Trim();
        Customer.Status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Active";

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
