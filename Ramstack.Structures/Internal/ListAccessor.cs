namespace Ramstack.Internal;

#if NET9_0_OR_GREATER

/// <summary>
/// Provides low-level access to the internal array buffer of a <see cref="List{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the elements in the list.</typeparam>
internal static class ListAccessor<T>
{
    /// <summary>
    /// Returns a reference to the internal array buffer of the specified <see cref="List{T}"/>.
    /// </summary>
    /// <param name="list">The list whose internal buffer is to be accessed.</param>
    /// <returns>
    /// A reference to the internal array used by the list.
    /// </returns>
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_items")]
    public static extern ref T[] GetArray(List<T> list);

    /// <summary>
    /// Returns a reference to the internal "_size" field of the specified <see cref="List{T}"/>
    /// representing the number of elements in the list.
    /// </summary>
    /// <param name="list">The list whose internal "_size" field is to be accessed.</param>
    /// <returns>
    /// A reference to the internal "_size" field representing the number of elements in the list.
    /// </returns>
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_size")]
    public static extern ref int GetCount(List<T> list);
}

#endif
