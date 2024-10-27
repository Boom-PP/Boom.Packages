using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

using IdGen;

using NJsonSchema.Annotations;


namespace Boom.IdType;

/// <summary>
/// Represents an identifier based on the IdGen library. Ids are time-ordered
/// and are suitable for use as a database key. They are also case-insensitive, and formatted to minimize
/// confusion between different characters and digits in case it has to be read by humans.<br />
/// Usages:
/// <code>var varId = Id.NewId();</code>
/// <code>Id id = Id.NewId();</code>
/// <code>string stringId = Id.NewId();</code>
/// <code>string stringId = id;</code>
/// <code>Id id = stringId; // Will fail. No implicit operator from string to Id</code>
/// <code>Id id = Id.FromExisting(stringId);</code>
/// </summary>
/// <remarks>
/// The epoch for this ID generator is Aug 30, 2024. It will keep to the default formatting of xxxx-xxxx-xxxx
/// for the next 80 years or so, when it will overflow with an extra character in the last group.
/// Consider refactoring this package around the summer of 2104.
/// </remarks>
[JsonConverter(typeof(IdJsonConverter))]
[JsonSchemaType(typeof(string))] // Temp workaround to support Cratis Chronicle. Will be removed later.
public sealed record Id : IComparable<Id>
{
    private static readonly DateTime DefaultEpoch = new(2024, 8, 30, 0, 0, 0, DateTimeKind.Utc);

    private readonly string?      _value;
    internal static  IdGenerator? IdGeneratorInstance;

    private static IdGenerator IdGenerator
    {
        get
        {
            if (IdGeneratorInstance is null)
                IdGeneratorInstance = new IdGenerator(Environment.ProcessId % 1024);

            return IdGeneratorInstance;
        }
    }

    internal Id(string value, [CallerMemberName] string? caller = null)
    {
        if (string.IsNullOrWhiteSpace(value) && caller != "Empty")
            throw new ArgumentException("Id must have a value");

        if (value.Length < 9 && caller != "Empty")
            throw new ArgumentException("An Id must be at least 9 characters", nameof(value));

        _value = value.Replace("-", "");
    }

    public bool IsEmpty => string.IsNullOrWhiteSpace(_value);

    public static Id Empty => new(string.Empty);

    /// <summary>
    /// Creates a new instance of the <see cref="Id"/> class from an existing ID value.
    /// </summary>
    /// <param name="existingId">The existing ID value to create the instance from.</param>
    /// <returns>A new instance of the <see cref="Id"/> class.</returns>
    /// <remarks>NOTE: The existing value is not validated. Use carefully.</remarks>
    public static Id FromExisting(string existingId) => new(existingId);

    /// <summary>
    /// Will create an ID from a string if the input string is valid
    /// </summary>
    /// <param name="value"></param>
    /// <param name="id"></param>
    /// <returns>True if the value was successfully parsed</returns>
    /// <remarks>Was mainly added to support FastEndpoints request binding: https://fast-endpoints.com/docs/model-binding#supported-dto-property-types</remarks>
    public static bool TryParse(string? value, out Id? id)
    {
        id = null;
        if (value is null)
            return false;

        id = FromExisting(value);
        return true;
    }

    public static implicit operator string(Id? id) => id is null ? string.Empty : id.ToString();

    // public static implicit operator Id(string value) => new(value);
    public          bool   Equals(Id? other) => _value == other?._value;
    public override int    GetHashCode()     => _value?.GetHashCode() ?? 0;
    public override string ToString()        => AwesomeIdFormat(_value) ?? string.Empty;

    public static Id NewId() => new(IdGenerator.CreateId().ToBase());

    private static string? AwesomeIdFormat(string? raw)
    {
        return string.IsNullOrWhiteSpace(raw) ? null : $"{raw[..4]}-{raw[4..8]}-{raw[8..]}";
    }

    public int CompareTo(Id? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        return other is null 
                   ? 1 
                   : string.Compare(_value, other._value, StringComparison.OrdinalIgnoreCase);
    }
}


public class IdJsonConverter : JsonConverter<Id>
{
    public override Id? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => Id.FromExisting(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, Id value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());
}
