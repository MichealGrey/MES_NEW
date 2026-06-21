using Microsoft.EntityFrameworkCore;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Infrastructure.Persistence;

/// <summary>
/// Partial MesDbContext with Phase 1 entity DbSets.
/// </summary>
public partial class MesDbContext
{
    // === Phase 1: Quality ===
    public DbSet<IqcIncomingBatch> IqcIncomingBatches => Set<IqcIncomingBatch>();
    public DbSet<IqcInspectionTask> IqcInspectionTasks => Set<IqcInspectionTask>();
    public DbSet<IqcInspectionResult> IqcInspectionResults => Set<IqcInspectionResult>();
    public DbSet<IqcInspectionStandard> IqcInspectionStandards => Set<IqcInspectionStandard>();
    public DbSet<IqcSupplierQualityStat> IqcSupplierQualityStats => Set<IqcSupplierQualityStat>();
    public DbSet<FqcInspectionRecord> FqcInspectionRecords => Set<FqcInspectionRecord>();
    public DbSet<OqcInspectionRecord> OqcInspectionRecords => Set<OqcInspectionRecord>();
    public DbSet<ShipmentMslCheck> ShipmentMslChecks => Set<ShipmentMslCheck>();
    public DbSet<NonconformingRecord> NonconformingRecords => Set<NonconformingRecord>();
    public DbSet<MrbReview> MrbReviews => Set<MrbReview>();
    public DbSet<MrbReviewItem> MrbReviewItems => Set<MrbReviewItem>();
    public DbSet<DispositionRecord> DispositionRecords => Set<DispositionRecord>();

    // === Phase 1: Warehouse ===
    public DbSet<WarehouseReceipt> WarehouseReceipts => Set<WarehouseReceipt>();
    public DbSet<WarehouseInventory> WarehouseInventories => Set<WarehouseInventory>();
    public DbSet<WarehouseLocation> WarehouseLocations => Set<WarehouseLocation>();
    public DbSet<MaterialShelfLife> MaterialShelfLives => Set<MaterialShelfLife>();
    public DbSet<WarehouseIssueOrder> WarehouseIssueOrders => Set<WarehouseIssueOrder>();
    public DbSet<WarehouseIssueItem> WarehouseIssueItems => Set<WarehouseIssueItem>();
    public DbSet<WarehouseReturnOrder> WarehouseReturnOrders => Set<WarehouseReturnOrder>();
    public DbSet<WarehouseReturnItem> WarehouseReturnItems => Set<WarehouseReturnItem>();
    public DbSet<FinishedGoodsReceipt> FinishedGoodsReceipts => Set<FinishedGoodsReceipt>();
    public DbSet<FinishedGoodsInventory> FinishedGoodsInventories => Set<FinishedGoodsInventory>();
    public DbSet<FinishedGoodsShipment> FinishedGoodsShipments => Set<FinishedGoodsShipment>();
    public DbSet<FinishedGoodsShipmentItem> FinishedGoodsShipmentItems => Set<FinishedGoodsShipmentItem>();

    // === Phase 1: Abnormal / Equipment / FirstArticle / Alert ===
    public DbSet<AbnormalRecord> AbnormalRecords => Set<AbnormalRecord>();
    public DbSet<LineStopRecord> LineStopRecords => Set<LineStopRecord>();
    public DbSet<EquipmentFaultRecord> EquipmentFaultRecords => Set<EquipmentFaultRecord>();
    public DbSet<EquipmentRepairSparePart> EquipmentRepairSpareParts => Set<EquipmentRepairSparePart>();
    public DbSet<EquipmentPmPlan> EquipmentPmPlans => Set<EquipmentPmPlan>();
    public DbSet<EquipmentPmExecution> EquipmentPmExecutions => Set<EquipmentPmExecution>();
    public DbSet<FirstArticleInspection> FirstArticleInspections => Set<FirstArticleInspection>();
    public DbSet<FirstArticleInspectionItem> FirstArticleInspectionItems => Set<FirstArticleInspectionItem>();
    public DbSet<FirstArticleSignature> FirstArticleSignatures => Set<FirstArticleSignature>();
    public DbSet<BondPullTestRecord> BondPullTestRecords => Set<BondPullTestRecord>();
    public DbSet<QualityAlert> QualityAlerts => Set<QualityAlert>();
    public DbSet<QualityAlertAffectedLot> QualityAlertAffectedLots => Set<QualityAlertAffectedLot>();
    public DbSet<RecallNotice> RecallNotices => Set<RecallNotice>();
    public DbSet<RecallNoticeItem> RecallNoticeItems => Set<RecallNoticeItem>();
}
