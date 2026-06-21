using System.Windows;
using System.Windows.Controls;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Views;

public partial class RouteDialogWindow : Window
{
    public RouteInfo Route { get; }

    public RouteDialogWindow(RouteInfo route)
    {
        Route = route;
        InitializeComponent();

        txtRouteId.Text = route.RouteId;
        txtRouteName.Text = route.RouteName;
        txtRouteCode.Text = route.RouteCode;
        txtProductId.Text = route.ProductId;
        txtVersion.Text = route.Version;
        txtDescription.Text = route.Description;
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtRouteName.Text))
        {
            MessageBox.Show("路线名称不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Route.RouteId = txtRouteId.Text.Trim();
        Route.RouteName = txtRouteName.Text.Trim();
        Route.RouteCode = txtRouteCode.Text.Trim();
        Route.ProductId = txtProductId.Text.Trim();
        Route.Version = txtVersion.Text.Trim();
        Route.Description = txtDescription.Text.Trim();

        DialogResult = true;
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
