using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class GenealogyService : IGenealogyService
{
    private readonly IRepository<ProdGenealogy> _genealogyRepo;

    public GenealogyService(IRepository<ProdGenealogy> genealogyRepo)
    {
        _genealogyRepo = genealogyRepo;
    }

    public async Task RecordRelationAsync(LotGenealogy relation)
    {
        var entity = new ProdGenealogy
        {
            GenealogyId = relation.GenealogyId,
            ParentLotId = relation.ParentLotId,
            ChildLotId = relation.ChildLotId,
            RelationType = relation.RelationType,
            StepCode = relation.StepCode,
            StepSeq = relation.StepSeq,
            Qty = relation.Qty,
            Grade = relation.Grade,
            WaferLotId = relation.WaferLotId,
            OperatorId = relation.OperatorId,
            ReasonCode = relation.ReasonCode,
            Remark = relation.Remark,
            CreatedAt = relation.CreatedAt,
        };

        await _genealogyRepo.AddAsync(entity);
    }

    public async Task<List<LotGenealogy>> GetUpstreamAsync(string lotId)
    {
        var records = await _genealogyRepo.GetWhereAsync(r => r.ChildLotId == lotId);
        return records.Select(MapToModel).ToList();
    }

    public async Task<List<LotGenealogy>> GetDownstreamAsync(string lotId)
    {
        var records = await _genealogyRepo.GetWhereAsync(r => r.ParentLotId == lotId);
        return records.Select(MapToModel).ToList();
    }

    public async Task<List<LotGenealogy>> GetFullTreeAsync(string lotId)
    {
        var upstream = await GetUpstreamAsync(lotId);
        var downstream = await GetDownstreamAsync(lotId);
        return upstream.Concat(downstream).ToList();
    }

    private static LotGenealogy MapToModel(ProdGenealogy entity) => new()
    {
        GenealogyId = entity.GenealogyId,
        ParentLotId = entity.ParentLotId,
        ChildLotId = entity.ChildLotId,
        RelationType = entity.RelationType,
        StepCode = entity.StepCode,
        StepSeq = entity.StepSeq,
        Qty = entity.Qty,
        Grade = entity.Grade,
        WaferLotId = entity.WaferLotId,
        OperatorId = entity.OperatorId,
        ReasonCode = entity.ReasonCode,
        Remark = entity.Remark,
        CreatedAt = entity.CreatedAt,
    };
}
