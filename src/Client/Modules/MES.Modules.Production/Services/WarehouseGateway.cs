using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class WarehouseGateway : IWarehouseGateway
{
    private readonly IRepository<MasterMaterial> _materialRepo;
    private readonly IRepository<MaterialConsumeEntity> _consumeRepo;
    private readonly IRepository<MaterialRequirement> _requirementRepo;

    public WarehouseGateway(
        IRepository<MasterMaterial> materialRepo,
        IRepository<MaterialConsumeEntity> consumeRepo,
        IRepository<MaterialRequirement> requirementRepo)
    {
        _materialRepo = materialRepo;
        _consumeRepo = consumeRepo;
        _requirementRepo = requirementRepo;
    }

    public async Task<bool> IsMaterialReadyAsync(string lotId, string stepCode)
    {
        var requiredMaterials = await GetRequiredMaterialsAsync(stepCode);
        if (requiredMaterials.Count == 0) return true;

        foreach (var materialId in requiredMaterials)
        {
            var material = await _materialRepo.GetByIdAsync(materialId);
            if (material is null) return false;
        }

        return true;
    }

    public async Task RecordMaterialConsumeAsync(MaterialConsume consume)
    {
        var entity = new MaterialConsumeEntity
        {
            ConsumeId = consume.ConsumeId,
            LotId = consume.LotId,
            StepCode = consume.StepCode,
            MaterialId = consume.MaterialId,
            MaterialName = consume.MaterialName,
            BatchNo = consume.BatchNo,
            ConsumedQty = (decimal)consume.ConsumedQty,
            Unit = consume.Unit,
            OperatorId = consume.OperatorId,
            ConsumedAt = consume.ConsumedAt,
        };
        await _consumeRepo.AddAsync(entity);
    }

    public async Task<List<string>> GetRequiredMaterialsAsync(string stepCode)
    {
        var requirements = await _requirementRepo.GetWhereAsync(r => r.StepCode == stepCode);
        return requirements.Select(r => r.MaterialId).ToList();
    }
}
