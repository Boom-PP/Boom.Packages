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

    /// <summary>
    /// Creates a new instance of the <see cref="Id"/> class from an existing ID value.
    /// </summary>
    /// <param name="existingId">The existing ID value to create the instance from.</param>
    /// <returns>A new instance of the <see cref="Id"/> class.</returns>
    /// <remarks>NOTE: The existing value is not validated. Use carefully.</remarks>
    public static Id FromExisting(string existingId) => new(existingId);

    public static implicit operator string(Id id) => id.ToString();
    // public static implicit operator Id(string value) => new(value);
    public bool Equals(Id? other) => _value == other?._value;
    public override int GetHashCode() => _value?.GetHashCode() ?? 0;
    public override string ToString() => _value ?? string.Empty;
    
    public static Id NewId() => new(IdGenerator.CreateId().ToBase52());
}

