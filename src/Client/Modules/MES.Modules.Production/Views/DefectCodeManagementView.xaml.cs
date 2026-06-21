using System.Windows.Controls;
using MES.Modules.Production.ViewModels;

namespace MES.Modules.Production.Views;

public partial class DefectCodeManagementView : UserControl
{
    public DefectCodeManagementView()
    {
        InitializeComponent();
    }

    private void cmbFilterCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is DefectCodeManagementViewModel vm && cmbFilterCategory.SelectedItem is ComboBoxItem item)
        {
            var text = item.Content?.ToString();
            vm.SelectedCategory = text == "全部" ? string.Empty : text ?? string.Empty;
        }
    }
}
