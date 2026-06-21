using System.Windows;
using System.Windows.Controls;
using MES.Modules.Production.ViewModels;

namespace MES.Modules.Production.Views;

public partial class ReasonCodeManagementView : UserControl
{
    public ReasonCodeManagementView()
    {
        InitializeComponent();
    }

    private void cmbFilterCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is ReasonCodeManagementViewModel vm && cmbFilterCategory.SelectedItem is ComboBoxItem item)
        {
            var text = item.Content?.ToString();
            vm.SelectedCategory = text == "全部" ? string.Empty : text ?? string.Empty;
        }
    }

    private void cmbFilterApplicableTo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is ReasonCodeManagementViewModel vm && cmbFilterApplicableTo.SelectedItem is ComboBoxItem item)
        {
            var text = item.Content?.ToString();
            vm.SelectedApplicableTo = text == "全部" ? string.Empty : text ?? string.Empty;
        }
    }
}
