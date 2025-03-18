using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ramstack.Collections;

/// <summary>
/// Represents a read-only array wrapper.
/// </summary>
/// <typeparam name="T">The type of element stored by the array.</typeparam>
[DebuggerDisplay("{ToStringDebugger(),nq}")]
[CollectionBuilder(typeof(ReadOnlyArray), nameof(ReadOnlyArray.Create))]
public readonly struct ReadOnlyArray<T> : IReadOnlyList<T>, IEquatable<ReadOnlyArray<T>>
{
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    internal readonly T[]? Inner;

    /// <summary>
    /// Gets an empty read-only array.
    /// </summary>
    public static ReadOnlyArray<T> Empty => new(Array.Empty<T>());

    /// <summary>
    /// Gets the number of elements contained in the <see cref="ReadOnlyArray{T}"/>.
    /// </summary>
    public int Length => Inner!.Length;

    /// <summary>
    /// Gets a value indicating whether this <see cref="ReadOnlyArray{T}"/> was declared but not initialized.
    /// </summary>
    public bool IsDefault
    {
        [MemberNotNullWhen(false, nameof(Inner))]
        get => Inner is null;
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="ReadOnlyArray{T}"/> is empty or is not initialized.
    /// </summary>
    public bool IsDefaultOrEmpty
    {
        [MemberNotNullWhen(false, nameof(Inner))]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Inner is var inner && (inner is null || inner.Length == 0);
    }

    /// <summary>
    /// Gets an element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get. </param>
    public ref readonly T this[int index] => ref Inner![index];

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyArray{T}"/> structure.
    /// </summary>
    /// <param name="array">The underlying array.</param>
    internal ReadOnlyArray(T[] array) =>
        Inner = array;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyArray{T}"/> structure with the specified object.
    /// </summary>
    /// <param name="item">The object to store in the array.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyArray(T item) =>
        Inner = [item];

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyArray{T}"/> structure with the specified objects.
    /// </summary>
    /// <param name="item1">The object to store in the array.</param>
    /// <param name="item2">The object to store in the array.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyArray(T item1, T item2) =>
        Inner = [item1, item2];

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyArray{T}"/> structure with the specified objects.
    /// </summary>
    /// <param name="item1">The object to store in the array.</param>
    /// <param name="item2">The object to store in the array.</param>
    /// <param name="item3">The object to store in the array.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyArray(T item1, T item2, T item3) =>
        Inner = [item1, item2, item3];

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyArray{T}"/> structure with the specified objects.
    /// </summary>
    /// <param name="item1">The object to store in the array.</param>
    /// <param name="item2">The object to store in the array.</param>
    /// <param name="item3">The object to store in the array.</param>
    /// <param name="item4">The object to store in the array.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyArray(T item1, T item2, T item3, T item4) =>
        Inner = [item1, item2, item3, item4];

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyArray{T}"/> structure with the specified objects.
    /// </summary>
    /// <param name="items">The objects to populate the array with.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyArray(ReadOnlySpan<T> items)
    {
        // Avoid address exposure in cases where the destination local does not actually end up escaping in any way.
        // https://github.com/dotnet/runtime/pull/102808

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (items.Length != 0)
            Inner = items.ToArray();
        else
            Inner = [];
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// An enumerator that can be used to iterate through the collection.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator()
    {
        var array = Inner;
        _ = array!.Length;

        return new Enumerator(array);
    }

    /// <summary>
    /// Forms a slice out of the current array that begins at a specified index.
    /// </summary>
    /// <param name="start">The index at which to begin the slice.</param>
    /// <returns>
    /// A readonly span that consists of all elements of the current array from <paramref name="start"/> to the end.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyArray<T> Slice(int start)
    {
        var array = Inner;
        _ = array!.Length;

        return start != 0
            ? new ReadOnlyArray<T>(array.AsSpan(start))
            : this;
    }

    /// <summary>
    /// Forms a slice out of the current array that begins at a specified index for a specified length.
    /// </summary>
    /// <param name="start">The index at which to begin the slice.</param>
    /// <param name="length">The desired length for the slice.</param>
    /// <returns>
    /// A readonly span that consists of <paramref name="length"/> elements from the current array starting at <paramref name="start"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyArray<T> Slice(int start, int length)
    {
        var array = Inner;
        var count = array!.Length;

        if (start == 0 && length == count)
            return this;

        return new ReadOnlyArray<T>(array.AsSpan(start, length));
    }

    /// <inheritdoc />
    public bool Equals(ReadOnlyArray<T> other) =>
        ReferenceEquals(Inner, other.Inner);

    /// <inheritdoc />
    public override bool Equals(object? obj) =>
        obj is ReadOnlyArray<T> array && Equals(array);

    /// <inheritdoc />
    public override int GetHashCode() =>
        Inner?.GetHashCode() ?? 0;

    /// <summary>
    /// Creates a read-only span over the current array.
    /// </summary>
    /// <returns>
    /// The read-only span representation of the array.
    /// </returns>
    public ReadOnlySpan<T> AsSpan() =>
        new(Inner ?? []);

    /// <summary>
    /// Creates a new read-only span over a portion of the current array starting at a specified position to the end of the array.
    /// </summary>
    /// <param name="start">The index at which to begin the span.</param>
    /// <returns>
    /// The read-only span representation of the array.
    /// </returns>
    public ReadOnlySpan<T> AsSpan(int start) =>
        // ReSharper disable once ReplaceSliceWithRangeIndexer
        AsSpan().Slice(start);

    /// <summary>
    /// Creates a new read-only span over a portion of the current array starting at a specified position for a specified length.
    /// </summary>
    /// <param name="start">The index at which to begin the span.</param>
    /// <param name="length">The number of items in the span.</param>
    /// <returns>
    /// The read-only span representation of the array.
    /// </returns>
    public ReadOnlySpan<T> AsSpan(int start, int length) =>
        AsSpan().Slice(start, length);

    /// <summary>
    /// Creates an <see cref="ArrayView{T}"/> over the current array.
    /// </summary>
    /// <returns>
    /// The read-only view of the array.
    /// </returns>
    public ArrayView<T> AsView() =>
        new(Inner ?? []);

    /// <summary>
    /// Creates an <see cref="ArrayView{T}"/> over the current array starting at a specified position to the end of the array.
    /// </summary>
    /// <param name="index">The index at which to begin the array view.</param>
    /// <returns>
    /// The read-only view of the array.
    /// </returns>
    public ArrayView<T> AsView(int index) =>
        // ReSharper disable once ReplaceSliceWithRangeIndexer
        AsView().Slice(index);

    /// <summary>
    /// Creates an <see cref="ArrayView{T}"/> over the current array starting at a specified position for a specified length.
    /// </summary>
    /// <param name="index">The index at which to begin the array view.</param>
    /// <param name="count">The number of items in the array view.</param>
    /// <returns>
    /// The read-only view of the array.
    /// </returns>
    public ArrayView<T> AsView(int index, int count) =>
        AsView().Slice(index, count);

    /// <summary>
    /// Creates a new read-only memory region over the read-only array.
    /// </summary>
    /// <returns>
    /// The memory representation of the array.
    /// </returns>
    public ReadOnlyMemory<T> AsMemory() =>
        this;

    /// <summary>
    /// Returns a reference to the element of the <see cref="ReadOnlyArray{T}"/> at index zero.
    /// </summary>
    /// <returns>
    /// A reference to the element of the <see cref="ReadOnlyArray{T}"/> at index zero, or <see langword="null"/> if <see cref="IsDefault"/> is true.
    /// </returns>
    public ref readonly T GetPinnableReference() =>
        ref MemoryMarshal.GetArrayDataReference(Inner!);

    /// <summary>
    /// Copies the contents of this <see cref="ArrayView{T}"/> into a destination <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="destination">The span to copy items into.</param>
    public void CopyTo(Span<T> destination)
    {
        var array = Inner;
        _ = array!.Length;

        array.AsSpan().CopyTo(destination);
    }

    /// <summary>
    /// Attempts to copy the contents of this <see cref="ArrayView{T}"/> into a destination <see cref="Span{T}"/>
    /// and returns a value to indicate whether the operation succeeded.
    /// </summary>
    /// <param name="destination">The span to copy items into.</param>
    /// <returns>
    /// <see langword="true"/> if the copy operation succeeded; otherwise, <see langword="false"/>.
    /// </returns>
    public bool TryCopyTo(Span<T> destination)
    {
        var array = Inner;
        _ = array!.Length;

        return array.AsSpan().TryCopyTo(destination);
    }

    /// <summary>
    /// Returns a read-only array from the <paramref name="array"/>.
    /// </summary>
    /// <param name="array">The array of objects to get a read-only wrapper for.</param>
    /// <returns>
    /// A read-only array.
    /// </returns>
    public static explicit operator ReadOnlyArray<T>(T[]? array) =>
        new(array!);

    /// <summary>
    /// Returns a read-only array from the <paramref name="array"/>.
    /// </summary>
    /// <param name="array">The array of objects to get a read-only wrapper for.</param>
    /// <returns>
    /// A read-only array.
    /// </returns>
    public static implicit operator ReadOnlyArray<T>(ImmutableArray<T> array) =>
        // https://github.com/dotnet/runtime/issues/83141#issuecomment-1460324087
        // Unsafe.As<ImmutableArray<T>, ReadOnlyArray<T>>(ref array);
        new(ImmutableCollectionsMarshal.AsArray(array)!);

    /// <summary>
    /// Returns a <see cref="ImmutableArray{T}"/> array from the <paramref name="array"/>.
    /// </summary>
    /// <param name="array">The array of objects to get a read-only wrapper for.</param>
    /// <returns>
    /// A read-only array.
    /// </returns>
    public static implicit operator ImmutableArray<T>(ReadOnlyArray<T> array) =>
        // https://github.com/dotnet/runtime/issues/83141#issuecomment-1460324087
        // Unsafe.As<ReadOnlyArray<T>, ImmutableArray<T>>(ref array);
        ImmutableCollectionsMarshal.AsImmutableArray(array.Inner);

    /// <summary>
    /// Returns a <see cref="ReadOnlySpan{T}"/> from the <paramref name="array"/>.
    /// </summary>
    /// <param name="array">The <see cref="ReadOnlyArray{T}"/>.</param>
    /// <returns>
    /// A <see cref="ReadOnlySpan{T}"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<T>(ReadOnlyArray<T> array) =>
        new(array.Inner ?? []);

    /// <summary>
    /// Returns a <see cref="ReadOnlyMemory{T}"/> from the <paramref name="array"/>.
    /// </summary>
    /// <param name="array">The <see cref="ReadOnlyArray{T}"/>.</param>
    /// <returns>
    /// A <see cref="ReadOnlyMemory{T}"/>.
    /// </returns>
    public static implicit operator ReadOnlyMemory<T>(ReadOnlyArray<T> array) =>
        new(array.Inner ?? []);

    /// <summary>
    /// Returns a value that indicates if two arrays are equal.
    /// </summary>
    /// <param name="left">The array to the left of the operator.</param>
    /// <param name="right">The array to the right of the operator.</param>
    /// <returns>
    /// <c>true</c> if the arrays are equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(ReadOnlyArray<T> left, ReadOnlyArray<T> right) =>
        left.Inner == right.Inner;

    /// <summary>
    /// Returns a value that indicates if two arrays are not equal.
    /// </summary>
    /// <param name="left">The array to the left of the operator.</param>
    /// <param name="right">The array to the right of the operator.</param>
    /// <returns>
    /// <c>true</c> if the arrays are not equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(ReadOnlyArray<T> left, ReadOnlyArray<T> right) =>
        left.Inner != right.Inner;

    /// <summary>
    /// Returns a value that indicates if two arrays are equal.
    /// </summary>
    /// <param name="left">The array to the left of the operator.</param>
    /// <param name="right">The array to the right of the operator.</param>
    /// <returns>
    /// <c>true</c> if the arrays are equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(ReadOnlyArray<T>? left, ReadOnlyArray<T>? right) =>
        left.GetValueOrDefault() == right.GetValueOrDefault();

    /// <summary>
    /// Returns a value that indicates if two arrays are not equal.
    /// </summary>
    /// <param name="left">The array to the left of the operator.</param>
    /// <param name="right">The array to the right of the operator.</param>
    /// <returns>
    /// <c>true</c> if the arrays are not equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(ReadOnlyArray<T>? left, ReadOnlyArray<T>? right) =>
        left.GetValueOrDefault() != right.GetValueOrDefault();

    /// <summary>
    /// Returns a string representation of the current instance's state, intended for debugging purposes.
    /// </summary>
    /// <returns>
    /// A string containing information about the current instance.
    /// </returns>
    private string ToStringDebugger() =>
        Inner is { } array ? $"Length = {array.Length}" : "Uninitialized";

    [MemberNotNull(nameof(Inner))]
    private void EnsureInitialized()
    {
        if (Inner is null)
        {
            const string? message =
                "This operation cannot be performed on a default instance of ReadOnlyArray<T>. " +
                "Consider initializing the array, or checking the ReadOnlyArray<T>.IsDefault property.";
            throw new InvalidOperationException(message);
        }
    }

    #region IEnumerable<T> implementation

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        EnsureInitialized();

        return Inner.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        EnsureInitialized();

        return Inner.AsEnumerable().GetEnumerator();
    }

    #endregion

    #region IReadOnlyCollection<T> implementation

    int IReadOnlyCollection<T>.Count
    {
        get
        {
            EnsureInitialized();
            return Inner.Length;
        }
    }

    #endregion

    #region IReadOnlyList<T> implementation

    T IReadOnlyList<T>.this[int index]
    {
        get
        {
            EnsureInitialized();
            return Inner![index];
        }
    }

    #endregion

    #region Inner type: Enumerator

    /// <summary>
    /// Represents the <seealso cref="ReadOnlyArray{T}"/> enumerator.
    /// </summary>
    public struct Enumerator
    {
        private readonly T[] _array;
        private int _index;

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        public readonly ref readonly T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _array[_index];
        }

        /// <summary>
        /// Initializes a new instance of the <seealso cref="ReadOnlyArray{T}"/> structure.
        /// </summary>
        /// <param name="array">The array to iterate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(T[] array)
        {
            _array = array;
            _index = -1;
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the enumerator was successfully advanced to the next element; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() =>
            ++_index < _array.Length;
    }

    #endregion
}
