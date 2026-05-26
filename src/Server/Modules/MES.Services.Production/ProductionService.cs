using System.Collections.Concurrent;
using MES.Contracts.Production;
using MES.Services.Production.Models;

namespace MES.Services.Production;

public class ProductionService : IProductionService
{
    // In-memory stores (V1 - no database yet)
    private readonly ConcurrentDictionary<string, RouteInfo> _routes = new();
    private readonly ConcurrentDictionary<string, MES.Contracts.Production.LotDto> _lots = new();
    private readonly ConcurrentDictionary<string, LotStepRecord> _stepRecords = new();
    private readonly ConcurrentDictionary<string, QuantityTransaction> _quantityTransactions = new();
    private readonly ConcurrentDictionary<string, AuditTrail> _auditTrails = new();
    private readonly ConcurrentDictionary<string, object> _operationRecords = new();

    public Task<List<RouteInfo>> GetAllRoutesAsync()
    {
        return Task.FromResult(_routes.Values.ToList());
    }

    public Task<RouteInfo?> GetRouteAsync(string routeId, string? version = null)
    {
        var key = string.IsNullOrEmpty(version) ? routeId : $"{routeId}@{version}";
        _routes.TryGetValue(key, out var route);
        return Task.FromResult(route);
    }

    public Task<List<RouteStep>> GetRouteStepsAsync(string routeId, string? version = null)
    {
        var key = string.IsNullOrEmpty(version) ? routeId : $"{routeId}@{version}";
        if (_routes.TryGetValue(key, out var route))
        {
            return Task.FromResult(route.Steps.OrderBy(s => s.StepSeq).ToList());
        }
        return Task.FromResult(new List<RouteStep>());
    }

    public async Task<TrackValidationResult> ValidateTrackInAsync(TrackInRequest request)
    {
        var result = new TrackValidationResult
        {
            LotId = request.LotId,
            EquipmentId = request.EquipmentId,
            IsValid = true
        };

        // Check lot exists
        if (!_lots.TryGetValue(request.LotId, out var lot))
        {
            result.IsValid = false;
            result.Errors.Add($"Lot '{request.LotId}' not found.");
            return result;
        }

        result.LotStatus = lot.Status;

        // Check lot status allows TrackIn
        if (lot.Status is not ("Waiting" or "InProduction"))
        {
            result.IsValid = false;
            result.Errors.Add($"Lot status '{lot.Status}' does not allow TrackIn.");
        }

        // Check route exists
        var routeKey = lot.RouteId;
        if (!_routes.TryGetValue(routeKey, out var route))
        {
            result.IsValid = false;
            result.Errors.Add($"Route '{lot.RouteId}' not found.");
            return result;
        }

        result.RouteId = route.RouteId;

        // Find current step
        var currentStep = route.Steps.FirstOrDefault(s => s.StepCode == lot.CurrentStep);
        if (currentStep is null)
        {
            // Lot has no current step, use first step
            currentStep = route.Steps.OrderBy(s => s.StepSeq).FirstOrDefault();
            if (currentStep is null)
            {
                result.IsValid = false;
                result.Errors.Add("No steps defined in route.");
                return result;
            }
        }

        result.TargetStepCode = currentStep.StepCode;

        // Check if step is already in progress for this lot
        var existingRecord = _stepRecords.Values.FirstOrDefault(r =>
            r.LotId == request.LotId &&
            r.StepCode == currentStep.StepCode &&
            r.Status == "InProgress");

        if (existingRecord is not null)
        {
            result.IsValid = false;
            result.Errors.Add($"Step '{currentStep.StepCode}' is already in progress for this lot.");
        }

        return result;
    }

    public async Task<TrackResultDto> TrackInAsync(TrackInRequest request)
    {
        var validationResult = await ValidateTrackInAsync(request);
        if (!validationResult.IsValid)
        {
            return new TrackResultDto
            {
                Success = false,
                Message = string.Join("; ", validationResult.Errors),
                LotId = request.LotId
            };
        }

        // Get lot and route
        _lots.TryGetValue(request.LotId, out var lot);
        var routeKey = lot!.RouteId;
        _routes.TryGetValue(routeKey, out var route);

        // Determine target step
        var targetStep = route!.Steps.FirstOrDefault(s => s.StepCode == lot.CurrentStep);
        if (targetStep is null)
        {
            targetStep = route.Steps.OrderBy(s => s.StepSeq).First();
        }

        // Create step record
        var recordId = $"SR-{request.LotId}-{targetStep.StepCode}-{DateTime.UtcNow:yyyyMMddHHmmss}";
        var stepRecord = new LotStepRecord
        {
            RecordId = recordId,
            LotId = request.LotId,
            RouteId = route.RouteId,
            RouteVersion = route.RouteVersion,
            StepSeq = targetStep.StepSeq,
            StepCode = targetStep.StepCode,
            StepName = targetStep.StepName,
            Status = "InProgress",
            EquipmentId = request.EquipmentId,
            OperatorId = request.OperatorId,
            TrackInTime = DateTime.UtcNow,
            QtyIn = lot.UnitCount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _stepRecords.TryAdd(recordId, stepRecord);

        // Record quantity transaction
        var qtId = $"QT-{recordId}";
        _quantityTransactions.TryAdd(qtId, new QuantityTransaction
        {
            TransactionId = qtId,
            LotId = request.LotId,
            StepCode = targetStep.StepCode,
            TransactionType = "TrackIn",
            Quantity = lot.UnitCount,
            OperatorId = request.OperatorId,
            TransactionTime = DateTime.UtcNow
        });

        // Update lot
        lot.CurrentEquipment = request.EquipmentId;
        lot.Status = "InProduction";
        lot.CurrentStep = targetStep.StepCode;

        // Audit trail
        var auditId = $"AUD-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}";
        _auditTrails.TryAdd(auditId, new AuditTrail
        {
            AuditId = auditId,
            EntityType = "Lot",
            EntityId = request.LotId,
            Action = "TrackIn",
            NewValue = $"Step: {targetStep.StepCode}, Equipment: {request.EquipmentId}",
            OperatorId = request.OperatorId,
            EquipmentId = request.EquipmentId,
            Timestamp = DateTime.UtcNow
        });

        // Find next step
        var nextStep = route.Steps.FirstOrDefault(s => s.StepSeq == targetStep.StepSeq + 1);

        return new TrackResultDto
        {
            Success = true,
            Message = $"Lot '{request.LotId}' tracked in to step '{targetStep.StepCode}' successfully.",
            LotId = request.LotId,
            StepCode = targetStep.StepCode,
            NextStepCode = nextStep?.StepCode,
            Timestamp = DateTime.UtcNow
        };
    }

    public async Task<TrackValidationResult> ValidateTrackOutAsync(TrackOutRequest request)
    {
        var result = new TrackValidationResult
        {
            LotId = request.LotId,
            EquipmentId = request.EquipmentId,
            IsValid = true
        };

        // Check lot exists
        if (!_lots.TryGetValue(request.LotId, out var lot))
        {
            result.IsValid = false;
            result.Errors.Add($"Lot '{request.LotId}' not found.");
            return result;
        }

        result.LotStatus = lot.Status;

        // Check lot is InProduction
        if (lot.Status != "InProduction")
        {
            result.IsValid = false;
            result.Errors.Add($"Lot status '{lot.Status}' does not allow TrackOut.");
        }

        // Check route exists
        if (!_routes.TryGetValue(lot.RouteId, out var route))
        {
            result.IsValid = false;
            result.Errors.Add($"Route '{lot.RouteId}' not found.");
            return result;
        }

        result.RouteId = route.RouteId;
        result.CurrentStepCode = lot.CurrentStep;

        // Check there is an in-progress step record for this lot + current step
        var activeRecord = _stepRecords.Values.FirstOrDefault(r =>
            r.LotId == request.LotId &&
            r.StepCode == lot.CurrentStep &&
            r.Status == "InProgress");

        if (activeRecord is null)
        {
            result.IsValid = false;
            result.Errors.Add($"No active TrackIn record found for lot '{request.LotId}' at step '{lot.CurrentStep}'.");
        }

        // Validate quantities
        if (request.QtyPass + request.QtyFail <= 0)
        {
            result.IsValid = false;
            result.Errors.Add("Total quantity (Pass + Fail) must be greater than 0.");
        }

        if (activeRecord is not null && request.QtyPass + request.QtyFail > activeRecord.QtyIn)
        {
            result.Warnings.Add("Output quantity exceeds input quantity. Please verify.");
        }

        return result;
    }

    public async Task<TrackResultDto> TrackOutAsync(TrackOutRequest request)
    {
        var validationResult = await ValidateTrackOutAsync(request);
        if (!validationResult.IsValid)
        {
            return new TrackResultDto
            {
                Success = false,
                Message = string.Join("; ", validationResult.Errors),
                LotId = request.LotId
            };
        }

        // Get lot and active step record
        _lots.TryGetValue(request.LotId, out var lot);
        if (lot is null)
        {
            return new TrackResultDto { Success = false, Message = "Lot not found.", LotId = request.LotId };
        }

        var activeRecord = _stepRecords.Values.FirstOrDefault(r =>
            r.LotId == request.LotId &&
            r.StepCode == lot.CurrentStep &&
            r.Status == "InProgress");

        if (activeRecord is null)
        {
            return new TrackResultDto
            {
                Success = false,
                Message = "No active TrackIn record found.",
                LotId = request.LotId
            };
        }

        // Get route for next step
        _routes.TryGetValue(lot.RouteId, out var route);

        // Complete the step record
        activeRecord.Status = "Completed";
        activeRecord.TrackOutTime = DateTime.UtcNow;
        activeRecord.QtyPass = request.QtyPass;
        activeRecord.QtyFail = request.QtyFail;
        activeRecord.QtyOut = request.QtyPass + request.QtyFail;
        activeRecord.TrackOutResult = request.QtyFail > 0 ? "Partial" : "Pass";
        activeRecord.UpdatedAt = DateTime.UtcNow;

        // Record quantity transactions
        var qtPassId = $"QT-{activeRecord.RecordId}-PASS";
        _quantityTransactions.TryAdd(qtPassId, new QuantityTransaction
        {
            TransactionId = qtPassId,
            LotId = request.LotId,
            StepCode = activeRecord.StepCode,
            TransactionType = "TrackOut",
            Quantity = request.QtyPass,
            OperatorId = request.OperatorId,
            TransactionTime = DateTime.UtcNow
        });

        if (request.QtyFail > 0)
        {
            var qtFailId = $"QT-{activeRecord.RecordId}-FAIL";
            _quantityTransactions.TryAdd(qtFailId, new QuantityTransaction
            {
                TransactionId = qtFailId,
                LotId = request.LotId,
                StepCode = activeRecord.StepCode,
                TransactionType = "Scrap",
                Quantity = request.QtyFail,
                ReasonCode = request.FailReason,
                OperatorId = request.OperatorId,
                TransactionTime = DateTime.UtcNow
            });
        }

        // Advance to next step
        var currentStep = route?.Steps.FirstOrDefault(s => s.StepCode == lot.CurrentStep);
        var nextStep = route?.Steps.FirstOrDefault(s => s.StepSeq == currentStep!.StepSeq + 1);

        if (nextStep is not null)
        {
            lot.CurrentStep = nextStep.StepCode;
            lot.UnitCount = request.QtyPass;
            lot.Status = "InProduction";
        }
        else
        {
            // Last step completed
            lot.CurrentStep = activeRecord.StepCode;
            lot.UnitCount = request.QtyPass;
            lot.Status = "Completed";
            lot.QtyPass += request.QtyPass;
            lot.QtyFail += request.QtyFail;
        }

        // Audit trail
        var auditId = $"AUD-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}";
        _auditTrails.TryAdd(auditId, new AuditTrail
        {
            AuditId = auditId,
            EntityType = "Lot",
            EntityId = request.LotId,
            Action = "TrackOut",
            NewValue = $"Step: {activeRecord.StepCode}, Pass: {request.QtyPass}, Fail: {request.QtyFail}",
            OperatorId = request.OperatorId,
            EquipmentId = request.EquipmentId,
            Timestamp = DateTime.UtcNow
        });

        return new TrackResultDto
        {
            Success = true,
            Message = nextStep is not null
                ? $"Lot '{request.LotId}' tracked out from '{activeRecord.StepCode}'. Next step: '{nextStep.StepCode}'."
                : $"Lot '{request.LotId}' completed all steps. Pass: {request.QtyPass}, Fail: {request.QtyFail}.",
            LotId = request.LotId,
            StepCode = activeRecord.StepCode,
            NextStepCode = nextStep?.StepCode,
            Timestamp = DateTime.UtcNow
        };
    }

    public Task<List<LotStepRecord>> GetOperationHistoryAsync(string lotId)
    {
        var records = _stepRecords.Values
            .Where(r => r.LotId == lotId)
            .OrderBy(r => r.StepSeq)
            .ToList();
        return Task.FromResult(records);
    }

    public Task<List<AuditTrail>> GetAuditTrailAsync(string entityType, string entityId)
    {
        var trails = _auditTrails.Values
            .Where(a => a.EntityType == entityType && a.EntityId == entityId)
            .OrderByDescending(a => a.Timestamp)
            .ToList();
        return Task.FromResult(trails);
    }

    public Task<Dictionary<string, object>> GetLotQuantitySummaryAsync(string lotId)
    {
        var transactions = _quantityTransactions.Values
            .Where(t => t.LotId == lotId)
            .ToList();

        var summary = new Dictionary<string, object>
        {
            ["LotId"] = lotId,
            ["TotalTransactions"] = transactions.Count,
            ["TrackInQty"] = transactions.Where(t => t.TransactionType == "TrackIn").Sum(t => t.Quantity),
            ["TrackOutQty"] = transactions.Where(t => t.TransactionType == "TrackOut").Sum(t => t.Quantity),
            ["ScrapQty"] = transactions.Where(t => t.TransactionType == "Scrap").Sum(t => t.Quantity),
        };

        return Task.FromResult(summary);
    }

    public Task SeedAsync()
    {
        // Seed Routes
        var bgaRoute = CreateBgaStandardRoute();
        var qfnRoute = CreateQfnStandardRoute();
        var sopRoute = CreateSopStandardRoute();

        _routes.TryAdd(bgaRoute.RouteId, bgaRoute);
        _routes.TryAdd(qfnRoute.RouteId, qfnRoute);
        _routes.TryAdd(sopRoute.RouteId, sopRoute);

        return Task.CompletedTask;
    }

    private static RouteInfo CreateBgaStandardRoute()
    {
        return new RouteInfo
        {
            RouteId = "BGA-STD",
            RouteName = "BGA Standard Packaging Route",
            RouteVersion = "1.0",
            ProductId = "BGA",
            PackageType = "BGA",
            IsActive = true,
            IsApproved = true,
            ApprovedBy = "Admin",
            ApprovedAt = DateTime.UtcNow,
            Steps =
            [
                new RouteStep { RouteId = "BGA-STD", StepSeq = 10, StepCode = "DIE_BAKE", StepName = "Die Bake", RequireTrackIn = true, RequireTrackOut = true, EquipmentGroup = "Oven" },
                new RouteStep { RouteId = "BGA-STD", StepSeq = 20, StepCode = "WIRE_BOND", StepName = "Wire Bond", RequireTrackIn = true, RequireTrackOut = true, RequireRecipeCheck = true, EquipmentGroup = "Wire Bonder" },
                new RouteStep { RouteId = "BGA-STD", StepSeq = 30, StepCode = "MOLD", StepName = "Mold", RequireTrackIn = true, RequireTrackOut = true, RequireRecipeCheck = true, EquipmentGroup = "Mold Press" },
                new RouteStep { RouteId = "BGA-STD", StepSeq = 40, StepCode = "CURE", StepName = "Cure", RequireTrackIn = true, RequireTrackOut = true, EquipmentGroup = "Oven" },
                new RouteStep { RouteId = "BGA-STD", StepSeq = 50, StepCode = "LASER_MARK", StepName = "Laser Mark", RequireTrackIn = true, RequireTrackOut = true, EquipmentGroup = "Laser Marker" },
                new RouteStep { RouteId = "BGA-STD", StepSeq = 60, StepCode = "PLATE", StepName = "Plating", RequireTrackIn = true, RequireTrackOut = true, EquipmentGroup = "Plating Line" },
                new RouteStep { RouteId = "BGA-STD", StepSeq = 70, StepCode = "SINGULATE", StepName = "Singulation", RequireTrackIn = true, RequireTrackOut = true, EquipmentGroup = "Saw" },
                new RouteStep { RouteId = "BGA-STD", StepSeq = 80, StepCode = "TEST_FT", StepName = "Final Test", RequireTrackIn = true, RequireTrackOut = true, RequireQuantityBalance = true, EquipmentGroup = "Tester" },
                new RouteStep { RouteId = "BGA-STD", StepSeq = 90, StepCode = "PACK", StepName = "Packing", RequireTrackIn = true, RequireTrackOut = true, EquipmentGroup = "Packer" },
            ]
        };
    }

    private static RouteInfo CreateQfnStandardRoute()
    {
        return new RouteInfo
        {
            RouteId = "QFN-STD",
            RouteName = "QFN Standard Packaging Route",
            RouteVersion = "1.0",
            ProductId = "QFN",
            PackageType = "QFN",
            IsActive = true,
            IsApproved = true,
            ApprovedBy = "Admin",
            ApprovedAt = DateTime.UtcNow,
            Steps =
            [
                new RouteStep { RouteId = "QFN-STD", StepSeq = 10, StepCode = "DIE_ATTACH", StepName = "Die Attach", RequireTrackIn = true, RequireTrackOut = true, EquipmentGroup = "Die Bonder" },
                new RouteStep { RouteId = "QFN-STD", StepSeq = 20, StepCode = "CURE_DA", StepName = "Cure Die Attach", RequireTrackIn = true, RequireTrackOut = true, EquipmentGroup = "Oven" },
                new RouteStep { RouteId = "QFN-STD", StepSeq = 30, StepCode = "WIRE_BOND", StepName = "Wire Bond", RequireTrackIn = true, RequireTrackOut = true, RequireRecipeCheck = true, EquipmentGroup = "Wire Bonder" },
                new RouteStep { RouteId = "QFN-STD", StepSeq = 40, StepCode = "MOLD", StepName = "Mold", RequireTrackIn = true, RequireTrackOut = true, EquipmentGroup = "Mold Press" },
                new RouteStep { RouteId = "QFN-STD", StepSeq = 50, StepCode = "CURE", StepName = "Cure", RequireTrackIn = true, RequireTrackOut = true, EquipmentGroup = "Oven" },
                new RouteStep { RouteId = "QFN-STD", StepSeq = 60, StepCode = "SINGULATE", StepName = "Singulation", RequireTrackIn = true, RequireTrackOut = true, EquipmentGroup = "Saw" },
                new RouteStep { RouteId = "QFN-STD", StepSeq = 70, StepCode = "TEST_FT", StepName = "Final Test", RequireTrackIn = true, RequireTrackOut = true, EquipmentGroup = "Tester" },
                new RouteStep { RouteId = "QFN-STD", StepSeq = 80, StepCode = "PACK", StepName = "Packing", RequireTrackIn = true, RequireTrackOut = true, EquipmentGroup = "Packer" },
            ]
        };
    }

    private static RouteInfo CreateSopStandardRoute()
    {
        return new RouteInfo
        {
            RouteId = "SOP-STD",
            RouteName = "SOP Standard Packaging Route",
            RouteVersion = "1.0",
            ProductId = "SOP",
            PackageType = "SOP",
            IsActive = true,
            IsApproved = true,
            ApprovedBy = "Admin",
            ApprovedAt = DateTime.UtcNow,
            Steps =
            [
                new RouteStep { RouteId = "SOP-STD", StepSeq = 10, StepCode = "DIE_ATTACH", StepName = "Die Attach", RequireTrackIn = true, RequireTrackOut = true, EquipmentGroup = "Die Bonder" },
                new RouteStep { RouteId = "SOP-STD", StepSeq = 20, StepCode = "WIRE_BOND", StepName = "Wire Bond", RequireTrackIn = true, RequireTrackOut = true, EquipmentGroup = "Wire Bonder" },
                new RouteStep { RouteId = "SOP-STD", StepSeq = 30, StepCode = "MOLD", StepName = "Mold", RequireTrackIn = true, RequireTrackOut = true, EquipmentGroup = "Mold Press" },
                new RouteStep { RouteId = "SOP-STD", StepSeq = 40, StepCode = "CURE", StepName = "Cure", RequireTrackIn = true, RequireTrackOut = true, EquipmentGroup = "Oven" },
                new RouteStep { RouteId = "SOP-STD", StepSeq = 50, StepCode = "TRIM_FORM", StepName = "Trim & Form", RequireTrackIn = true, RequireTrackOut = true, EquipmentGroup = "Trim Form" },
                new RouteStep { RouteId = "SOP-STD", StepSeq = 60, StepCode = "TEST_FT", StepName = "Final Test", RequireTrackIn = true, RequireTrackOut = true, EquipmentGroup = "Tester" },
                new RouteStep { RouteId = "SOP-STD", StepSeq = 70, StepCode = "PACK", StepName = "Packing", RequireTrackIn = true, RequireTrackOut = true, EquipmentGroup = "Packer" },
            ]
        };
    }
}
