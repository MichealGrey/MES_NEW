using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class OperationHistoryService : IOperationHistoryService
{
    private readonly IRepository<ProdOperationHistory> _opRepo;

    public OperationHistoryService(IRepository<ProdOperationHistory> opRepo)
    {
        _opRepo = opRepo;
    }

    public async Task WriteAsync(string lotId, string orderId, string operationType, string stepName, int stepSeq,
        string? equipmentId, string? recipeId, string? carrierId,
        int? beforeQty, int? afterQty, string operatorId, string operatorName,
        string workstation, string? remark)
    {
        var entity = new ProdOperationHistory
        {
            OperationId = Guid.NewGuid().ToString("N"),
            LotId = lotId,
            OrderId = orderId,
            OperationType = operationType,
            StepCode = stepName,
            StepSeq = stepSeq,
            EquipmentId = equipmentId,
            CarrierId = carrierId,
            RecipeId = recipeId,
            OperatorId = operatorId,
            OperatorName = operatorName,
            InputQty = beforeQty,
            OutputQty = afterQty,
            Remark = remark,
            CreatedAt = DateTime.UtcNow,
        };

        await _opRepo.AddAsync(entity);
    }

    public async Task<List<OperationRecord>> GetByLotAsync(string lotId)
    {
        var ops = await _opRepo.GetWhereAsync(o => o.LotId == lotId);
        return ops.OrderByDescending(o => o.CreatedAt).Select(MapToModel).ToList();
    }

    private static OperationRecord MapToModel(ProdOperationHistory entity) => new()
    {
        Time = entity.CreatedAt,
        LotId = entity.LotId,
        Operator = $"{entity.OperatorName}({entity.OperatorId})",
        OperationType = entity.OperationType,
        EquipmentId = entity.EquipmentId ?? string.Empty,
        Remark = entity.Remark,
    };
}
