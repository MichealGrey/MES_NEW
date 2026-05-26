using System.Windows.Controls;
using System.Windows.Input;

namespace MES.Modules.Production.Views;

public partial class WorkOrderListView : UserControl
{
    public WorkOrderListView()
    {
        InitializeComponent();
    }

    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is ViewModels.WorkOrderListViewModel vm && vm.SelectedWorkOrder != null)
        {
            vm.ViewDetailCommand.Execute(vm.SelectedWorkOrder);
        }
    }
}
