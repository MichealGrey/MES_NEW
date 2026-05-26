using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class TrackService : ITrackService
{
    private readonly ILotRepository _lotRepo;
    private readonly IRouteRepository _routeRepo;
    private readonly IRouteService _routeService;
    private readonly IQuantityService _quantityService;
    private readonly IOperationHistoryService _opHistoryService;
    private readonly IAuditService _auditService;
    private readonly IYieldService _yieldService;
    private readonly IGenealogyService _genealogyService;
    private readonly ICarrierService _carrierService;
    private readonly IEquipmentGateway _equipmentGateway;
    private readonly IRecipeGateway _recipeGateway;
    private readonly IQualityGateway _qualityGateway;
    private readonly IWarehouseGateway _warehouseGateway;
    private readonly IAlarmService _alarmService;
    private readonly ICarrierBindingRepository _carrierBindingRepo;
    private readonly IQuantityTransactionRepository _qtyTxnRepo;

    public TrackService(
        ILotRepository lotRepo,
        IRouteRepository routeRepo,
        IRouteService routeService,
        IQuantityService quantityService,
        IOperationHistoryService opHistoryService,
        IAuditService auditService,
        IYieldService yieldService,
        IGenealogyService genealogyService,
        ICarrierService carrierService,
        IEquipmentGateway equipmentGateway,
        IRecipeGateway recipeGateway,
        IQualityGateway qualityGateway,
        IWarehouseGateway warehouseGateway,
        IAlarmService alarmService,
        ICarrierBindingRepository carrierBindingRepo,
        IQuantityTransactionRepository qtyTxnRepo)
    {
        _lotRepo = lotRepo;
        _routeRepo = routeRepo;
        _routeService = routeService;
        _quantityService = quantityService;
        _opHistoryService = opHistoryService;
        _auditService = auditService;
        _yieldService = yieldService;
        _genealogyService = genealogyService;
        _carrierService = carrierService;
        _equipmentGateway = equipmentGateway;
        _recipeGateway = recipeGateway;
        _qualityGateway = qualityGateway;
        _warehouseGateway = warehouseGateway;
        _alarmService = alarmService;
        _carrierBindingRepo = carrierBindingRepo;
        _qtyTxnRepo = qtyTxnRepo;
    }

    public async Task<TrackValidationResult> ValidateTrackInAsync(TrackInRequest request)
    {
        var result = new TrackValidationResult();

        var lot = await _lotRepo.GetByIdAsync(request.LotId);
        if (lot is null)
        {
            result.AddError($"批次 {request.LotId} 不存在");
            return result;
        }

        var lotModel = MapToLotInfo(lot);
        if (!lotModel.CanTrackIn)
        {
            if (lotModel.IsCompleted) result.AddError("批次已完成，不可进站");
            else if (lotModel.IsScrapped) result.AddError("批次已报废，不可进站");
            else if (lotModel.IsHold) result.AddError($"批次处于 Hold 状态，禁止进站");
            else if (lotModel.IsUnderMRB) result.AddError("批次处于 MRB 审查中，禁止进站");
            else result.AddError($"批次状态 {lotModel.Status} 不允许进站");
        }

        var routeId = lot.ReworkRouteId ?? (string.IsNullOrEmpty(lot.RouteId) ? "DEFAULT" : lot.RouteId);
        if (string.IsNullOrEmpty(routeId))
        {
            result.AddWarning("未绑定工艺路线，使用默认路线");
        }
        else
        {
            var steps = await _routeService.GetStepsAsync(routeId);
            var currentStep = steps.FirstOrDefault(s => s.StepSeq == request.StepSeq);
            if (currentStep is null)
            {
                result.AddError($"工序 Step {request.StepSeq} 不存在于路线 {routeId}");
            }
            else
            {
                if (currentStep.StepSeq > 1)
                {
                    var prevStep = steps.FirstOrDefault(s => s.StepSeq == currentStep.StepSeq - 1);
                    if (prevStep != null)
                    {
                        var prevRecord = await _lotRepo.GetLotStepAsync(request.LotId, prevStep.StepCode);
                        if (prevRecord is null || prevRecord.Status != "Completed")
                        {
                            result.AddError($"上一站 {prevStep.StepName} 未完成，不可进站");
                        }
                    }
                }

                if (!string.IsNullOrEmpty(currentStep.RequiredCarrierType))
                {
                    result.AddWarning($"工序要求载具: {currentStep.RequiredCarrierType}");
                }
            }
        }

        if (string.IsNullOrEmpty(request.EquipmentId))
        {
            result.AddWarning("未输入设备号");
        }

        if (!string.IsNullOrEmpty(request.EquipmentId))
        {
            var equipCheck = await _equipmentGateway.CheckEquipmentAsync(request.EquipmentId, request.StepCode);
            if (!equipCheck.IsAllowed)
            {
                result.AddError(equipCheck.Reason ?? "设备校验失败");
                await _alarmService.RaiseAlarmAsync("EquipmentDown", equipCheck.Reason ?? "设备不允许进站",
                    lotId: request.LotId, equipmentId: request.EquipmentId, stepCode: request.StepCode);
            }
        }

        if (!string.IsNullOrEmpty(request.RecipeId))
        {
            var recipeApproved = await _recipeGateway.IsRecipeApprovedAsync(request.RecipeId, request.StepCode, request.EquipmentId);
            if (!recipeApproved)
            {
                result.AddError($"Recipe {request.RecipeId} 未审批或不匹配");
                await _alarmService.RaiseAlarmAsync("RecipeError", $"Recipe {request.RecipeId} 未审批",
                    lotId: request.LotId, equipmentId: request.EquipmentId, stepCode: request.StepCode);
            }
        }

        var requiresGate = await _qualityGateway.RequiresQualityGateAsync(request.LotId, request.StepCode);
        if (requiresGate)
        {
            var gatePassed = await _qualityGateway.IsQualityGatePassedAsync(request.LotId, request.StepCode);
            if (!gatePassed)
            {
                result.AddError($"工序 {request.StepCode} 需要质量 Gate 放行");
            }
        }

        var materialReady = await _warehouseGateway.IsMaterialReadyAsync(request.LotId, request.StepCode);
        if (!materialReady)
        {
            result.AddWarning($"工序 {request.StepCode} 物料不齐套");
            await _alarmService.CheckAndRaiseAsync("MaterialShort", request.LotId, request.EquipmentId, request.StepCode);
        }

        return result;
    }

    public async Task<TrackResult> TrackInAsync(TrackInRequest request)
    {
        var validation = await ValidateTrackInAsync(request);
        if (!validation.IsValid)
        {
            return new TrackResult { Success = false, Message = string.Join("; ", validation.Errors) };
        }

        var lot = await _lotRepo.GetByIdAsync(request.LotId);
        if (lot is null) return new TrackResult { Success = false, Message = "批次不存在" };
        var routeId = lot.ReworkRouteId ?? (string.IsNullOrEmpty(lot.RouteId) ? "DEFAULT" : lot.RouteId);

        var stepRecord = new ProdLotStep
        {
            RecordId = Guid.NewGuid().ToString("N"),
            LotId = request.LotId,
            RouteId = routeId,
            RouteVersion = lot.RouteVersion,
            StepSeq = request.StepSeq,
            StepName = request.StepName,
            StepCode = request.StepCode,
            Status = "Processing",
            TrackInEquipment = request.EquipmentId,
            TrackInCarrier = request.CarrierId,
            TrackInRecipe = request.RecipeId,
            TrackInTime = DateTime.UtcNow,
            TrackInOperator = request.OperatorName,
            InputQty = request.InputQty > 0 ? request.InputQty : lot.UnitCount,
        };

        await _lotRepo.GetDbContext().Set<ProdLotStep>().AddAsync(stepRecord);
        await _lotRepo.GetDbContext().SaveChangesAsync();

        lot.Status = "Processing";
        lot.CurrentStepCode = request.StepCode;
        lot.CurrentStepSeq = request.StepSeq;
        lot.UpdatedAt = DateTime.UtcNow;
        if (lot.OriginalQty == 0) lot.OriginalQty = lot.UnitCount;
        await _lotRepo.UpdateAsync(lot);

        if (!string.IsNullOrEmpty(request.CarrierId))
        {
            var steps = await _routeService.GetStepsAsync(routeId);
            var currentStep = steps.FirstOrDefault(s => s.StepSeq == request.StepSeq);
            var carrierType = currentStep?.RequiredCarrierType ?? string.Empty;

            await _carrierService.BindCarrierAsync(
                request.LotId, request.CarrierId, carrierType,
                request.StepCode, request.StepSeq, request.OperatorId);
        }

        await _opHistoryService.WriteAsync(
            request.LotId, lot.OrderId ?? "N/A", "TrackIn", request.StepName, request.StepSeq,
            request.EquipmentId, request.RecipeId, request.CarrierId,
            null, null, request.OperatorId, request.OperatorName,
            request.Workstation, request.Remark);

        await _auditService.WriteAsync(new AuditTrail
        {
            EntityType = "Lot", EntityId = request.LotId,
            Action = "TrackIn",
            OperatorId = request.OperatorId, OperatorName = request.OperatorName,
            BeforeState = lot.Status,
            AfterState = "Processing",
            Detail = $"进站: 工序 {request.StepName}, 设备 {request.EquipmentId}",
            Reason = request.Remark
        });

        return new TrackResult
        {
            Success = true,
            LotId = request.LotId,
            StepCode = request.StepCode,
            Message = $"{request.LotId} 进站 {request.StepName} 成功",
            TrackTime = DateTime.UtcNow,
            RecordId = stepRecord.RecordId,
            Warnings = validation.Warnings
        };
    }

    public async Task<TrackValidationResult> ValidateTrackOutAsync(TrackOutRequest request)
    {
        var (validation, lot, yield, autoHold) = await ValidateTrackOutAsyncWithLot(request);
        return validation;
    }

    public async Task<TrackResult> TrackOutAsync(TrackOutRequest request)
    {
        var (validation, lot, yield, autoHold) = await ValidateTrackOutAsyncWithLot(request);
        if (!validation.IsValid)
        {
            return new TrackResult { Success = false, Message = string.Join("; ", validation.Errors) };
        }

        var routeId = lot.ReworkRouteId ?? (string.IsNullOrEmpty(lot.RouteId) ? "DEFAULT" : lot.RouteId);

        var stepRecord = await _lotRepo.GetLotStepAsync(request.LotId, request.StepCode);
        if (stepRecord is null)
        {
            return new TrackResult { Success = false, Message = "未找到进站记录" };
        }

        stepRecord.Status = "Completed";
        stepRecord.TrackOutTime = DateTime.UtcNow;
        stepRecord.TrackOutOperator = request.OperatorName;
        stepRecord.PassQty = request.PassQty;
        stepRecord.FailQty = request.FailQty;
        stepRecord.ScrapQty = request.ScrapQty;
        stepRecord.ReworkQty = request.ReworkQty;
        stepRecord.HoldQty = request.HoldQty;
        stepRecord.PendingQty = request.PendingQty;
        stepRecord.RecipeId = request.RecipeId;
        stepRecord.Remark = request.Remark;
        _lotRepo.GetDbContext().Update(stepRecord);
        await _lotRepo.GetDbContext().SaveChangesAsync();

        await _qtyTxnRepo.AddAsync(new MES.Infrastructure.Persistence.Entities.QuantityTransaction
        {
            LotId = request.LotId,
            RouteId = routeId,
            StepSeq = request.StepSeq,
            StepCode = request.StepCode,
            StepName = request.StepName,
            EquipmentId = request.EquipmentId,
            InputQty = request.InputQty,
            PassQty = request.PassQty,
            FailQty = request.FailQty,
            ScrapQty = request.ScrapQty,
            ReworkQty = request.ReworkQty,
            HoldQty = request.HoldQty,
            PendingQty = request.PendingQty,
            OperatorId = request.OperatorId,
            OperatorName = request.OperatorName,
            Timestamp = DateTime.UtcNow,
        });

        lot.TotalPassQty += request.PassQty;
        lot.TotalScrapQty += request.ScrapQty;
        lot.TotalReworkQty += request.ReworkQty;
        lot.TotalHoldQty += request.HoldQty;
        lot.UnitCount = request.PassQty;
        lot.UpdatedAt = DateTime.UtcNow;

        var nextStep = await _routeService.GetNextStepAsync(request.LotId, routeId, lot.RouteVersion, request.StepSeq);
        string? nextStepName = null;

        if (nextStep is not null)
        {
            lot.CurrentStepSeq = nextStep.StepSeq;
            lot.CurrentStepCode = nextStep.StepCode;
            lot.Status = "Waiting";
            nextStepName = nextStep.StepName;
        }
        else
        {
            lot.Status = "Completed";
            lot.CurrentStepCode = request.StepCode + " (完成)";
        }

        if (autoHold)
        {
            lot.Status = "Hold";
            lot.HoldCategory = "YieldHold";
            lot.HoldReason = $"良率 {yield:F1}% 低于阈值，自动 Hold";
            lot.HoldTime = DateTime.Now;
            lot.HoldOperator = "SYSTEM";
        }

        await _alarmService.CheckAndRaiseAsync("LowYield", request.LotId, request.EquipmentId, request.StepCode, yieldValue: yield);
        await _lotRepo.UpdateAsync(lot);

        await _opHistoryService.WriteAsync(
            request.LotId, lot.OrderId ?? "N/A", "TrackOut", request.StepName, request.StepSeq,
            request.EquipmentId, request.RecipeId, null,
            request.InputQty, request.PassQty, request.OperatorId, request.OperatorName,
            request.Workstation, request.Remark);

        await _auditService.WriteAsync(new AuditTrail
        {
            EntityType = "Lot", EntityId = request.LotId,
            Action = "TrackOut",
            OperatorId = request.OperatorId, OperatorName = request.OperatorName,
            BeforeState = "Processing",
            AfterState = lot.Status,
            Detail = $"出站: 工序 {request.StepName}, 设备 {request.EquipmentId}, 良品 {request.PassQty}",
            Reason = request.Remark
        });

        if (nextStep is null)
        {
            await _genealogyService.RecordRelationAsync(new LotGenealogy
            {
                ParentLotId = request.LotId,
                ChildLotId = request.LotId,
                RelationType = "Completed",
                StepCode = request.StepCode,
                StepSeq = request.StepSeq,
                Qty = request.PassQty,
                OperatorId = request.OperatorId
            });
        }
        else if (lot.IsPartialLot && !string.IsNullOrEmpty(lot.MotherLotId))
        {
            await _genealogyService.RecordRelationAsync(new LotGenealogy
            {
                ParentLotId = lot.MotherLotId,
                ChildLotId = request.LotId,
                RelationType = "PartialCompleted",
                StepCode = request.StepCode,
                StepSeq = request.StepSeq,
                Qty = request.PassQty,
                OperatorId = request.OperatorId
            });
        }

        return new TrackResult
        {
            Success = true,
            LotId = request.LotId,
            StepCode = request.StepCode,
            Message = $"{request.LotId} 出站 {request.StepName} 成功，良率 {yield:F1}%",
            NextStepName = nextStepName,
            TrackTime = DateTime.UtcNow,
            Warnings = validation.Warnings
        };
    }

    public async Task<TrackResult> ForceTrackInAsync(TrackInRequest request, string reason)
    {
        var result = await TrackInAsync(request);
        if (result.Success)
        {
            await _auditService.WriteAsync(new AuditTrail
            {
                EntityType = "Lot", EntityId = request.LotId,
                Action = "ForceTrackIn",
                OperatorId = request.OperatorId, OperatorName = request.OperatorName,
                Reason = reason,
                Detail = $"强制进站: 工序 {request.StepName}"
            });
        }
        return result;
    }

    public async Task<TrackResult> ForceTrackOutAsync(TrackOutRequest request, string reason)
    {
        var result = await TrackOutAsync(request);
        if (result.Success)
        {
            await _auditService.WriteAsync(new AuditTrail
            {
                EntityType = "Lot", EntityId = request.LotId,
                Action = "ForceTrackOut",
                OperatorId = request.OperatorId, OperatorName = request.OperatorName,
                Reason = reason,
                Detail = $"强制出站: 工序 {request.StepName}"
            });
        }
        return result;
    }

    private async Task<(TrackValidationResult Validation, ProdLot Lot, double Yield, bool AutoHold)> ValidateTrackOutAsyncWithLot(TrackOutRequest request)
    {
        var result = new TrackValidationResult();

        var lot = await _lotRepo.GetByIdAsync(request.LotId);
        if (lot is null || lot.Status != "Processing")
        {
            result.AddError("批次未进站或不在加工中");
            return (result, lot!, 0.0, false);
        }

        var qtyValidation = await _quantityService.ValidateTrackOutQuantityAsync(new TrackOutRequest
        {
            InputQty = request.InputQty,
            PassQty = request.PassQty,
            FailQty = request.FailQty,
            ScrapQty = request.ScrapQty,
            ReworkQty = request.ReworkQty,
            HoldQty = request.HoldQty,
            PendingQty = request.PendingQty,
        });
        if (!qtyValidation.IsBalanced)
        {
            result.Errors.AddRange(qtyValidation.Errors);
            result.Warnings.AddRange(qtyValidation.Warnings);
        }

        var routeId = lot.ReworkRouteId ?? (string.IsNullOrEmpty(lot.RouteId) ? "DEFAULT" : lot.RouteId);
        var yield = request.InputQty > 0
            ? await _yieldService.CalculateStepYieldAsync(request.PassQty, request.InputQty)
            : 0.0;
        var autoHold = await _yieldService.ShouldAutoHoldAsync(routeId, request.StepCode, yield);
        if (autoHold && request.HoldQty <= 0)
        {
            result.AddWarning($"良率 {yield:F1}% 低于阈值，建议自动 Hold");
        }

        if (request.ScrapQty > 0 && request.InputQty > 0)
        {
            var scrapRate = (double)request.ScrapQty / request.InputQty * 100;
            if (scrapRate > 5)
            {
                result.AddWarning($"报废率 {scrapRate:F1}% 超过 5%，需要主管签核");
            }
        }

        return (result, lot, yield, autoHold);
    }

    private static LotInfo MapToLotInfo(ProdLot e)
    {
        return new LotInfo
        {
            LotId = e.LotId,
            OrderId = e.OrderId,
            ProductId = e.ProductId,
            ProductName = e.ProductName,
            DieName = e.DieName ?? string.Empty,
            PackageType = Enum.TryParse<MES.Domain.Production.PackageType>(e.PackageType, true, out var pt) ? pt : MES.Domain.Production.PackageType.QFP,
            CurrentStep = e.CurrentStepCode ?? string.Empty,
            Status = e.Status,
            UnitCount = e.UnitCount,
            StripCount = e.StripCount,
            Priority = e.Priority,
            RouteId = e.RouteId,
            RouteVersion = e.RouteVersion,
            CurrentStepSeq = e.CurrentStepSeq,
            IsPartialLot = e.IsPartialLot,
            MotherLotId = e.MotherLotId,
            SplitReason = e.SplitReason,
            SplitTime = e.SplitTime,
            SplitQty = e.SplitQty,
            IsReworkLot = e.IsReworkLot,
            OriginalRouteId = e.OriginalRouteId,
            ReworkRouteId = e.ReworkRouteId,
            ReworkCount = e.ReworkCount,
            ReworkReason = e.ReworkReason,
            IsArchived = e.IsArchived,
            IsUnderMRB = e.IsUnderMrb,
            MRBReference = e.MrbReference,
            MRBDisposition = e.MrbDisposition,
            Grade = e.Grade,
            OriginalLotId = e.OriginalLotId,
            WaferLotId = e.WaferLotId,
            OriginalQty = e.OriginalQty,
            TotalPassQty = e.TotalPassQty,
            TotalScrapQty = e.TotalScrapQty,
            TotalReworkQty = e.TotalReworkQty,
            TotalHoldQty = e.TotalHoldQty,
            CarrierType = Enum.TryParse<MES.Domain.Production.CarrierType>(e.CarrierType, true, out var ct) ? ct : MES.Domain.Production.CarrierType.Strip,
            CarrierId = e.CarrierId ?? string.Empty,
            BinResult = e.BinResult,
            TestResult = e.TestResult,
            QtyPass = e.QtyPass,
            QtyFail = e.QtyFail,
            HoldCategory = Enum.TryParse<HoldType>(e.HoldCategory, true, out var ht) ? ht : HoldType.Engineering,
            HoldReason = e.HoldReason,
            HoldTime = e.HoldTime,
            HoldOperator = e.HoldOperator,
            ReleaseCondition = e.ReleaseCondition,
        };
    }
}
