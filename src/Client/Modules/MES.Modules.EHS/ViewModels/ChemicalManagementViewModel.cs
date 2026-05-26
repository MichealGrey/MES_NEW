using Prism.Mvvm;
using MES.Modules.EHS.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.EHS.ViewModels;

public class ChemicalManagementViewModel : BindableBase
{
    private ObservableCollection<ChemicalItem> _items = [];
    private ChemicalItem? _selectedItem;

    public ObservableCollection<ChemicalItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public ChemicalItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public ChemicalManagementViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<ChemicalItem>
        {
            new() { ChemicalName = "EMC G700", CAS = "N/A-EMC-700", Quantity = 500.0, Unit = "kg", Location = "Chem-Store-C", MsdsStatus = "Valid", ExpiryDate = DateTime.Now.AddMonths(6) },
            new() { ChemicalName = "Underfill UF-300", CAS = "N/A-UF-300", Quantity = 300.0, Unit = "kg", Location = "Chem-Store-C", MsdsStatus = "Valid", ExpiryDate = DateTime.Now.AddMonths(3) },
            new() { ChemicalName = "Flux FM-280", CAS = "N/A-FX-280", Quantity = 100.0, Unit = "L", Location = "Chem-Store-D", MsdsStatus = "Valid", ExpiryDate = DateTime.Now.AddMonths(4) },
            new() { ChemicalName = "Solder Paste SAC305", CAS = "N/A-SP-305", Quantity = 50.0, Unit = "kg", Location = "Chem-Store-A", MsdsStatus = "Valid", ExpiryDate = DateTime.Now.AddMonths(8) },
            new() { ChemicalName = "Dicing Tape DT-200", CAS = "N/A-DT-200", Quantity = 200.0, Unit = "卷", Location = "Chem-Store-A", MsdsStatus = "Valid", ExpiryDate = DateTime.Now.AddMonths(5) },
            new() { ChemicalName = "Mold Compound MC-15", CAS = "N/A-MC-015", Quantity = 100.0, Unit = "kg", Location = "Chem-Store-B", MsdsStatus = "Expired", ExpiryDate = DateTime.Now.AddMonths(-1) },
            new() { ChemicalName = "Isopropyl Alcohol (IPA)", CAS = "67-63-0", Quantity = 250.0, Unit = "L", Location = "Chem-Store-B", MsdsStatus = "Valid", ExpiryDate = DateTime.Now.AddMonths(12) },
            new() { ChemicalName = "Conductive Adhesive CA-80", CAS = "N/A-CA-080", Quantity = 80.0, Unit = "kg", Location = "Chem-Store-D", MsdsStatus = "Valid", ExpiryDate = DateTime.Now.AddMonths(2) },
            new() { ChemicalName = "Cleaning Solvent CS-40", CAS = "N/A-CS-040", Quantity = 150.0, Unit = "L", Location = "Chem-Store-D", MsdsStatus = "Expiring", ExpiryDate = DateTime.Now.AddDays(14) },
            new() { ChemicalName = "Die Attach Film DAF-10", CAS = "N/A-DAF-010", Quantity = 60.0, Unit = "卷", Location = "Chem-Store-D", MsdsStatus = "Valid", ExpiryDate = DateTime.Now.AddMonths(9) },
        };
    }
}
