using Microsoft.EntityFrameworkCore;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Infrastructure.Persistence;

/// <summary>
/// Partial MesDbContext with Phase 2 entity DbSets.
/// </summary>
public partial class MesDbContext
{
    // === Phase 2: Order Review ===
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<OrderReview> OrderReviews => Set<OrderReview>();
    public DbSet<OrderReviewItem> OrderReviewItems => Set<OrderReviewItem>();
    public DbSet<OrderVersion> OrderVersions => Set<OrderVersion>();

    // === Phase 2: Master Production Plan ===
    public DbSet<MasterProductionPlan> MasterProductionPlans => Set<MasterProductionPlan>();
    public DbSet<CapacityLoad> CapacityLoads => Set<CapacityLoad>();
    public DbSet<CapacitySimulation> CapacitySimulations => Set<CapacitySimulation>();

    // === Phase 2: BOM & MRP ===
    public DbSet<Bom> Boms => Set<Bom>();
    public DbSet<BomItem> BomItems => Set<BomItem>();
    public DbSet<MrpCalculation> MrpCalculations => Set<MrpCalculation>();
    public DbSet<MrpShortageWarning> MrpShortageWarnings => Set<MrpShortageWarning>();

    // === Phase 2: Order Progress & OTD ===
    public DbSet<OrderProgressSnapshot> OrderProgressSnapshots => Set<OrderProgressSnapshot>();
    public DbSet<OtdStatistics> OtdStatistics => Set<OtdStatistics>();
    public DbSet<DelayReasonRecord> DelayReasonRecords => Set<DelayReasonRecord>();

    // === Phase 2: Rush Order ===
    public DbSet<RushOrderRequest> RushOrderRequests => Set<RushOrderRequest>();
    public DbSet<RushOrderImpact> RushOrderImpacts => Set<RushOrderImpact>();
}
