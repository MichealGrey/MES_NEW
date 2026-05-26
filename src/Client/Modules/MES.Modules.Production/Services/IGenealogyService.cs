using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IGenealogyService
{
    Task RecordRelationAsync(LotGenealogy relation);
    Task<List<LotGenealogy>> GetUpstreamAsync(string lotId);
    Task<List<LotGenealogy>> GetDownstreamAsync(string lotId);
    Task<List<LotGenealogy>> GetFullTreeAsync(string lotId);
}
