using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Boom.IdType.EntityFramework;

/// <summary>
/// Converts the <see cref="Id"/> class to a string representation and vice versa
/// for use on entities in Entity Framework Core.
/// </summary>
public class IdConverter() : ValueConverter<Id, string>
(
    id => id.ToString(),
    value => Id.FromExisting(value)
);

file static class IdConverterHelper
{
    public static string? ToDbValue(Id? id) => id is null || id == Id.Empty ? null : id.ToString();
    public static Id? FromDbValue(string dbValue)
    {
        if (string.IsNullOrWhiteSpace(dbValue)) return null;
        
        var id = Id.FromExisting(dbValue);
        if (id == null || id == Id.Empty) return null;

        return id;
    }
}