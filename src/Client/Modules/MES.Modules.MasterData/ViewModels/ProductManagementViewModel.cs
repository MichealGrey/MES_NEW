using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using MES.Modules.MasterData.Services;

namespace MES.Modules.MasterData.ViewModels;

public class ProductManagementViewModel : BindableBase
{
    private readonly IMasterDataService _service;
    private ObservableCollection<ProductInfo> _products = [];
    private ProductInfo? _selectedProduct;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private bool _isEditing;
    private ProductInfo _newProduct = new();

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
            if (SetProperty(ref _selectedProduct, value))
            {
                if (_selectedProduct != null)
                    EditingProduct = new ProductInfo
                    {
                        ProductId = _selectedProduct.ProductId,
                        ProductName = _selectedProduct.ProductName,
                        ProductType = _selectedProduct.ProductType,
                        PackageType = _selectedProduct.PackageType,
                        DieName = _selectedProduct.DieName,
                        CustomerId = _selectedProduct.CustomerId,
                        CustomerPN = _selectedProduct.CustomerPN,
                        InternalPN = _selectedProduct.InternalPN,
                        Status = _selectedProduct.Status,
                        Description = _selectedProduct.Description,
                    };
            }
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
                ApplyFilter();
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }

    public ProductInfo EditingProduct
    {
        get => _newProduct;
        set => SetProperty(ref _newProduct, value);
    }

    public int TotalCount => Products.Count;
    public int ActiveCount => Products.Count(p => p.Status == "Active");
    public int InactiveCount => Products.Count(p => p.Status != "Active");

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand AddCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand CancelCommand { get; }
    public DelegateCommand<ProductInfo?> DeleteCommand { get; }

    public ProductManagementViewModel(IMasterDataService service)
    {
        _service = service;

        RefreshCommand = new DelegateCommand(OnRefresh);
        AddCommand = new DelegateCommand(OnAdd);
        SaveCommand = new DelegateCommand(OnSave, () => !string.IsNullOrWhiteSpace(EditingProduct.ProductId) && !string.IsNullOrWhiteSpace(EditingProduct.ProductName));
        CancelCommand = new DelegateCommand(OnCancel);
        DeleteCommand = new DelegateCommand<ProductInfo?>(OnDelete, p => p != null);

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            ErrorMessage = null;
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"数据加载失败: {ex.Message}";
        }
    }

    private async Task ReloadDataAsync()
    {
        var products = await _service.GetAllProductsAsync();
        Products = new ObservableCollection<ProductInfo>(products);
        UpdateStatistics();
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            _ = ReloadDataAsync();
            return;
        }

        var key = SearchText.Trim().ToLower();
        var filtered = Products.Where(p =>
            p.ProductId.ToLower().Contains(key)
            || p.ProductName.ToLower().Contains(key)
            || p.InternalPN.ToLower().Contains(key)
            || p.CustomerPN.ToLower().Contains(key)).ToList();

        Products = new ObservableCollection<ProductInfo>(filtered);
    }

    private void OnAdd()
    {
        EditingProduct = new ProductInfo { Status = "Active" };
        IsEditing = true;
        SaveCommand.RaiseCanExecuteChanged();
    }

    private async void OnSave()
    {
        try
        {
            await _service.SaveProductAsync(EditingProduct);
            IsEditing = false;
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存失败: {ex.Message}";
        }
    }

    private void OnCancel()
    {
        IsEditing = false;
        EditingProduct = new ProductInfo();
    }

    private async void OnDelete(ProductInfo? product)
    {
        if (product == null) return;
        try
        {
            if (System.Windows.MessageBox.Show($"确定删除产品 {product.ProductId}?", "确认", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes)
                return;

            await _service.DeleteProductAsync(product.ProductId);
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"删除失败: {ex.Message}";
        }
    }

    private async void OnRefresh()
    {
        try
        {
            ErrorMessage = null;
            SearchText = string.Empty;
            IsEditing = false;
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"刷新失败: {ex.Message}";
        }
    }

    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(TotalCount));
        RaisePropertyChanged(nameof(ActiveCount));
        RaisePropertyChanged(nameof(InactiveCount));
    }
}
