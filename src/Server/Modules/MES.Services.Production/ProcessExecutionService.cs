using MES.Contracts.Production;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MES.Services.Production;

/// <summary>
/// 工序执行服务实现
/// </summary>
public class ProcessExecutionService : IProcessExecutionService
{
    private readonly MesDbContext _context;
    private readonly ILogger<ProcessExecutionService> _logger;

    public ProcessExecutionService(MesDbContext context, ILogger<ProcessExecutionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 开始工序（记录进站时间、设备、操作员）
    /// </summary>
    public async Task<StepExecutionResponse> StartStepAsync(StepStartRequest request)
    {
        var now = DateTime.UtcNow;
        var operationId = $"OP-{now:yyyyMMddHHmmss}-{request.LotId[^4..]}";

        try
        {
            // 1. 验证批次是否存在且状态允许操作
            var lot = await _context.ProdLots.FindAsync(request.LotId);
            if (lot == null)
            {
                return Fail(operationId, $"批次 {request.LotId} 不存在");
            }

            if (lot.Status == "Completed" || lot.Status == "Scrapped" || lot.Status == "Archived")
            {
                return Fail(operationId, $"批次 {request.LotId} 状态为 {lot.Status}，不允许开始工序");
            }

            if (lot.Status == "Hold")
            {
                return Fail(operationId, $"批次 {request.LotId} 已被Hold，不允许开始工序");
            }

            // 2. 验证工序是否在工艺路线中
            var routeStep = await _context.MasterRouteSteps
                .FirstOrDefaultAsync(x => x.RouteId == lot.RouteId && x.StepCode == request.StepCode);

            if (routeStep == null)
            {
                return Fail(operationId, $"工序 {request.StepCode} 不在工艺路线 {lot.RouteId} 中");
            }

            // 3. 验证工序顺序 - 必须是当前工序或允许的工序
            if (routeStep.StepSeq != lot.CurrentStepSeq)
            {
                return Fail(operationId, $"工序 {request.StepCode} 不是当前工序。当前工序序号: {lot.CurrentStepSeq}，目标工序序号: {routeStep.StepSeq}");
            }

            // 4. 验证设备是否可用
            var equipment = await _context.MasterEquipments.FindAsync(request.EquipmentId);
            if (equipment == null)
            {
                return Fail(operationId, $"设备 {request.EquipmentId} 不存在");
            }

            if (equipment.Status != "Available" && equipment.Status != "Running")
            {
                return Fail(operationId, $"设备 {request.EquipmentId} 状态为 {equipment.Status}，不可用");
            }

            // 5. 验证操作员班次
            var operatorExists = await _context.SysUsers.AnyAsync(x => x.UserId == request.OperatorId && x.IsActive);
            if (!operatorExists)
            {
                return Fail(operationId, $"操作员 {request.OperatorId} 不存在或已停用");
            }

            // 6. 获取或创建工序记录
            var lotStep = await _context.ProdLotSteps
                .FirstOrDefaultAsync(x => x.LotId == request.LotId && x.StepCode == request.StepCode);

            if (lotStep == null)
            {
                lotStep = new ProdLotStep
                {
                    RecordId = $"LS-{now:yyyyMMddHHmmss}-{request.StepCode}",
                    LotId = request.LotId,
                    RouteId = lot.RouteId,
                    RouteVersion = lot.RouteVersion,
                    StepSeq = routeStep.StepSeq,
                    StepCode = request.StepCode,
                    StepName = routeStep.StepName,
                    Status = "InProduction",
                    InputQty = lot.OriginalQty,
                    PendingQty = lot.OriginalQty,
                    TrackInTime = now,
                    TrackInEquipment = request.EquipmentId,
                    TrackInOperator = request.OperatorId,
                    CreatedAt = now
                };
                _context.ProdLotSteps.Add(lotStep);
            }
            else
            {
                if (lotStep.Status == "Completed")
                {
                    return Fail(operationId, $"工序 {request.StepCode} 已完成，不能重新开始");
                }

                lotStep.Status = "InProduction";
                lotStep.TrackInTime = now;
                lotStep.TrackInEquipment = request.EquipmentId;
                lotStep.TrackInOperator = request.OperatorId;
            }

            // 7. 创建操作历史记录
            var operationHistory = new ProdOperationHistory
            {
                OperationId = operationId,
                LotId = request.LotId,
                OrderId = lot.OrderId,
                OperationType = "Start",
                StepCode = request.StepCode,
                StepSeq = routeStep.StepSeq,
                EquipmentId = request.EquipmentId,
                OperatorId = request.OperatorId,
                InputQty = lot.OriginalQty,
                CreatedAt = now
            };

            if (request.Parameters.Count > 0)
            {
                operationHistory.Detail = JsonSerializer.Serialize(request.Parameters);
            }

            _context.ProdOperationHistories.Add(operationHistory);

            // 8. 更新批次当前工序
            lot.CurrentStepCode = request.StepCode;
            lot.CurrentStepSeq = routeStep.StepSeq;
            lot.Status = "InProduction";
            lot.UpdatedAt = now;

            // 9. 保存更改
            await _context.SaveChangesAsync();

            // 10. 获取下一工序信息
            var nextStep = await _context.MasterRouteSteps
                .Where(x => x.RouteId == lot.RouteId && x.StepSeq > routeStep.StepSeq)
                .OrderBy(x => x.StepSeq)
                .FirstOrDefaultAsync();

            _logger.LogInformation(
                "批次 {LotId} 开始工序 {StepCode} (序号: {StepSeq})，设备: {EquipmentId}，操作员: {OperatorId}",
                request.LotId, request.StepCode, routeStep.StepSeq, request.EquipmentId, request.OperatorId);

            return new StepExecutionResponse
            {
                Success = true,
                OperationId = operationId,
                Message = $"工序 {request.StepCode} 已开始",
                CurrentStepCode = request.StepCode,
                CurrentStepSeq = routeStep.StepSeq,
                NextStepCode = nextStep?.StepCode ?? string.Empty,
                NextStepSeq = nextStep?.StepSeq ?? 0,
                IsLastStep = nextStep == null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "开始工序失败: {LotId} - {StepCode}", request.LotId, request.StepCode);
            return Fail(operationId, $"开始工序失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 完成工序（记录出站时间、数量、参数）
    /// </summary>
    public async Task<StepExecutionResponse> CompleteStepAsync(StepCompleteRequest request)
    {
        var now = DateTime.UtcNow;
        var operationId = $"OP-{now:yyyyMMddHHmmss}-{request.LotId[^4..]}";

        try
        {
            // 1. 验证批次是否存在
            var lot = await _context.ProdLots.FindAsync(request.LotId);
            if (lot == null)
            {
                return Fail(operationId, $"批次 {request.LotId} 不存在");
            }

            // 2. 验证工序记录是否存在且状态正确
            var lotStep = await _context.ProdLotSteps
                .FirstOrDefaultAsync(x => x.LotId == request.LotId && x.StepCode == request.StepCode);

            if (lotStep == null)
            {
                return Fail(operationId, $"工序 {request.StepCode} 记录不存在，请先开始工序");
            }

            if (lotStep.Status != "InProduction")
            {
                return Fail(operationId, $"工序 {request.StepCode} 状态为 {lotStep.Status}，不能完成");
            }

            // 3. 验证数量平衡
            var totalOutput = request.PassQty + request.FailQty + request.ScrapQty;
            if (totalOutput != lotStep.InputQty)
            {
                return Fail(operationId,
                    $"数量不平衡: 投入 {lotStep.InputQty}，产出 {totalOutput} (合格: {request.PassQty} + 不合格: {request.FailQty} + 报废: {request.ScrapQty})");
            }

            // 4. 更新工序记录
            lotStep.Status = "Completed";
            lotStep.TrackOutTime = now;
            lotStep.PassQty = request.PassQty;
            lotStep.FailQty = request.FailQty;
            lotStep.ScrapQty = request.ScrapQty;
            lotStep.PendingQty = 0;
            if (!string.IsNullOrEmpty(request.Remarks))
            {
                lotStep.Remark = request.Remarks;
            }

            // 5. 创建操作历史记录
            var operationHistory = new ProdOperationHistory
            {
                OperationId = operationId,
                LotId = request.LotId,
                OrderId = lot.OrderId,
                OperationType = "Complete",
                StepCode = request.StepCode,
                StepSeq = lotStep.StepSeq,
                OperatorId = request.OperatorId,
                InputQty = lotStep.InputQty,
                OutputQty = request.PassQty,
                ScrapQty = request.ScrapQty,
                CreatedAt = now
            };

            _context.ProdOperationHistories.Add(operationHistory);

            // 6. 更新批次累计数量
            lot.TotalPassQty += request.PassQty;
            lot.TotalScrapQty += request.ScrapQty;
            lot.QtyPass += request.PassQty;
            lot.QtyFail += request.FailQty;
            lot.UpdatedAt = now;

            // 7. 移动到下一工序
            var nextStep = await _context.MasterRouteSteps
                .Where(x => x.RouteId == lot.RouteId && x.StepSeq > lotStep.StepSeq)
                .OrderBy(x => x.StepSeq)
                .FirstOrDefaultAsync();

            if (nextStep != null)
            {
                lot.CurrentStepCode = nextStep.StepCode;
                lot.CurrentStepSeq = nextStep.StepSeq;

                // 创建下一工序记录
                var nextLotStep = await _context.ProdLotSteps
                    .FirstOrDefaultAsync(x => x.LotId == request.LotId && x.StepCode == nextStep.StepCode);

                if (nextLotStep == null)
                {
                    nextLotStep = new ProdLotStep
                    {
                        RecordId = $"LS-{now:yyyyMMddHHmmss}-{nextStep.StepCode}",
                        LotId = request.LotId,
                        RouteId = lot.RouteId,
                        RouteVersion = lot.RouteVersion,
                        StepSeq = nextStep.StepSeq,
                        StepCode = nextStep.StepCode,
                        StepName = nextStep.StepName,
                        Status = "Pending",
                        InputQty = request.PassQty,
                        PendingQty = request.PassQty,
                        CreatedAt = now
                    };
                    _context.ProdLotSteps.Add(nextLotStep);
                }

                _logger.LogInformation(
                    "批次 {LotId} 完成工序 {StepCode}，移动到下一工序 {NextStepCode}",
                    request.LotId, request.StepCode, nextStep.StepCode);
            }
            else
            {
                // 最后一道工序完成，批次状态设为Completed
                lot.Status = "Completed";
                lot.CurrentStepCode = request.StepCode;
                lot.CurrentStepSeq = lotStep.StepSeq;

                _logger.LogInformation("批次 {LotId} 完成所有工序", request.LotId);
            }

            // 8. 保存更改
            await _context.SaveChangesAsync();

            return new StepExecutionResponse
            {
                Success = true,
                OperationId = operationId,
                Message = nextStep != null
                    ? $"工序 {request.StepCode} 已完成，下一工序: {nextStep.StepCode}"
                    : $"工序 {request.StepCode} 已完成，批次全部工序完成",
                CurrentStepCode = nextStep?.StepCode ?? request.StepCode,
                CurrentStepSeq = nextStep?.StepSeq ?? lotStep.StepSeq,
                NextStepCode = string.Empty,
                NextStepSeq = 0,
                IsLastStep = nextStep == null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "完成工序失败: {LotId} - {StepCode}", request.LotId, request.StepCode);
            return Fail(operationId, $"完成工序失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 记录工艺参数
    /// </summary>
    public async Task<StepExecutionResponse> RecordParametersAsync(string lotId, string stepCode, List<StepParameterRecord> parameters)
    {
        var now = DateTime.UtcNow;
        var operationId = $"PARAM-{now:yyyyMMddHHmmss}-{lotId[^4..]}";

        try
        {
            // 验证批次是否存在
            var lot = await _context.ProdLots.FindAsync(lotId);
            if (lot == null)
            {
                return Fail(operationId, $"批次 {lotId} 不存在");
            }

            // 验证工序记录是否存在
            var lotStep = await _context.ProdLotSteps
                .FirstOrDefaultAsync(x => x.LotId == lotId && x.StepCode == stepCode);

            if (lotStep == null)
            {
                return Fail(operationId, $"工序 {stepCode} 记录不存在");
            }

            // 创建参数记录操作历史
            var operationHistory = new ProdOperationHistory
            {
                OperationId = operationId,
                LotId = lotId,
                OrderId = lot.OrderId,
                OperationType = "ParameterRecord",
                StepCode = stepCode,
                StepSeq = lotStep.StepSeq,
                Detail = JsonSerializer.Serialize(parameters),
                CreatedAt = now
            };

            _context.ProdOperationHistories.Add(operationHistory);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "批次 {LotId} 工序 {StepCode} 记录 {Count} 个工艺参数",
                lotId, stepCode, parameters.Count);

            return new StepExecutionResponse
            {
                Success = true,
                OperationId = operationId,
                Message = $"已记录 {parameters.Count} 个工艺参数",
                CurrentStepCode = lotStep.StepCode,
                CurrentStepSeq = lotStep.StepSeq
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "记录工艺参数失败: {LotId} - {StepCode}", lotId, stepCode);
            return Fail(operationId, $"记录工艺参数失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 查询工序状态
    /// </summary>
    public async Task<StepStatusResponse> GetStepStatusAsync(string lotId, string stepCode)
    {
        var lotStep = await _context.ProdLotSteps
            .FirstOrDefaultAsync(x => x.LotId == lotId && x.StepCode == stepCode);

        if (lotStep == null)
        {
            // 检查工序是否在工艺路线中
            var lot = await _context.ProdLots.FindAsync(lotId);
            if (lot == null)
            {
                throw new KeyNotFoundException($"批次 {lotId} 不存在");
            }

            var routeStep = await _context.MasterRouteSteps
                .FirstOrDefaultAsync(x => x.RouteId == lot.RouteId && x.StepCode == stepCode);

            if (routeStep == null)
            {
                throw new KeyNotFoundException($"工序 {stepCode} 不在工艺路线中");
            }

            return new StepStatusResponse
            {
                LotId = lotId,
                StepCode = stepCode,
                StepSeq = routeStep.StepSeq,
                Status = "NotStarted"
            };
        }

        return new StepStatusResponse
        {
            LotId = lotStep.LotId,
            StepCode = lotStep.StepCode,
            StepSeq = lotStep.StepSeq,
            Status = lotStep.Status,
            TrackInTime = lotStep.TrackInTime,
            TrackOutTime = lotStep.TrackOutTime,
            EquipmentId = lotStep.TrackInEquipment,
            OperatorId = lotStep.TrackInOperator,
            InputQty = lotStep.InputQty,
            PassQty = lotStep.PassQty,
            FailQty = lotStep.FailQty,
            ScrapQty = lotStep.ScrapQty
        };
    }

    /// <summary>
    /// 获取当前工序
    /// </summary>
    public async Task<StepStatusResponse?> GetCurrentStepAsync(string lotId)
    {
        var lot = await _context.ProdLots.FindAsync(lotId);
        if (lot == null)
        {
            throw new KeyNotFoundException($"批次 {lotId} 不存在");
        }

        if (string.IsNullOrEmpty(lot.CurrentStepCode))
        {
            return null;
        }

        return await GetStepStatusAsync(lotId, lot.CurrentStepCode);
    }

    private static StepExecutionResponse Fail(string operationId, string message)
    {
        return new StepExecutionResponse
        {
            Success = false,
            OperationId = operationId,
            Message = message
        };
    }
}
