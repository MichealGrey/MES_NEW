using Prism.Mvvm;

namespace MES.Shell.Models;

public class MenuItemInfo : BindableBase
{
    private string _moduleKey = string.Empty;
    private string _displayName = string.Empty;
    private bool _isSelected;
    public string ModuleKey { get => _moduleKey; set => SetProperty(ref _moduleKey, value); }
    public string DisplayName { get => _displayName; set => SetProperty(ref _displayName, value); }
    public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }
}
