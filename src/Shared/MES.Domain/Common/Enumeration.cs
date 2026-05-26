namespace MES.Domain.Common;

public abstract class Enumeration(int id, string name) : IEquatable<Enumeration>
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public override string ToString() => Name;
    public override int GetHashCode() => Id.GetHashCode();
    public override bool Equals(object? obj) => obj is Enumeration other && Equals(other);
    public bool Equals(Enumeration? other) => other is not null && Id == other.Id;
}
