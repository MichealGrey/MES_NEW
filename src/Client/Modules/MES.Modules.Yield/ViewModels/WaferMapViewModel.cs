using Prism.Mvvm;
using MES.Modules.Yield.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MES.Modules.Yield.ViewModels;

public class WaferMapViewModel : BindableBase
{
    private ObservableCollection<WaferDieItem> _items = [];

    public ObservableCollection<WaferDieItem> Items { get => _items; set => SetProperty(ref _items, value); }

    public int TotalDies => Items.Count;
    public int PassCount => Items.Count(d => d.Result == "Pass");
    public int FailCount => Items.Count(d => d.Result == "Fail");
    public double DieYield => TotalDies > 0 ? (double)PassCount / TotalDies * 100 : 0;

    public WaferMapViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        // Generate a simplified strip unit map (10x10 grid for strip inspection)
        var dies = new List<WaferDieItem>();
        var random = new Random(42);

        for (int row = 0; row < 10; row++)
        {
            for (int col = 0; col < 10; col++)
            {
                // Edge units have higher fail probability
                double failProb = (row == 0 || row == 9) ? 0.20 : 0.05;
                bool isFail = random.NextDouble() < failProb;
                dies.Add(new WaferDieItem
                {
                    Row = row,
                    Col = col,
                    DieId = $"U{row:D2}{col:D2}",
                    Result = isFail ? "Fail" : "Pass"
                });
            }
        }

        Items = new ObservableCollection<WaferDieItem>(dies);
    }
}
