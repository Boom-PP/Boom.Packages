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