using System.Collections.Immutable;

namespace Ramstack.Collections;

/// <summary>
/// Provides extension methods for the <see cref="ImmutableArray{T}"/> structure.
/// </summary>
public static class ImmutableArrayExtensions
{
    /// <summary>
    /// Returns a <see cref="ReadOnlyArray{T}"/> over an immutable array.
    /// </summary>
    /// <param name="value">The immutable array.</param>
    /// <returns>
    /// The read-only array over an immutable array.
    /// </returns>
    public static ReadOnlyArray<T> AsReadOnlyArray<T>(this ImmutableArray<T> value) =>
        value;

    /// <summary>
    /// Creates an <see cref="ArrayView{T}"/> over an immutable array.
    /// </summary>
    /// <param name="value">The immutable array to wrap.</param>
    /// <returns>
    /// The read-only view of the array.
    /// </returns>
    public static ArrayView<T> AsView<T>(this ImmutableArray<T> value) =>
        value.AsReadOnlyArray().AsView();

    /// <summary>
    /// Creates an <see cref="ArrayView{T}"/> over an immutable array starting at a specified position to the end of the array.
    /// </summary>
    /// <param name="value">The immutable array to wrap.</param>
    /// <param name="index">The index at which to begin the array view.</param>
    /// <returns>
    /// The read-only view of the array.
    /// </returns>
    public static ArrayView<T> AsView<T>(this ImmutableArray<T> value, int index) =>
        value.AsReadOnlyArray().AsView(index);

    /// <summary>
    /// Creates an <see cref="ArrayView{T}"/> over an immutable array starting at a specified position for a specified length.
    /// </summary>
    /// <param name="value">The immutable array to wrap.</param>
    /// <param name="index">The index at which to begin the array view.</param>
    /// <param name="count">The number of items in the array view.</param>
    /// <returns>
    /// The read-only view of the array.
    /// </returns>
    public static ArrayView<T> AsView<T>(this ImmutableArray<T> value, int index, int count) =>
        value.AsReadOnlyArray().AsView(index, count);
}
