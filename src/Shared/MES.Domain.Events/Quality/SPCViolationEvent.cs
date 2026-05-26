using MES.Domain.Common;

namespace MES.Domain.Events.Quality;

public class SPCViolationEvent : DomainEvent
{
    public string EquipmentId { get; set; } = string.Empty;
    public string ParameterName { get; set; } = string.Empty;
    public string Rule { get; set; } = string.Empty;
    public double Value { get; set; }
}
