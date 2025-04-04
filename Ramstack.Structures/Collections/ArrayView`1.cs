namespace Ramstack.Collections;

/// <summary>
/// Represents a read-only view of an array, providing safe and efficient access to a subset of its elements.
/// </summary>
/// <typeparam name="T">The type of the elements in the array view.</typeparam>
[DebuggerDisplay("{ToStringDebugger(),nq}")]
[DebuggerTypeProxy(typeof(ArrayViewDebugView<>))]
[CollectionBuilder(typeof(ArrayView), nameof(ArrayView.Create))]
public readonly struct ArrayView<T> : IReadOnlyList<T>
{
    private readonly T[]? _array;
    private readonly int _index;
    private readonly int _count;

    /// <summary>
    /// Gets the empty array view.
    /// </summary>
    public static ArrayView<T> Empty => default;

    /// <summary>
    /// Gets the number of elements in the current <see cref="ArrayView{T}"/> object.
    /// </summary>
    public int Length => _count;

    /// <summary>
    /// Gets a value indicating whether the <see cref="ArrayView{T}"/> was declared but not initialized.
    /// </summary>
    public bool IsDefault => _array is null;

    /// <inheritdoc cref="IReadOnlyList{T}.this"/>
    public ref readonly T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if ((uint)index >= (uint)_count)
                ThrowHelper.ThrowArgumentOutOfRangeException();

            return ref _array!.GetRawArrayData(_index + index);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayView{T}"/> structure that creates a view
    /// for the all elements in the specified array.
    /// </summary>
    /// <param name="array">The array to wrap.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArrayView(T[] array)
    {
        _index = 0;
        _count = array.Length;
        _array = array;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayView{T}"/> structure that creates
    /// a view for the specified range of the elements in the specified array.
    /// </summary>
    /// <param name="array">The array to wrap.</param>
    /// <param name="index">The zero-based index of the first element in the range.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArrayView(T[] array, int index)
    {
        if ((uint)index > (uint)array.Length)
            ThrowHelper.ThrowArgumentOutOfRangeException();

        _index = index;
        _count = array.Length - index;
        _array = array;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayView{T}"/> structure that creates
    /// a view for the specified range of the elements in the specified array.
    /// </summary>
    /// <param name="array">The array to wrap.</param>
    /// <param name="index">The zero-based index of the first element in the range.</param>
    /// <param name="length">The number of elements in the range.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArrayView(T[] array, int index, int length)
    {
        if (IntPtr.Size == 8)
        {
            if ((ulong)(uint)index + (uint)length > (uint)array.Length)
                ThrowHelper.ThrowArgumentOutOfRangeException();
        }
        else
        {
            if ((uint)index > (uint)array.Length || (uint)length > (uint)(array.Length - index))
                ThrowHelper.ThrowArgumentOutOfRangeException();
        }

        _index = index;
        _count = length;
        _array = array;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayView{T}"/> structure that creates
    /// a view for the specified range of the elements in the specified array.
    /// </summary>
    /// <param name="array">The array to wrap.</param>
    /// <param name="index">The zero-based index of the first element in the range.</param>
    /// <param name="length">The number of elements in the range.</param>
    /// <param name="dummy">The dummy parameter.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ArrayView(T[] array, int index, int length, int dummy)
    {
        _index = index;
        _count = length;
        _array = array;
        _ = dummy;
    }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    public Enumerator GetEnumerator() =>
        new(this);

    /// <summary>
    /// Forms a slice out of the current array view starting at the specified index.
    /// </summary>
    /// <param name="index">The index at which to begin the slice.</param>
    /// <returns>
    /// An array view that consists of all elements of the current array view
    /// from <paramref name="index"/> to the end of the array view.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArrayView<T> Slice(int index)
    {
        if ((uint)index > (uint)_count)
            ThrowHelper.ThrowArgumentOutOfRangeException();

        return new ArrayView<T>(_array!, _index + index, _count - index, dummy: 0);
    }

    /// <summary>
    /// Forms a slice of the specified length out of the current array view starting at the specified index.
    /// </summary>
    /// <param name="index">The index at which to begin the slice.</param>
    /// <param name="count">The length of the slice.</param>
    /// <returns>
    /// An array view of <paramref name="count"/> elements starting at <paramref name="index"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArrayView<T> Slice(int index, int count)
    {
        if (IntPtr.Size == 8)
        {
            if ((ulong)(uint)index + (uint)count > (uint)_count)
                ThrowHelper.ThrowArgumentOutOfRangeException();
        }
        else
        {
            if ((uint)index > (uint)_count || (uint)count > (uint)(_count - index))
                ThrowHelper.ThrowArgumentOutOfRangeException();
        }

        return new ArrayView<T>(_array!, _index + index, count, dummy: 0);
    }

    /// <summary>
    /// Copies the contents of this array view into a new array.
    /// </summary>
    /// <returns>
    /// An array containing the data in the current array view.
    /// </returns>
    public T[] ToArray() =>
        ((ReadOnlySpan<T>)this).ToArray();

    /// <summary>
    /// Returns a read-only span over the current array view.
    /// </summary>
    /// <returns>
    /// The read-only span representation of the array view.
    /// </returns>
    public ReadOnlySpan<T> AsSpan() =>
        this;

    /// <summary>
    /// Returns a read-only a memory region over the current array view.
    /// </summary>
    /// <returns>
    /// The memory representation of the read-only array.
    /// </returns>
    public ReadOnlyMemory<T> AsMemory() =>
        this;

    /// <summary>
    /// Returns a reference to the element of the <see cref="ArrayView{T}"/> at index zero.
    /// </summary>
    /// <returns>
    /// A reference to the element of the <see cref="ArrayView{T}"/> at index zero.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref readonly T GetPinnableReference()
    {
        //
        // Normalize the returned reference.
        // We must not return a reference to an element outside the bounds of our view
        // when the view is empty (_count = 0), even if it points to a valid element
        // in the underlying array. In this case, we return a null reference.
        //
        // Examples:
        //
        // Full array (_array):
        // [0][1][2][3][4][5][6][7][8][9]
        //  |--------------------------|
        //
        // Case 1: Non-empty view (_index = 3, _count = 4)
        //          [3][4][5][6]
        //           ^        ^
        //           |--------| <- valid range for this view
        //
        // Case 2: Empty view (_index = 3, _count = 0)
        //          [3][4][5][6]
        //           ^
        //           | <- no valid reference for this view
        //

        ref var p = ref Unsafe.NullRef<T>();

        if (_count != 0)
            p = ref _array!.GetRawArrayData(_index)!;

        return ref p!;
    }

    /// <summary>
    /// Copies the contents of this <see cref="ArrayView{T}"/> into a destination <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="destination">The span to copy items into.</param>
    public void CopyTo(Span<T> destination) =>
        AsSpan().CopyTo(destination);

    /// <summary>
    /// Attempts to copy the contents of this <see cref="ArrayView{T}"/>
    /// into a destination <see cref="Span{T}"/> and returns a value to indicate
    /// whether the operation succeeded.
    /// </summary>
    /// <param name="destination">The span to copy items into.</param>
    /// <returns>
    /// <see langword="true"/> if the copy operation succeeded; otherwise, <see langword="false"/>.
    /// </returns>
    public bool TryCopyTo(Span<T> destination) =>
        AsSpan().TryCopyTo(destination);

    /// <summary>
    /// Defines an implicit conversion of an array of type <typeparamref name="T"/> to <see cref="ArrayView{T}"/>.
    /// </summary>
    /// <param name="array">The array to convert.</param>
    /// <returns>
    /// An array view representation of the array.
    /// </returns>
    public static implicit operator ArrayView<T>(T[]? array) =>
        new(array ?? []);

    /// <summary>
    /// Defines an implicit conversion of a read-only array of type <typeparamref name="T"/> to an array view.
    /// </summary>
    /// <param name="array">The array to convert.</param>
    /// <returns>
    /// An array view representation of the read-only array.
    /// </returns>
    public static implicit operator ArrayView<T>(ReadOnlyArray<T> array) =>
        array.AsView();

    /// <summary>
    /// Defines an implicit conversion of a read-only array of type <typeparamref name="T"/> to an array view.
    /// </summary>
    /// <param name="array">The array to convert.</param>
    /// <returns>
    /// An array view representation of the read-only array.
    /// </returns>
    public static implicit operator ArrayView<T>(ImmutableArray<T> array)
    {
        #if NET8_0_OR_GREATER
        return ImmutableCollectionsMarshal.AsArray(array);
        #else
        return Unsafe.As<ImmutableArray<T>, ReadOnlyArray<T>>(ref array).AsView();
        #endif
    }

    /// <summary>
    /// Defines an implicit conversion of an array view to the <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    /// <param name="view">The array view to convert.</param>
    /// <returns>
    /// A <see cref="ReadOnlyMemory{T}"/> representation of the array view.
    /// </returns>
    public static implicit operator ReadOnlyMemory<T>(ArrayView<T> view) =>
        new(view._array ?? [], view._index, view._count);

    /// <summary>
    /// Defines an implicit conversion of an array view to the <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="view">The array view to convert.</param>
    /// <returns>
    /// A <see cref="ReadOnlySpan{T}"/> representation of the array view.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<T>(ArrayView<T> view)
    {
        var array = view._array ?? [];
        return MemoryMarshal.CreateReadOnlySpan(
            ref array.GetRawArrayData(view._index),
            length: view._count);
    }

    /// <summary>
    /// Defines an implicit conversion of an array segment to the <see cref="ArrayView{T}"/>.
    /// </summary>
    /// <param name="segment">The array segment to convert.</param>
    /// <returns>
    /// A <see cref="ArrayView{T}"/> representation of the array segment.
    /// </returns>
    public static implicit operator ArrayView<T>(ArraySegment<T> segment) =>
        new(segment.Array!, segment.Offset, segment.Count, dummy: 0);

    /// <summary>
    /// Returns a string representation of the current instance's state,
    /// intended for debugging purposes.
    /// </summary>
    /// <returns>
    /// A string containing information about the current instance.
    /// </returns>
    private string ToStringDebugger() =>
        _array is null ? "Uninitialized" : $"Length = {_count}";

    #region IEnumerable<T> implementation

    IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
        new ArrayViewEnumerator(this);

    IEnumerator IEnumerable.GetEnumerator() =>
        new ArrayViewEnumerator(this);

    #endregion

    #region IReadOnlyCollection<T> implementation

    int IReadOnlyCollection<T>.Count => _count;

    #endregion

    #region IReadOnlyList<T> implementation

    T IReadOnlyList<T>.this[int index] => this[index];

    #endregion

    #region Inner type: Enumerator

    /// <summary>
    /// Provides the ability to iterate through the elements of the <see cref="ArrayView{T}"/>.
    /// </summary>
    public struct Enumerator
    {
        private readonly T[]? _array;
        private int _index;
        private readonly int _final;

        /// <inheritdoc cref="IEnumerator{T}.Current" />
        public readonly ref readonly T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if ((uint)_index >= (uint)_final)
                    ThrowHelper.ThrowArgumentOutOfRangeException();

                return ref _array!.GetRawArrayData(_index);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Enumerator"/> structure.
        /// </summary>
        /// <param name="view">The <see cref="ArrayView{T}"/> to iterate through its elements.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(ArrayView<T> view)
        {
            _array = view._array;
            _index = view._index - 1;
            _final = view._index + view._count;
        }

        /// <inheritdoc cref="IEnumerator.MoveNext" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            ++_index;
            return (uint)_final > (uint)_index;
        }
    }

    #endregion

    #region Inner type: ArrayViewEnumerator

    /// <summary>
    /// Provides the ability to iterate through the elements of the <see cref="ArrayView{T}"/>.
    /// </summary>
    private sealed class ArrayViewEnumerator : IEnumerator<T>
    {
        private readonly T[]? _array;
        private int _index;
        private readonly int _final;

        /// <inheritdoc cref="IEnumerator{T}.Current" />
        public T Current
        {
            get
            {
                if ((uint)_index >= (uint)_final)
                    ThrowHelper.ThrowArgumentOutOfRangeException();

                return _array!.GetRawArrayData(_index);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayViewEnumerator"/> class.
        /// </summary>
        /// <param name="view">The <see cref="ArrayView{T}"/> to iterate through its elements.</param>
        public ArrayViewEnumerator(ArrayView<T> view)
        {
            _array = view._array;
            _index = view._index - 1;
            _final = view._index + view._count;
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            ++_index;
            return (uint)_index < (uint)_final;
        }

        /// <inheritdoc />
        public void Dispose()
        { }

        /// <inheritdoc />
        object? IEnumerator.Current => Current;

        /// <inheritdoc />
        void IEnumerator.Reset() =>
            ThrowHelper.ThrowNotSupportedException();
    }

    #endregion
}
