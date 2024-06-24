using System.Diagnostics.CodeAnalysis;
using IdGen;

namespace Boom.Id;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed record ID
{
    private readonly string? _value;
    internal static IdGenerator? _idGeneratorInstance;
    
    private static IdGenerator _idGenerator {
        get
        {
            if (_idGeneratorInstance is null)
                _idGeneratorInstance = new IdGenerator(Environment.ProcessId % 1024);

            return _idGeneratorInstance;
        }
    }

    private ID(string value) => _value = value;
    
    public bool IsEmpty => string.IsNullOrWhiteSpace(_value);
    
    public static ID Empty => new(string.Empty);
    public static implicit operator string(ID id) => id.ToString();
    // public static implicit operator ID(string value) => new(value);
    public bool Equals(ID? other) => _value == other?._value;
    public override int GetHashCode() => _value?.GetHashCode() ?? 0;
    public override string ToString() => _value ?? string.Empty;
    
    public static ID NewId() => new(_idGenerator.CreateId().ToBase52());
}

