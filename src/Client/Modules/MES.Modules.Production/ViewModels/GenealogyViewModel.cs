using System.Collections.ObjectModel;
using System.Windows.Input;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class GenealogyViewModel : BindableBase
{
    private readonly IGenealogyService _genealogyService;
    private readonly ILotSplitMergeService _splitMergeService;
    private readonly ICarrierService _carrierService;

    private string _searchLotId = string.Empty;
    private ObservableCollection<LotGenealogy> _genealogyRecords = [];
    private ObservableCollection<LotInfo> _relatedLots = [];
    private ObservableCollection<LotCarrierBinding> _carrierHistory = [];

    public string SearchLotId
    {
        get => _searchLotId;
        set => SetProperty(ref _searchLotId, value);
    }

    public ObservableCollection<LotGenealogy> GenealogyRecords
    {
        get => _genealogyRecords;
        set => SetProperty(ref _genealogyRecords, value);
    }

    public ObservableCollection<LotInfo> RelatedLots
    {
        get => _relatedLots;
        set => SetProperty(ref _relatedLots, value);
    }

    public ObservableCollection<LotCarrierBinding> CarrierHistory
    {
        get => _carrierHistory;
        set => SetProperty(ref _carrierHistory, value);
    }

    public ICommand SearchCommand { get; }
    public ICommand LoadCarrierHistoryCommand { get; }

    public GenealogyViewModel(IGenealogyService genealogyService, ILotSplitMergeService splitMergeService, ICarrierService carrierService)
    {
        _genealogyService = genealogyService;
        _splitMergeService = splitMergeService;
        _carrierService = carrierService;

        SearchCommand = new DelegateCommand(async () => await SearchGenealogyAsync());
        LoadCarrierHistoryCommand = new DelegateCommand(async () => await LoadCarrierHistoryAsync());
    }

    private async System.Threading.Tasks.Task SearchGenealogyAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchLotId)) return;

        var records = await _genealogyService.GetFullTreeAsync(SearchLotId);
        GenealogyRecords = new ObservableCollection<LotGenealogy>(records);

        var childLots = await _splitMergeService.GetChildLotsAsync(SearchLotId);
        RelatedLots = new ObservableCollection<LotInfo>(childLots);
    }

    private async System.Threading.Tasks.Task LoadCarrierHistoryAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchLotId)) return;

        var history = await _carrierService.GetCarrierHistoryAsync(SearchLotId);
        CarrierHistory = new ObservableCollection<LotCarrierBinding>(history);
    }
}
