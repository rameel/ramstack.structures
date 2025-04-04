namespace Ramstack.Collections;

/// <summary>
/// Provides extension methods for the <see cref="ArrayView{T}"/> structure.
/// </summary>
public static class ArrayViewExtensions
{
    /// <summary>
    /// Creates a new <see cref="ArrayView{T}"/> over an array.
    /// </summary>
    /// <param name="value">The target array.</param>
    /// <returns>
    /// An <see cref="ArrayView{T}"/> representing the array.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayView<T> AsView<T>(this T[]? value) =>
        new(value ?? []);

    /// <summary>
    /// Creates a new <see cref="ArrayView{T}"/> over a portion of the target array from a specified position to the end of the array.
    /// </summary>
    /// <param name="value">The target array.</param>
    /// <param name="index">The index at which to begin the array view.</param>
    /// <returns>
    /// An <see cref="ArrayView{T}"/> representing the array.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayView<T> AsView<T>(this T[]? value, int index) =>
        new(value ?? [], index);

    /// <summary>
    /// Creates a new <see cref="ArrayView{T}"/> over a portion of the target array from a specified position for a specified number of elements.
    /// </summary>
    /// <param name="value">The target array.</param>
    /// <param name="index">The index at which to begin the array view.</param>
    /// <param name="count">The number of items in the array view.</param>
    /// <returns>
    /// An <see cref="ArrayView{T}"/> representing the array.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayView<T> AsView<T>(this T[]? value, int index, int count) =>
        new(value ?? [], index, count);

    /// <summary>
    /// Finds the index of the first occurrence of a specified value in this instance.
    /// </summary>
    /// <param name="view">The <see cref="ArrayView{T}"/> instance.</param>
    /// <param name="value">The value to locate in the array view.</param>
    /// <returns>
    /// The zero-based index of the first occurrence of the value in this instance; otherwise, <c>-1</c>.
    /// </returns>
    public static int IndexOf<T>(this ArrayView<T> view, T value) where T : IEquatable<T> =>
        view.AsSpan().IndexOf(value);

    /// <summary>
    /// Finds the index of the first occurrence of a specified value in the array view.
    /// </summary>
    /// <param name="view">The <see cref="ArrayView{T}"/> instance.</param>
    /// <param name="value">The value to locate in the array view.</param>
    /// <param name="comparer">The equality comparer to use for the search.
    /// If <see langword="null"/>, the default equality comparer is used.</param>
    /// <returns>
    /// The zero-based index of the first occurrence of the value in the array view; otherwise, <c>-1</c>.
    /// </returns>
    public static int IndexOf<T>(this ArrayView<T> view, T value, IEqualityComparer<T>? comparer)
    {
        return Impl(view.AsSpan(), value, comparer);

        static int Impl(ReadOnlySpan<T> span, T value, IEqualityComparer<T>? comparer)
        {
            if (typeof(T).IsValueType && (comparer is null || ReferenceEquals(comparer, EqualityComparer<T>.Default)))
            {
                for (var i = 0; i < span.Length; i++)
                    if (EqualityComparer<T>.Default.Equals(span[i], value))
                        return i;
            }
            else
            {
                comparer ??= EqualityComparer<T>.Default;

                for (var i = 0; i < span.Length; i++)
                    if (comparer.Equals(span[i], value))
                        return i;
            }

            return -1;
        }
    }

    /// <summary>
    /// Finds the index of the last occurrence of a specified value in the array view.
    /// </summary>
    /// <param name="view">The <see cref="ArrayView{T}"/> instance.</param>
    /// <param name="value">The value to locate in the array view.</param>
    /// <returns>
    /// The zero-based index of the last occurrence of the value in the array view; otherwise, <c>-1</c>.
    /// </returns>
    public static int LastIndexOf<T>(this ArrayView<T> view, T value) where T : IEquatable<T> =>
        view.AsSpan().LastIndexOf(value);

    /// <summary>
    /// Finds the index of the last occurrence of a specified value in the array view.
    /// </summary>
    /// <param name="view">The <see cref="ArrayView{T}"/> instance.</param>
    /// <param name="value">The value to locate in the array view.</param>
    /// <param name="comparer">The equality comparer to use for the search.
    /// If <see langword="null"/>, the default equality comparer is used.</param>
    /// <returns>
    /// The zero-based index of the last occurrence of the value in the array view; otherwise, <c>-1</c>.
    /// </returns>
    public static int LastIndexOf<T>(this ArrayView<T> view, T value, IEqualityComparer<T>? comparer)
    {
        return Impl(view.AsSpan(), value, comparer);

        static int Impl(ReadOnlySpan<T> span, T value, IEqualityComparer<T>? comparer)
        {
            if (typeof(T).IsValueType && (comparer is null || ReferenceEquals(comparer, EqualityComparer<T>.Default)))
            {
                for (var i = span.Length - 1; i >= 0; i--)
                    if (EqualityComparer<T>.Default.Equals(span[i], value))
                        return i;
            }
            else
            {
                comparer ??= EqualityComparer<T>.Default;
                for (var i = span.Length - 1; i >= 0; i--)
                    if (comparer.Equals(span[i], value))
                        return i;
            }

            return -1;
        }
    }

    #if NET9_0_OR_GREATER

    /// <summary>
    /// Returns an <see cref="ArrayView{T}"/> over the specified list.
    /// Items should not be added or removed from the <see cref="List{T}"/>
    /// while the <see cref="ArrayView{T}"/> is in use.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the list.</typeparam>
    /// <param name="list">The list to get the data view over.</param>
    /// <returns>
    /// A <see cref="ArrayView{T}"/> instance over the specified list.
    /// </returns>
    public static ArrayView<T> AsView<T>(this List<T>? list)
    {
        if (list is not null)
        {
            var array = ListAccessor<T>.GetArray(list);
            var count = Math.Min(list.Count, array.Length);

            return new ArrayView<T>(array, 0, count);
        }

        return ArrayView<T>.Empty;
    }

    #endif
}
