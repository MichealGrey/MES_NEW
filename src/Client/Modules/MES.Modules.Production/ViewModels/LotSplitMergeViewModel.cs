using System.Collections.ObjectModel;
using System.Windows.Input;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class LotSplitMergeViewModel : BindableBase
{
    private readonly ILotSplitMergeService _splitMergeService;

    private string _motherLotId = string.Empty;
    private int _splitQty;
    private string _splitReason = string.Empty;
    private string _splitType = "Normal";
    private ObservableCollection<LotInfo> _childLots = [];
    private ObservableCollection<LotSplitRecord> _splitRecords = [];
    private string _message = string.Empty;

    public string MotherLotId
    {
        get => _motherLotId;
        set => SetProperty(ref _motherLotId, value);
    }

    public int SplitQty
    {
        get => _splitQty;
        set => SetProperty(ref _splitQty, value);
    }

    public string SplitReason
    {
        get => _splitReason;
        set => SetProperty(ref _splitReason, value);
    }

    public string SplitType
    {
        get => _splitType;
        set => SetProperty(ref _splitType, value);
    }

    public ObservableCollection<LotInfo> ChildLots
    {
        get => _childLots;
        set => SetProperty(ref _childLots, value);
    }

    public ObservableCollection<LotSplitRecord> SplitRecords
    {
        get => _splitRecords;
        set => SetProperty(ref _splitRecords, value);
    }

    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    public ICommand SplitCommand { get; }
    public ICommand LoadChildLotsCommand { get; }
    public ICommand LoadSplitRecordsCommand { get; }

    public LotSplitMergeViewModel(ILotSplitMergeService splitMergeService)
    {
        _splitMergeService = splitMergeService;

        SplitCommand = new DelegateCommand(async () => await SplitLotAsync());
        LoadChildLotsCommand = new DelegateCommand(async () => await LoadChildLotsAsync());
        LoadSplitRecordsCommand = new DelegateCommand(async () => await LoadSplitRecordsAsync());
    }

    private async System.Threading.Tasks.Task SplitLotAsync()
    {
        try
        {
            var record = await _splitMergeService.SplitLotAsync(
                MotherLotId, SplitQty, SplitReason, SplitType, "OP001", "操作员");

            Message = $"拆批成功：{record.ChildLotId}";
            await LoadChildLotsAsync();
            await LoadSplitRecordsAsync();
        }
        catch (System.Exception ex)
        {
            Message = $"拆批失败：{ex.Message}";
        }
    }

    private async System.Threading.Tasks.Task LoadChildLotsAsync()
    {
        if (string.IsNullOrWhiteSpace(MotherLotId)) return;

        var lots = await _splitMergeService.GetChildLotsAsync(MotherLotId);
        ChildLots = new ObservableCollection<LotInfo>(lots);
    }

    private async System.Threading.Tasks.Task LoadSplitRecordsAsync()
    {
        if (string.IsNullOrWhiteSpace(MotherLotId)) return;

        var records = await _splitMergeService.GetSplitRecordsAsync(MotherLotId);
        SplitRecords = new ObservableCollection<LotSplitRecord>(records);
    }
}
