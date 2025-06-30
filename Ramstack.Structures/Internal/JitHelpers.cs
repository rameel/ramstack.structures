namespace Ramstack.Internal;

/// <summary>
/// Provides JIT helper methods.
/// </summary>
internal static class JitHelpers
{
    /// <summary>
    /// Returns the reference to the first character of the specified string.
    /// </summary>
    /// <param name="text">The string instance.</param>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>
    /// The reference to the first character.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly char GetRawStringData(this string text, int index) =>
        ref Unsafe.Add(ref Unsafe.AsRef(in text.GetPinnableReference()), (nint)(uint)index);

    /// <summary>
    /// Returns the reference to an element at the specified index.
    /// </summary>
    /// <typeparam name="T">The type of array elements.</typeparam>
    /// <param name="array">The array instance.</param>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>
    /// The reference to an element at the specified index.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetRawArrayData<T>(this T[] array, int index)
    {
        // It's valid for a ref to point just past the end of an array, and it'll
        // be properly GC-tracked. (Though dereferencing it may result in undefined behavior)
        return ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), (nint)(uint)index);
    }
}
