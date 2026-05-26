using System.Windows.Controls;
using MES.Shell.Models;
using MES.Shell.ViewModels;

namespace MES.Shell.Views
{
    /// <summary>
    /// NavigationView.xaml 的交互逻辑
    ///
    /// 事件处理与ViewModel协作
    /// - ListBox 的 SelectionChanged 事件不适合直接用Command绑定
    /// - 在Code-Behind中获取选中的NavigationItem，调用ViewModel方法
    /// </summary>
    public partial class NavigationView : UserControl
    {
        public NavigationView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 侧边菜单选中项变化 → 触发导航
        /// </summary>
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is NavigationItem item && item.IsLeaf)
            {
                if (DataContext is NavigationViewModel vm)
                {
                    vm.NavigateTo(item.ViewName);
                }
            }
        }
    }
}
