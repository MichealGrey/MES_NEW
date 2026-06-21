using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class RouteManagementViewModel : BindableBase
{
    private readonly IMasterDataService _masterDataService;
    private ObservableCollection<RouteInfo> _routes = [];
    private RouteInfo? _selectedRoute;
    private string? _errorMessage;

    public ObservableCollection<RouteInfo> Routes
    {
        get => _routes;
        set => SetProperty(ref _routes, value);
    }

    public RouteInfo? SelectedRoute
    {
        get => _selectedRoute;
        set
        {
            SetProperty(ref _selectedRoute, value);
            RaiseCanExecuteChanged();
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public DelegateCommand AddCommand { get; }
    public DelegateCommand EditCommand { get; }
    public DelegateCommand DeleteCommand { get; }
    public DelegateCommand RefreshCommand { get; }

    public RouteManagementViewModel(IMasterDataService masterDataService)
    {
        _masterDataService = masterDataService;
        AddCommand = new DelegateCommand(OnAdd);
        EditCommand = new DelegateCommand(OnEdit, () => SelectedRoute != null);
        DeleteCommand = new DelegateCommand(OnDelete, () => SelectedRoute != null);
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            ErrorMessage = null;
            var routes = await _masterDataService.GetAllRoutesAsync();
            Routes = new ObservableCollection<RouteInfo>(routes);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"数据加载失败: {ex.Message}";
        }
    }

    private void OnAdd()
    {
        var dialog = new Views.RouteDialogWindow(new RouteInfo());
        dialog.Owner = Application.Current.MainWindow;
        if (dialog.ShowDialog() == true)
            _ = SaveRouteAsync(dialog.Route);
    }

    private void OnEdit()
    {
        if (SelectedRoute == null) return;
        var clone = Clone(SelectedRoute);
        var dialog = new Views.RouteDialogWindow(clone);
        dialog.Owner = Application.Current.MainWindow;
        if (dialog.ShowDialog() == true)
            _ = SaveRouteAsync(dialog.Route);
    }

    private async Task SaveRouteAsync(RouteInfo route)
    {
        try
        {
            await _masterDataService.SaveRouteAsync(route);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存失败: {ex.Message}";
            MessageBox.Show($"保存失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void OnDelete()
    {
        if (SelectedRoute == null) return;
        var result = MessageBox.Show(
            $"确认删除工艺路线 {SelectedRoute.RouteName} ({SelectedRoute.RouteId})？",
            "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            await _masterDataService.DeleteRouteAsync(SelectedRoute.RouteId);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"删除失败: {ex.Message}";
            MessageBox.Show($"删除失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void RaiseCanExecuteChanged()
    {
        EditCommand.RaiseCanExecuteChanged();
        DeleteCommand.RaiseCanExecuteChanged();
    }

    private static RouteInfo Clone(RouteInfo r) => new()
    {
        RouteId = r.RouteId,
        RouteName = r.RouteName,
        RouteCode = r.RouteCode,
        ProductId = r.ProductId,
        Version = r.Version,
        Status = r.Status,
        Description = r.Description,
    };
}
