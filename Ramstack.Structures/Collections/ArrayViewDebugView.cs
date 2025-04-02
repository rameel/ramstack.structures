namespace Ramstack.Collections;

/// <summary>
/// Represents a debugger view for the <see cref="ArrayView{T}"/> structure, allowing inspection of its contents.
/// </summary>
/// <typeparam name="T">The type of items in the <see cref="ArrayView{T}"/>.</typeparam>
internal sealed class ArrayViewDebugView<T>(ArrayView<T> view)
{
    /// <summary>
    /// Gets the array of items contained in the <see cref="ArrayView{T}"/> for debugging purposes.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Items => view.ToArray();
}
