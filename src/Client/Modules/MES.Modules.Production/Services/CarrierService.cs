using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class CarrierService : ICarrierService
{
    private readonly IRepository<ProdCarrierBinding> _bindingRepo;
    private readonly IRepository<ProdLot> _lotRepo;

    public CarrierService(IRepository<ProdCarrierBinding> bindingRepo, IRepository<ProdLot> lotRepo)
    {
        _bindingRepo = bindingRepo;
        _lotRepo = lotRepo;
    }

    public async Task<LotCarrierBinding> BindCarrierAsync(string lotId, string carrierId,
        string carrierType, string stepCode, int stepSeq, string operatorId)
    {
        await UnbindCarrierAsync(lotId, operatorId);

        var binding = new ProdCarrierBinding
        {
            BindingId = Guid.NewGuid().ToString("N"),
            LotId = lotId,
            CarrierId = carrierId,
            CarrierType = carrierType,
            StepCode = stepCode,
            StepSeq = stepSeq,
            BindTime = DateTime.UtcNow,
            OperatorId = operatorId,
        };

        await _bindingRepo.AddAsync(binding);

        var lot = await _lotRepo.GetByIdAsync(lotId);
        if (lot is not null)
        {
            lot.CarrierId = carrierId;
            lot.CarrierType = carrierType;
            lot.UpdatedAt = DateTime.UtcNow;
            await _lotRepo.UpdateAsync(lot);
        }

        return MapToModel(binding);
    }

    public async Task UnbindCarrierAsync(string lotId, string operatorId)
    {
        var lot = await _lotRepo.GetByIdAsync(lotId);
        if (lot is null || string.IsNullOrEmpty(lot.CarrierId)) return;

        var currentCarrierId = lot.CarrierId;

        lot.CarrierId = string.Empty;
        lot.UpdatedAt = DateTime.UtcNow;
        await _lotRepo.UpdateAsync(lot);

        var bindings = await _bindingRepo.GetWhereAsync(b => b.CarrierId == currentCarrierId && !b.UnbindTime.HasValue);
        foreach (var binding in bindings)
        {
            binding.UnbindTime = DateTime.UtcNow;
            await _bindingRepo.UpdateAsync(binding);
        }
    }

    public async Task<LotCarrierBinding> TransferCarrierAsync(string fromLotId, string toLotId,
        string stepCode, int stepSeq, string operatorId)
    {
        var fromLot = await _lotRepo.GetByIdAsync(fromLotId);
        if (fromLot is null || string.IsNullOrEmpty(fromLot.CarrierId))
            throw new InvalidOperationException($"批次 {fromLotId} 未绑定载具");

        var carrierId = fromLot.CarrierId;
        var carrierType = fromLot.CarrierType ?? string.Empty;

        await UnbindCarrierAsync(fromLotId, operatorId);

        var binding = await BindCarrierAsync(toLotId, carrierId, carrierType, stepCode, stepSeq, operatorId);
        binding.FromCarrierId = carrierId;

        var entity = await _bindingRepo.GetWhereAsync(b => b.BindingId == binding.BindingId);
        var record = entity.FirstOrDefault();
        if (record is not null)
        {
            record.FromCarrierId = carrierId;
            await _bindingRepo.UpdateAsync(record);
        }

        return binding;
    }

    public async Task<List<LotCarrierBinding>> GetCarrierHistoryAsync(string lotId)
    {
        var bindings = await _bindingRepo.GetWhereAsync(b => b.LotId == lotId);
        return bindings.Select(MapToModel).ToList();
    }

    public async Task<LotCarrierBinding?> GetCurrentBindingAsync(string carrierId)
    {
        var bindings = await _bindingRepo.GetWhereAsync(b => b.CarrierId == carrierId && !b.UnbindTime.HasValue);
        return bindings.Select(MapToModel).FirstOrDefault();
    }

    private static LotCarrierBinding MapToModel(ProdCarrierBinding entity) => new()
    {
        BindingId = entity.BindingId,
        LotId = entity.LotId,
        CarrierId = entity.CarrierId,
        CarrierType = entity.CarrierType,
        StepCode = entity.StepCode,
        StepSeq = entity.StepSeq,
        BindTime = entity.BindTime,
        UnbindTime = entity.UnbindTime,
        OperatorId = entity.OperatorId,
        FromCarrierId = entity.FromCarrierId,
    };
}
