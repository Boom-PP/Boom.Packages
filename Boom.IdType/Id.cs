using IdGen;

namespace Boom.IdType;

public sealed record Id
{
    private readonly string? _value;
    internal static IdGenerator? IdGeneratorInstance;
    
    private static IdGenerator IdGenerator {
        get
        {
            if (IdGeneratorInstance is null)
                IdGeneratorInstance = new IdGenerator(Environment.ProcessId % 1024);

            return IdGeneratorInstance;
        }
    }

    internal Id(string value) => _value = value;
    
    public bool IsEmpty => string.IsNullOrWhiteSpace(_value);
    
    public static Id Empty => new(string.Empty);
    public static implicit operator string(Id id) => id.ToString();
    // public static implicit operator ID(string value) => new(value);
    public bool Equals(Id? other) => _value == other?._value;
    public override int GetHashCode() => _value?.GetHashCode() ?? 0;
    public override string ToString() => _value ?? string.Empty;
    
    public static Id NewId() => new(IdGenerator.CreateId().ToBase52());
}

