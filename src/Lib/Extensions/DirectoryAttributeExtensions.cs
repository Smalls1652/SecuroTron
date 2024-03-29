using System.ComponentModel;
using System.DirectoryServices.Protocols;

namespace SecuroTron.Lib.Extensions;

/// <summary>
/// Extension methods for <see cref="DirectoryAttribute"/>.
/// </summary>
public static class DirectoryAttributeExtensions
{
    /// <summary>
    /// Gets the values of the attribute as an array of the specified type.
    /// </summary>
    /// <typeparam name="T">The type to convert the values to.</typeparam>
    /// <param name="attribute">The <see cref="DirectoryAttribute"/> to get the values from.</param>
    /// <returns>An array of the values from the attribute.</returns>
    public static T[]? GetValues<T>(this DirectoryAttribute attribute)
    {
        if (attribute.Count == 0)
        {
            return null;
        }

        T[] values = new T[attribute.Count];
        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));

        for (int i = 0; i < attribute.Count; i++)
        {
            values[i] = (T)converter.ConvertFrom(attribute[i])!;
        }

        return values;
    }

    /// <summary>
    /// Gets the first value of the attribute as the specified type.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="attribute">The <see cref="DirectoryAttribute"/> to get the value from.</param>
    /// <returns>The first value from the attribute.</returns>
    public static T? GetFirstValue<T>(this DirectoryAttribute attribute)
    {
        if (attribute.Count == 0)
        {
            return default;
        }

        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));

        return (T?)converter.ConvertFrom(attribute[0]);
    }
}
