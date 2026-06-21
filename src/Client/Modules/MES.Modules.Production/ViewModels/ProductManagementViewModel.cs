using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class ProductManagementViewModel : BindableBase
{
    private readonly IMasterDataService _masterDataService;
    private ObservableCollection<ProductInfo> _products = [];
    private ProductInfo? _selectedProduct;
    private string? _errorMessage;

    public ObservableCollection<ProductInfo> Products
    {
        get => _products;
        set => SetProperty(ref _products, value);
    }

    public ProductInfo? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            SetProperty(ref _selectedProduct, value);
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

    public ProductManagementViewModel(IMasterDataService masterDataService)
    {
        _masterDataService = masterDataService;
        AddCommand = new DelegateCommand(OnAdd);
        EditCommand = new DelegateCommand(OnEdit, () => SelectedProduct != null);
        DeleteCommand = new DelegateCommand(OnDelete, () => SelectedProduct != null);
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            ErrorMessage = null;
            var products = await _masterDataService.GetAllProductsAsync();
            Products = new ObservableCollection<ProductInfo>(products);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"数据加载失败: {ex.Message}";
        }
    }

    private void OnAdd()
    {
        var dialog = new Views.ProductDialogWindow(new ProductInfo());
        dialog.Owner = Application.Current.MainWindow;
        if (dialog.ShowDialog() == true)
        {
            _ = SaveProductAsync(dialog.Product);
        }
    }

    private void OnEdit()
    {
        if (SelectedProduct == null) return;
        var clone = Clone(SelectedProduct);
        var dialog = new Views.ProductDialogWindow(clone);
        dialog.Owner = Application.Current.MainWindow;
        if (dialog.ShowDialog() == true)
        {
            _ = SaveProductAsync(dialog.Product);
        }
    }

    private async Task SaveProductAsync(ProductInfo product)
    {
        try
        {
            await _masterDataService.SaveProductAsync(product);
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
        if (SelectedProduct == null) return;
        var result = MessageBox.Show(
            $"确认删除产品 {SelectedProduct.ProductName} ({SelectedProduct.ProductId})？",
            "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            await _masterDataService.DeleteProductAsync(SelectedProduct.ProductId);
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

    private static ProductInfo Clone(ProductInfo p) => new()
    {
        ProductId = p.ProductId,
        ProductName = p.ProductName,
        ProductCode = p.ProductCode,
        CustomerId = p.CustomerId,
        CustomerName = p.CustomerName,
        PackageType = p.PackageType,
        ProcessStage = p.ProcessStage,
        DefaultRouteId = p.DefaultRouteId,
        UnitQty = p.UnitQty,
        Status = p.Status,
        Description = p.Description,
    };
}
