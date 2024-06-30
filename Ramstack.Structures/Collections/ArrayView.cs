namespace Ramstack.Collections;

/// <summary>
/// Provides methods for creating array views.
/// </summary>
public static class ArrayView
{
    /// <summary>
    /// Creates an array view for the specified array.
    /// </summary>
    /// <typeparam name="T">The type of items stored in the array.</typeparam>
    /// <param name="items">The array of items for which an array view will be created.</param>
    /// <returns>
    /// An array view for the specified array.
    /// </returns>
    public static ArrayView<T> Create<T>(params T[] items) =>
        new(items);

    /// <summary>
    /// Creates an array view for the specified read-only span.
    /// </summary>
    /// <remarks>
    /// This method allocates an array to convert the span into an array.
    /// It is used by the C# compiler to create an <see cref="ArrayView{T}"/>
    /// from the collection expression introduced in C#12.
    /// </remarks>
    /// <typeparam name="T">The type of items stored in the span.</typeparam>
    /// <param name="items">The array of items for which an array view will be created.</param>
    /// <returns>
    /// An <see cref="ArrayView{T}"/> instance for the specified read-only span.
    /// </returns>
    public static ArrayView<T> Create<T>(ReadOnlySpan<T> items) =>
        new(items.ToArray());
}
