using System.Windows;
using System.Windows.Controls;
using MES.Modules.Production.ViewModels;

namespace MES.Modules.Production.Views;

public partial class LotOverviewView : UserControl
{
    public LotOverviewView() => InitializeComponent();

    private void OnClearFilterClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is LotOverviewViewModel vm)
        {
            vm.SearchText = string.Empty;
            vm.StatusFilter = null;
            vm.StageFilter = "All";
        }
    }
}
