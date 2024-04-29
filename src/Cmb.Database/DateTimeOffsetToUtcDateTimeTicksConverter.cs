using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cmb.Database;

public class DateTimeOffsetToUtcDateTimeTicksConverter : ValueConverter<DateTimeOffset, long>
{
    /// <summary>
    ///     Creates a new instance of this converter.
    /// </summary>
    /// <param name="mappingHints">
    ///     Hints that can be used by the <see cref="ITypeMappingSource" /> to create data types with appropriate
    ///     facets for the converted data.
    /// </param>
    public DateTimeOffsetToUtcDateTimeTicksConverter(ConverterMappingHints? mappingHints = null)
        : base(v => v.UtcDateTime.Ticks,
            v => new DateTimeOffset(v, new TimeSpan(0, 0, 0)),
            mappingHints)
    { }


    public static ValueConverterInfo DefaultInfo { get; } = 
        new(typeof(DateTimeOffset), typeof(long), i => new DateTimeOffsetToUtcDateTimeTicksConverter(i.MappingHints));
}