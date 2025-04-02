using System.Collections.Immutable;

namespace Ramstack.Collections;

/// <summary>
/// Provides extension methods for the <see cref="ReadOnlyArray{T}"/> class.
/// </summary>
public static partial class ReadOnlyArrayExtensions
{
    /// <summary>
    /// Creates a read-only array from the specified read-only span.
    /// </summary>
    /// <typeparam name="T">The type of element in the array.</typeparam>
    /// <param name="items">The read-only span to create a read-only from.</param>
    /// <returns>
    /// A read-only array containing the specified items.
    /// </returns>
    public static ReadOnlyArray<T> ToReadOnlyArray<T>(this Span<T> items) =>
        new(items);

    /// <summary>
    /// Creates a read-only array from the specified read-only span.
    /// </summary>
    /// <typeparam name="T">The type of element in the array.</typeparam>
    /// <param name="items">The read-only span to create a read-only from.</param>
    /// <returns>
    /// A read-only array containing the specified items.
    /// </returns>
    public static ReadOnlyArray<T> ToReadOnlyArray<T>(this ReadOnlySpan<T> items) =>
        new(items);

    /// <summary>
    /// Enumerates a sequence exactly once and produces a read-only array of its contents.
    /// </summary>
    /// <typeparam name="T">The type of element in the sequence.</typeparam>
    /// <param name="items">The array to create a read-only from.</param>
    /// <returns>
    /// A read-only array containing the specified items.
    /// </returns>
    public static ReadOnlyArray<T> ToReadOnlyArray<T>(this T[] items) =>
        ReadOnlyArray.Create(items);

    /// <summary>
    /// Enumerates a sequence exactly once and produces a read-only array of its contents.
    /// </summary>
    /// <typeparam name="T">The type of element in the array.</typeparam>
    /// <param name="items">The sequence to create a read-only from.</param>
    /// <returns>
    /// A read-only array containing the specified items.
    /// </returns>
    [SuppressMessage("ReSharper", "OperatorIsCanBeUsed")]
    public static ReadOnlyArray<T> ToReadOnlyArray<T>([InstantHandle] this IEnumerable<T> items)
    {
        if (items.GetType() == typeof(ReadOnlyArray<T>))
            return (ReadOnlyArray<T>)items;

        if (items.GetType() == typeof(ImmutableArray<T>))
            return (ImmutableArray<T>)items;

        if (items.GetType() == typeof(T[]))
            return ReadOnlyArray.Create(Unsafe.As<IEnumerable<T>, T[]>(ref items));

        return new ReadOnlyArray<T>(items.ToArray());
    }

    /// <summary>
    /// Searches for the specified object and returns the index of its first occurrence.
    /// </summary>
    /// <typeparam name="T">The type of element in the array.</typeparam>
    /// <param name="array">The <see cref="ReadOnlyArray{T}"/> instance.</param>
    /// <param name="value">The object to locate in array.</param>
    /// <returns>
    /// The zero-based index of the first occurrence of <paramref name="value"/> in the <paramref name="array"/>, if found; otherwise, <c>-1</c>.
    /// </returns>
    public static int IndexOf<T>(this ReadOnlyArray<T> array, T value) =>
        Array.IndexOf(array.Inner!, value);

    /// <summary>
    /// Searches for the specified object and returns the index of the last occurrence within the entire <see cref="ReadOnlyArray{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of element in the array.</typeparam>
    /// <param name="array">The <see cref="ReadOnlyArray{T}"/> instance.</param>
    /// <param name="value">The object to locate in array.</param>
    /// <returns>
    /// The zero-based index of the last occurrence of <paramref name="value"/> in the <paramref name="array"/>, if found; otherwise, <c>-1</c>.
    /// </returns>
    public static int LastIndexOf<T>(this ReadOnlyArray<T> array, T value) =>
        Array.LastIndexOf(array.Inner!, value);
}
