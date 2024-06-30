using JetBrains.Annotations;

namespace Ramstack.Collections;

/// <summary>
/// Provides methods for creating an array that is read-only.
/// </summary>
public static class ReadOnlyArray
{
    /// <summary>
    /// Returns a read-only array that just wraps the <paramref name="array"/>.
    /// </summary>
    /// <typeparam name="T">The type of items stored in the array.</typeparam>
    /// <param name="array">The array of items to get a read-only wrapper for.</param>
    /// <returns>
    /// A read-only array.
    /// </returns>
    public static ReadOnlyArray<T> UnsafeWrap<T>(params T[] array) =>
        new(array);

    /// <summary>
    /// Creates a read-only array from the specified array of items.
    /// </summary>
    /// <typeparam name="T">The type of items stored in the array.</typeparam>
    /// <param name="items">The array of items to populate the array with.</param>
    /// <returns>
    /// A read-only array that contains the array of items.
    /// </returns>
    public static ReadOnlyArray<T> Create<T>(params T[] items)
    {
        _ = items.Length;
        return new ReadOnlyArray<T>(
            new ReadOnlySpan<T>(items));
    }

    /// <summary>
    /// Creates a read-only array with the specified items.
    /// </summary>
    /// <remarks>
    /// This method allocates an array to convert the span into an array.
    /// It is used by the C# compiler to create an <see cref="ReadOnlyArray{T}"/>
    /// from the collection expression introduced in C#12.
    /// </remarks>
    /// <typeparam name="T">The type of items stored in the array.</typeparam>
    /// <param name="items">The items to populate the array with.</param>
    /// <returns>
    /// A read-only array that contains the specified items.
    /// </returns>
    public static ReadOnlyArray<T> Create<T>(ReadOnlySpan<T> items) =>
        new(items);

    /// <summary>
    /// Creates a read-only array with the specified items.
    /// </summary>
    /// <typeparam name="T">The type of items stored in the array.</typeparam>
    /// <param name="items">The items to populate the array with.</param>
    /// <returns>
    /// A read-only array that contains the specified items.
    /// </returns>
    public static ReadOnlyArray<T> Create<T>([InstantHandle] IEnumerable<T> items) =>
        new(items.ToArray());
}
