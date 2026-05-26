using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace MES.Shell.Models;

public class NavigationItem : BindableBase
{
    private string _title = string.Empty;
    private string _viewName = string.Empty;
    private string _icon = string.Empty;
    private bool _isSelected;
    public string Title { get => _title; set => SetProperty(ref _title, value); }
    public string ViewName { get => _viewName; set => SetProperty(ref _viewName, value); }
    public string Icon { get => _icon; set => SetProperty(ref _icon, value); }
    public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }
    public ObservableCollection<NavigationItem> Children { get; } = [];
    public bool IsLeaf => !string.IsNullOrEmpty(ViewName);
}
