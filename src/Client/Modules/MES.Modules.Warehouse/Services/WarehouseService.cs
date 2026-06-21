using MES.Modules.Warehouse.Models;

namespace MES.Modules.Warehouse.Services;

public class WarehouseService : IWarehouseService
{
    private readonly List<FoupItem> _foups = [];
    private readonly List<MaterialItem> _materials = [];
    private readonly List<ReticleItem> _reticles = [];
    private readonly List<StockerItem> _stockers = [];
    private readonly List<InboundRecord> _inboundRecords = [];
    private readonly List<OutboundRecord> _outboundRecords = [];

    public WarehouseService() => SeedData();

    private void SeedData()
    {
        _foups.AddRange([
            new FoupItem { Id = "FOUP-001", CarrierId = "FOUP-001", Location = "STKR-A-01", LotId = "LOT-2024-001", Cleanliness = "Class 1", StripCount = 25, Status = "In Use", LastCleaned = DateTime.Now.AddDays(-3) },
            new FoupItem { Id = "FOUP-002", CarrierId = "FOUP-002", Location = "STKR-A-02", LotId = "LOT-2024-002", Cleanliness = "Class 1", StripCount = 25, Status = "In Use", LastCleaned = DateTime.Now.AddDays(-5) },
            new FoupItem { Id = "FOUP-003", CarrierId = "FOUP-003", Location = "STKR-B-01", LotId = string.Empty, Cleanliness = "Class 1", StripCount = 0, Status = "Available", LastCleaned = DateTime.Now.AddDays(-1) },
            new FoupItem { Id = "FOUP-004", CarrierId = "FOUP-004", Location = "STKR-B-02", LotId = "LOT-2024-003", Cleanliness = "Class 2", StripCount = 20, Status = "In Use", LastCleaned = DateTime.Now.AddDays(-10) },
            new FoupItem { Id = "FOUP-005", CarrierId = "FOUP-005", Location = string.Empty, LotId = string.Empty, Cleanliness = "Class 1", StripCount = 0, Status = "Cleaning", LastCleaned = DateTime.Now },
        ]);

        _materials.AddRange([
            new MaterialItem { Id = "MAT-001", MaterialNo = "MAT-WF-001", MaterialName = "8寸硅片", Category = "Wafer", Supplier = "信越化学", StockQty = 500, MinQty = 100, Unit = "片", Location = "WH-A-01", ExpiryDate = DateTime.Now.AddMonths(6), UnitPrice = 120.0 },
            new MaterialItem { Id = "MAT-002", MaterialNo = "MAT-AU-001", MaterialName = "金线0.8mil", Category = "Wire", Supplier = "田中贵金属", StockQty = 200, MinQty = 50, Unit = "轴", Location = "WH-A-02", ExpiryDate = DateTime.Now.AddYears(1), UnitPrice = 45.0 },
            new MaterialItem { Id = "MAT-003", MaterialNo = "MAT-EP-001", MaterialName = "环氧树脂", Category = "Epoxy", Supplier = "Namics", StockQty = 8, MinQty = 10, Unit = "瓶", Location = "WH-B-01", ExpiryDate = DateTime.Now.AddDays(30), UnitPrice = 350.0 },
            new MaterialItem { Id = "MAT-004", MaterialNo = "MAT-LF-001", MaterialName = "引线框架-QFN48", Category = "Leadframe", Supplier = "三井高科技", StockQty = 5000, MinQty = 1000, Unit = "片", Location = "WH-B-02", ExpiryDate = null, UnitPrice = 2.5 },
            new MaterialItem { Id = "MAT-005", MaterialNo = "MAT-MC-001", MaterialName = " molding compound", Category = "Compound", Supplier = "Sumitomo", StockQty = 25, MinQty = 5, Unit = "kg", Location = "WH-C-01", ExpiryDate = DateTime.Now.AddDays(90), UnitPrice = 85.0 },
        ]);

        _reticles.AddRange([
            new ReticleItem { Id = "RET-001", ReticleNo = "RET-QFN48-01", ProductName = "QFN48", Status = "Available", UseCount = 2500, MaxUseCount = 10000, LastUsed = DateTime.Now.AddDays(-2), LastInspection = DateTime.Now.AddDays(-30), Location = "RET-CAB-A" },
            new ReticleItem { Id = "RET-002", ReticleNo = "RET-SOP8-01", ProductName = "SOP8", Status = "In Use", UseCount = 8500, MaxUseCount = 10000, LastUsed = DateTime.Now, LastInspection = DateTime.Now.AddDays(-15), Location = "RET-CAB-A" },
            new ReticleItem { Id = "RET-003", ReticleNo = "RET-BGA256-01", ProductName = "BGA256", Status = "Maintenance", UseCount = 9800, MaxUseCount = 10000, LastUsed = DateTime.Now.AddDays(-5), LastInspection = DateTime.Now.AddDays(-60), Location = "RET-CAB-B" },
        ]);

        _stockers.AddRange([
            new StockerItem { Id = "STKR-001", StockerNo = "STKR-A", Zone = "A", TotalSlots = 200, UsedSlots = 156, Status = "Normal", Temperature = 22.5, Humidity = 45.2, LastMaintenance = DateTime.Now.AddDays(-15) },
            new StockerItem { Id = "STKR-002", StockerNo = "STKR-B", Zone = "B", TotalSlots = 150, UsedSlots = 98, Status = "Normal", Temperature = 23.1, Humidity = 43.8, LastMaintenance = DateTime.Now.AddDays(-20) },
            new StockerItem { Id = "STKR-003", StockerNo = "STKR-C", Zone = "C", TotalSlots = 100, UsedSlots = 95, Status = "Warning", Temperature = 24.5, Humidity = 50.1, LastMaintenance = DateTime.Now.AddDays(-45) },
        ]);

        _inboundRecords.AddRange([
            new InboundRecord { Id = "IN-001", MaterialNo = "MAT-WF-001", MaterialName = "8寸硅片", Qty = 100, Supplier = "信越化学", BatchNo = "BAT-2024-001", InboundDate = DateTime.Now.AddDays(-1), Operator = "张工", Status = "Received" },
            new InboundRecord { Id = "IN-002", MaterialNo = "MAT-AU-001", MaterialName = "金线0.8mil", Qty = 50, Supplier = "田中贵金属", BatchNo = "BAT-2024-002", InboundDate = DateTime.Now.AddDays(-3), Operator = "李工", Status = "Inspected" },
            new InboundRecord { Id = "IN-003", MaterialNo = "MAT-EP-001", MaterialName = "环氧树脂", Qty = 20, Supplier = "Namics", BatchNo = "BAT-2024-003", InboundDate = DateTime.Now.AddDays(-5), Operator = "王工", Status = "Inspected" },
        ]);

        _outboundRecords.AddRange([
            new OutboundRecord { Id = "OUT-001", MaterialNo = "MAT-WF-001", MaterialName = "8寸硅片", Qty = 50, Destination = "Dicing", WorkOrderNo = "WO-2024-001", OutboundDate = DateTime.Now.AddHours(-2), Operator = "张工" },
            new OutboundRecord { Id = "OUT-002", MaterialNo = "MAT-LF-001", MaterialName = "引线框架-QFN48", Qty = 500, Destination = "Wire Bonding", WorkOrderNo = "WO-2024-002", OutboundDate = DateTime.Now.AddHours(-4), Operator = "李工" },
            new OutboundRecord { Id = "OUT-003", MaterialNo = "MAT-MC-001", MaterialName = "Molding Compound", Qty = 5, Destination = "Molding", WorkOrderNo = "WO-2024-003", OutboundDate = DateTime.Now.AddHours(-6), Operator = "王工" },
        ]);
    }

    public async Task<List<FoupItem>> GetFoupsAsync() { await Task.Delay(50); return _foups.ToList(); }
    public async Task SaveFoupAsync(FoupItem item) { await Task.Delay(50); var existing = _foups.FirstOrDefault(f => f.Id == item.Id); if (existing is null) _foups.Add(item); else _foups[_foups.IndexOf(existing)] = item; }
    public async Task<List<MaterialItem>> GetMaterialsAsync() { await Task.Delay(50); return _materials.ToList(); }
    public async Task SaveMaterialAsync(MaterialItem item) { await Task.Delay(50); var existing = _materials.FirstOrDefault(m => m.Id == item.Id); if (existing is null) _materials.Add(item); else _materials[_materials.IndexOf(existing)] = item; }
    public async Task DeleteMaterialAsync(string materialId) { await Task.Delay(50); var item = _materials.FirstOrDefault(m => m.Id == materialId); if (item is not null) _materials.Remove(item); }
    public async Task<List<ReticleItem>> GetReticlesAsync() { await Task.Delay(50); return _reticles.ToList(); }
    public async Task SaveReticleAsync(ReticleItem item) { await Task.Delay(50); var existing = _reticles.FirstOrDefault(r => r.Id == item.Id); if (existing is null) _reticles.Add(item); else _reticles[_reticles.IndexOf(existing)] = item; }
    public async Task<List<StockerItem>> GetStockersAsync() { await Task.Delay(50); return _stockers.ToList(); }
    public async Task SaveStockerAsync(StockerItem item) { await Task.Delay(50); var existing = _stockers.FirstOrDefault(s => s.Id == item.Id); if (existing is null) _stockers.Add(item); else _stockers[_stockers.IndexOf(existing)] = item; }
    public async Task<List<InboundRecord>> GetInboundRecordsAsync() { await Task.Delay(50); return _inboundRecords.ToList(); }
    public async Task SaveInboundRecordAsync(InboundRecord record) { await Task.Delay(50); var existing = _inboundRecords.FirstOrDefault(r => r.Id == record.Id); if (existing is null) _inboundRecords.Add(record); else _inboundRecords[_inboundRecords.IndexOf(existing)] = record; }
    public async Task<List<OutboundRecord>> GetOutboundRecordsAsync() { await Task.Delay(50); return _outboundRecords.ToList(); }
    public async Task SaveOutboundRecordAsync(OutboundRecord record) { await Task.Delay(50); var existing = _outboundRecords.FirstOrDefault(r => r.Id == record.Id); if (existing is null) _outboundRecords.Add(record); else _outboundRecords[_outboundRecords.IndexOf(existing)] = record; }
    public async Task<List<InventoryRecord>> GetInventoryRecordsAsync()
    {
        await Task.Delay(50);
        return _materials.Select(m => new InventoryRecord
        {
            MaterialNo = m.MaterialNo,
            MaterialName = m.MaterialName,
            CurrentStock = m.StockQty,
            MinStock = m.MinQty,
            MaxStock = 0,
            Unit = m.Unit,
            Location = m.Location,
            TotalValue = m.StockQty * m.UnitPrice
        }).ToList();
    }
}
