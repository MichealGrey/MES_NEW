namespace MES.Modules.Warehouse.Models;

public class FoupItem
{
    public string Id { get; set; } = string.Empty;
    public string CarrierId { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string Cleanliness { get; set; } = string.Empty;
    public int StripCount { get; set; }
    public string Status { get; set; } = "In Use";
    public DateTime LastCleaned { get; set; }
}

public class MaterialItem
{
    public string Id { get; set; } = string.Empty;
    public string MaterialNo { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public int StockQty { get; set; }
    public int MinQty { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public double UnitPrice { get; set; }
}

public class ReticleItem
{
    public string Id { get; set; } = string.Empty;
    public string ReticleNo { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Status { get; set; } = "Available";
    public int UseCount { get; set; }
    public int MaxUseCount { get; set; }
    public DateTime LastUsed { get; set; }
    public DateTime LastInspection { get; set; }
    public string Location { get; set; } = string.Empty;
}

public class StockerItem
{
    public string Id { get; set; } = string.Empty;
    public string StockerNo { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public int TotalSlots { get; set; }
    public int UsedSlots { get; set; }
    public string Status { get; set; } = "Normal";
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public DateTime LastMaintenance { get; set; }
}

public class InboundRecord
{
    public string Id { get; set; } = string.Empty;
    public string MaterialNo { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public int Qty { get; set; }
    public string Supplier { get; set; } = string.Empty;
    public string BatchNo { get; set; } = string.Empty;
    public DateTime InboundDate { get; set; }
    public string Operator { get; set; } = string.Empty;
    public string Status { get; set; } = "Received";
}

public class OutboundRecord
{
    public string Id { get; set; } = string.Empty;
    public string MaterialNo { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public int Qty { get; set; }
    public string Destination { get; set; } = string.Empty;
    public string WorkOrderNo { get; set; } = string.Empty;
    public DateTime OutboundDate { get; set; }
    public string Operator { get; set; } = string.Empty;
}

public class InventoryRecord
{
    public string MaterialNo { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int MinStock { get; set; }
    public int MaxStock { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public double TotalValue { get; set; }
}
