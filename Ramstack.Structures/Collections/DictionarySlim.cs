namespace Ramstack.Collections;

/// <summary>
/// Represents a lightweight alternative to a <see cref="Dictionary{TKey, TValue}"/>,
/// providing a collection of keys and values with optimized memory usage.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
public partial class DictionarySlim<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IDictionary where TKey : notnull
{
    private readonly IEqualityComparer<TKey>? _comparer;
    private Entry[] _entries;
    private int _count;
    private int _freeIndex;
    private int _freeCount;

    /// <inheritdoc cref="ICollection{T}.Count" />
    public int Count => _count - _freeCount;

    /// <summary>
    /// Gets the total numbers of elements the internal data structure can hold without resizing.
    /// </summary>
    public int Capacity => _entries.Length;

    /// <summary>
    /// Gets the <see cref="IEqualityComparer{T}"/> that is used to determine equality of keys for the dictionary.
    /// </summary>
    public IEqualityComparer<TKey> Comparer => _comparer ?? EqualityComparer<TKey>.Default;

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    /// <param name="key">
    /// The key of the value to get or set.
    /// </param>
    public TValue this[TKey key]
    {
        get
        {
            ref var entry = ref FindEntry(key);
            if (Unsafe.IsNullRef(ref entry))
                ThrowHelper.ThrowKeyNotFoundException(key);

            return entry.Data.Value;
        }
        set => GetValueRefOrAddDefault(key, out _) = value;
    }

    /// <summary>
    /// Gets a collection containing the keys in the <see cref="DictionarySlim{TKey,TValue}"/>.
    /// </summary>
    public KeyCollection Keys => new(this);

    /// <summary>
    /// Gets a collection containing the values in the <see cref="DictionarySlim{TKey,TValue}"/>.
    /// </summary>
    public ValueCollection Values => new(this);

    /// <summary>
    /// Initializes a new instance of the <see cref="DictionarySlim{TKey,TValue}"/> class that is empty,
    /// has the default initial capacity, and uses the default equality comparer for the key type.
    /// </summary>
    public DictionarySlim()
    {
        _entries = [];

        //
        // For reference types:
        // We should always keep a comparer instance stored, either the one supplied,
        // or if none was supplied, the default one (repeatedly accessing
        // EqualityComparer<TKey>.Default with shared generics for each dictionary
        // lookup can introduce noticeable performance costs).
        //
        if (!typeof(TKey).IsValueType)
            _comparer = EqualityComparer<TKey>.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DictionarySlim{TKey,TValue}"/> class that is empty,
    /// has the default initial capacity, and uses the specified <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys,
    /// or <see langword="null"/> to use the default <see cref="EqualityComparer{T}"/> for the type of the key.</param>
    public DictionarySlim(IEqualityComparer<TKey>? comparer)
    {
        _entries = [];

        //
        // For value types:
        // When no comparer is specified, or if the default is used,
        // it’s better to rely on EqualityComparer<TKey>.Default.Equals
        // for each comparison, allowing the JIT to devirtualize
        // and potentially inline the operation.
        //
        // For reference types:
        // We should always keep a comparer instance stored, either the one supplied,
        // or if none was supplied, the default one (repeatedly accessing
        // EqualityComparer<TKey>.Default with shared generics for each dictionary
        // lookup can introduce noticeable performance costs).
        //

        if (!typeof(TKey).IsValueType)
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
        else if (comparer is not null && (object?)comparer != EqualityComparer<TKey>.Default)
            _comparer = comparer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DictionarySlim{TKey,TValue}"/> class that is empty,
    /// has the specified initial capacity, and uses the specified <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <param name="capacity">The initial number of elements that the <see cref="DictionarySlim{TKey,TValue}"/> can contain.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys,
    /// or <see langword="null"/> to use the default <see cref="EqualityComparer{T}"/> for the type of the key.</param>
    public DictionarySlim(int capacity, IEqualityComparer<TKey>? comparer = null)
    {
        if (capacity < 0)
            ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegative(ExceptionArgument.capacity);

        _entries = capacity > 0
            ? new Entry[HashHelper.RoundUpToPowerOf2(capacity)]
            : [];

        if (!typeof(TKey).IsValueType)
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
        else if (comparer is not null && (object?)comparer != EqualityComparer<TKey>.Default)
            _comparer = comparer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DictionarySlim{TKey,TValue}"/> class that contains elements
    /// copied from the specified <see cref="IDictionary{TKey,TValue}"/> and uses the specified <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <param name="dictionary">The <see cref="IDictionary{TKey,TValue}"/> whose elements are copied
    /// to the new <see cref="DictionarySlim{TKey,TValue}"/>.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys,
    /// or <see langword="null"/> to use the default <see cref="EqualityComparer{T}"/> for the type of the key.</param>
    public DictionarySlim(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer = null)
        : this(dictionary.Count, comparer)
    {
        AddRange(dictionary);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DictionarySlim{TKey,TValue}"/> class that contains elements
    /// copied from the specified <see cref="IEnumerable{T}"/> and uses the specified <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <param name="collection">The <see cref="IEnumerable{T}"/> whose elements are copied
    /// to the new <see cref="DictionarySlim{TKey,TValue}"/>.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys,
    /// or <see langword="null"/> to use the default <see cref="EqualityComparer{T}"/> for the type of the key.</param>
    public DictionarySlim(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer = null)
        : this((collection as ICollection<TKey>)?.Count ?? 0, comparer)
    {
        AddRange(collection);
    }

    /// <inheritdoc cref="IDictionary{TKey,TValue}.ContainsKey" />
    public bool ContainsKey(TKey key) =>
        !Unsafe.IsNullRef(ref FindEntry(key));

    /// <summary>
    /// Determines whether the <see cref="DictionarySlim{TKey,TValue}"/> contains a specific value.
    /// </summary>
    /// <param name="value">The value to locate in the <see cref="DictionarySlim{TKey,TValue}"/>.
    /// The value can be <see langword="null"/> for reference types.</param>
    /// <returns>
    /// <see lanword="true"/> if the <see cref="DictionarySlim{TKey,TValue}"/> contains an element
    /// with the specified value; otherwise, <see lanword="false"/>.
    /// </returns>
    public bool ContainsValue(TValue value)
    {
        var entries = _entries.AsSpan(0, _count);

        if (typeof(TValue).IsValueType)
        {
            for (var i = 0; i < entries.Length; i++)
                if (entries[i].Next >= -1 && EqualityComparer<TValue>.Default.Equals(entries[i].Data.Value, value))
                    return true;
        }
        else if (value == null)
        {
            for (var i = 0; i < entries.Length; i++)
                if (entries[i].Next >= -1 && entries[i].Data.Value == null)
                    return true;
        }
        else
        {
            var comparer = EqualityComparer<TValue>.Default;
            for (var i = 0; i < entries.Length; i++)
                if (entries[i].Next >= -1 && comparer.Equals(entries[i].Data.Value, value))
                    return true;
        }

        return false;
    }

    /// <inheritdoc />
    public void Add(TKey key, TValue value)
    {
        ref var v = ref GetValueRefOrAddDefault(key, out var exists);
        if (exists)
            ThrowHelper.ThrowArgumentException_AddingDuplicateKey(key);

        v = value;
    }

    /// <summary>
    /// Attempts to add the specified key and value to the dictionary.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add. It can be <see lanword="null"/>.</param>
    /// <returns>
    /// <see lanword="true"/> if the key/value pair was added to the dictionary successfully;
    /// otherwise, <see lanword="false"/>.
    /// </returns>
    public bool TryAdd(TKey key, TValue value)
    {
        ref var v = ref GetValueRefOrAddDefault(key, out var exists);
        if (exists)
            return false;

        v = value;
        return true;
    }

    /// <inheritdoc cref="IDictionary{TKey,TValue}.TryGetValue" />
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        ref var entry = ref FindEntry(key);

        if (Unsafe.IsNullRef(ref entry))
        {
            value = default;
            return false;
        }

        value = entry.Data.Value;
        return true;
    }

    /// <summary>
    /// Returns either a reference to a <typeparamref name="TValue"/> associated with the specified key
    /// or a reference <see langword="null" /> if it does not exist.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>
    /// A reference to a <typeparamref name="TValue"/> or a reference <see langword="null" /> if it does not exist.
    /// </returns>
    public ref readonly TValue GetValueRefOrNullRef(TKey key)
    {
        ref var value = ref Unsafe.NullRef<TValue>();
        ref var entry = ref FindEntry(key);

        if (!Unsafe.IsNullRef(ref entry))
            value = ref Unsafe.AsRef(in entry.Data.Value)!;

        return ref value!;
    }

    /// <inheritdoc />
    public bool Remove(TKey key) =>
        Remove(key, out _);

    /// <summary>
    /// Removes the value with the specified key from the <see cref="DictionarySlim{TKey,TValue}"/>,
    /// and copies the element to the value parameter.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    /// <param name="value">The removed element.</param>
    /// <returns>
    /// <see lanword="true"/> if the element is successfully found and removed; otherwise, <see lanword="false"/>.
    /// </returns>
    public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (ReferenceEquals(key, null))
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);

        var entries = _entries;
        var comparer = _comparer;
        var hashCode = typeof(TKey).IsValueType && comparer is null ? key.GetHashCode() : comparer!.GetHashCode(key);
        var bucketIndex = hashCode & (entries.Length - 1);
        var i = entries[bucketIndex].Bucket - 1;
        var last = -1;

        if (typeof(TKey).IsValueType && comparer is null)
        {
            while ((uint)i < (uint)entries.Length)
            {
                ref var entry = ref entries[i];

                if (entry.Next >= -1 && EqualityComparer<TKey>.Default.Equals(entry.Data.Key, key))
                {
                    value = entry.Data.Value;

                    if (last < 0)
                        entries[bucketIndex].Bucket = entry.Next + 1;
                    else
                        entries[last].Next = entry.Next;

                    if (!typeof(TKey).IsValueType)
                        Unsafe.AsRef(in entry.Data.Key) = default!;

                    if (!typeof(TValue).IsValueType)
                        Unsafe.AsRef(in entry.Data.Value) = default!;

                    entry.Next = -3 - _freeIndex;
                    _freeIndex = i;
                    _freeCount++;

                    return true;
                }

                last = i;
                i = entry.Next;
            }
        }
        else
        {
            while ((uint)i < (uint)entries.Length)
            {
                ref var entry = ref entries[i];

                if (entry.Next >= -1 && comparer!.Equals(entry.Data.Key, key))
                {
                    value = entry.Data.Value;

                    if (last < 0)
                        entries[bucketIndex].Bucket = entry.Next + 1;
                    else
                        entries[last].Next = entry.Next;

                    if (!typeof(TKey).IsValueType)
                        Unsafe.AsRef(in entry.Data.Key) = default!;

                    if (!typeof(TValue).IsValueType)
                        Unsafe.AsRef(in entry.Data.Value) = default!;

                    entry.Next = -3 - _freeIndex;
                    _freeIndex = i;
                    _freeCount++;

                    return true;
                }

                last = i;
                i = entry.Next;
            }
        }

        value = default!;
        return false;
    }

    /// <inheritdoc cref="ICollection{T}.Clear" />
    public void Clear()
    {
        var count = _count;
        if (count > 0)
        {
            _count = 0;
            _freeIndex = 0;
            _freeCount = 0;
            Array.Clear(_entries, 0, count);
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="DictionarySlim{TKey,TValue}"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="Enumerator"/> structure for the <see cref="DictionarySlim{TKey,TValue}"/>.
    /// </returns>
    public Enumerator GetEnumerator() =>
        new(this);

    #region IDictionary<TKey,TValue> implementation

    /// <inheritdoc />
    ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

    /// <inheritdoc />
    ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

    #endregion

    #region IReadOnlyDictionary<TKey,TValue> implementation

    /// <inheritdoc />
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

    /// <inheritdoc />
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

    #endregion

    #region IDictionary implementation

    /// <inheritdoc />
    bool IDictionary.IsFixedSize => false;

    /// <inheritdoc />
    bool IDictionary.IsReadOnly => false;

    /// <inheritdoc />
    object? IDictionary.this[object key]
    {
        get => this[(TKey)key];
        set => this[(TKey)key] = (TValue)value!;
    }

    /// <inheritdoc />
    bool IDictionary.Contains(object key) =>
        ContainsKey((TKey)key);

    /// <inheritdoc />
    void IDictionary.Add(object key, object? value) =>
        Add((TKey)key, (TValue)value!);

    /// <inheritdoc />
    void IDictionary.Remove(object key) =>
        Remove((TKey)key);

    /// <inheritdoc />
    IDictionaryEnumerator IDictionary.GetEnumerator() =>
        new DictionarySlimEnumerator(this, isGeneric: false);

    #endregion

    #region ICollection<KeyValuePair<TKey,TValue>> implementation

    /// <inheritdoc />
    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

    /// <inheritdoc />
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
        ThrowHelper.ThrowNotSupportedException();
        return false;
    }

    /// <inheritdoc />
    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) =>
        Add(item.Key, item.Value);

    /// <inheritdoc />
    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
        ref var entry = ref FindEntry(item.Key);
        return !Unsafe.IsNullRef(ref entry) && EqualityComparer<TValue>.Default.Equals(item.Value, entry.Data.Value);
    }

    /// <inheritdoc />
    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        var target = array.AsSpan(arrayIndex);
        var source = _entries.AsSpan(0, _count);

        if (target.Length < Count)
            ThrowHelper.ThrowArgumentException_DestinationTooShort(ExceptionArgument.array);

        target = target[..Count];

        for (int j = 0, i = 0; i < source.Length; i++)
        {
            ref var entry = ref source[i];

            if (entry.Next >= -1 && (uint)j < (uint)target.Length)
            {
                target[j] = new KeyValuePair<TKey, TValue>(entry.Data.Key, entry.Data.Value);
                j++;
            }
        }
    }

    #endregion

    #region ICollection implementation

    /// <inheritdoc />
    bool ICollection.IsSynchronized => false;

    /// <inheritdoc />
    object ICollection.SyncRoot => this;

    /// <inheritdoc />
    ICollection IDictionary.Keys => Keys;

    /// <inheritdoc />
    ICollection IDictionary.Values => Values;

    /// <inheritdoc />
    void ICollection.CopyTo(Array array, int index)
    {
        if (array is KeyValuePair<TKey, TValue>[] pairs)
        {
            var source = _entries.AsSpan(0, _count);
            var target = pairs.AsSpan(index, Count);

            for (int j = 0, i = 0; i < source.Length; i++)
            {
                ref var entry = ref source[i];

                if (entry.Next >= -1 && (uint)j < (uint)target.Length)
                {
                    target[j] = new KeyValuePair<TKey, TValue>(entry.Data.Key, entry.Data.Value);
                    j++;
                }
            }
        }
        else if (array is DictionaryEntry[] entries)
        {
            var source = _entries.AsSpan(0, _count);
            var target = entries.AsSpan(index, Count);

            for (int j = 0, i = 0; i < source.Length; i++)
            {
                ref var entry = ref source[i];

                if (entry.Next >= -1 && (uint)j < (uint)target.Length)
                {
                    target[j] = new DictionaryEntry(entry.Data.Key, entry.Data.Value);
                    j++;
                }
            }
        }
        else
        {
            try
            {
                var target = ((object[])array).AsSpan(index, Count);
                var source = _entries.AsSpan(0, _count);

                for (int j = 0, i = 0; i < source.Length; i++)
                {
                    ref var entry = ref source[i];

                    if (entry.Next >= -1)
                    {
                        target[j] = new KeyValuePair<TKey,TValue>(entry.Data.Key, entry.Data.Value);
                        j++;
                    }
                }
            }
            catch (Exception e) when (e is InvalidCastException or ArrayTypeMismatchException)
            {
                throw new ArgumentException("Target array type is not compatible with the type of items in the collection.", nameof(array));
            }
        }
    }

    #endregion

    #region IEnumerable<KeyValuePair<TKey,TValue>> implementation

    /// <inheritdoc />
    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() =>
        new DictionarySlimEnumerator(this, true);

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() =>
        new DictionarySlimEnumerator(this, true);

    #endregion

    private void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
    {
        if (enumerable is DictionarySlim<TKey, TValue> ds)
        {
            if ((object?)ds._comparer == _comparer)
            {
                CopyEntries(ds._entries, ds._count);
            }
            else
            {
                foreach (ref readonly var entry in ds)
                    Add(entry.Key, entry.Value);
            }
        }
        else if (enumerable is Dictionary<TKey, TValue> d)
        {
            foreach (var kvp in d)
                Add(kvp.Key, kvp.Value);
        }
        else
        {
            Span<KeyValuePair<TKey, TValue>> span;
            switch (enumerable)
            {
                case KeyValuePair<TKey, TValue>[] array:
                    span = array;
                    break;

                case List<KeyValuePair<TKey, TValue>> list:
                    span = CollectionsMarshal.AsSpan(list);
                    break;

                default:
                {
                    foreach (var kvp in enumerable)
                        Add(kvp.Key, kvp.Value);
                    return;
                }
            }

            foreach (ref var kvp in span)
                Add(kvp.Key, kvp.Value);
        }
    }

    private ref Entry FindEntry(TKey key)
    {
        if (ReferenceEquals(key, null))
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);

        ref var entry = ref Unsafe.NullRef<Entry>();

        var entries = _entries;
        var comparer = _comparer;
        var hashCode = typeof(TKey).IsValueType && comparer is null ? key.GetHashCode() : comparer!.GetHashCode(key);
        var bucketIndex = hashCode & (entries.Length - 1);

        if ((uint)bucketIndex < (uint)entries.Length)
        {
            var i = entries[bucketIndex].Bucket - 1;
            var collisions = 0;

            if (typeof(TKey).IsValueType && comparer is null)
            {
                while ((uint)i < (uint)entries.Length)
                {
                    entry = ref entries[i];
                    if (entry.Next >= -1 && EqualityComparer<TKey>.Default.Equals(entry.Data.Key, key))
                        break;

                    if (++collisions > entries.Length)
                        ThrowHelper.ThrowNotSupportedException_ConcurrentOperationsNotSupported();

                    i = entry.Next;
                    entry = ref Unsafe.NullRef<Entry>();
                }
            }
            else
            {
                while ((uint)i < (uint)entries.Length)
                {
                    entry = ref entries[i];
                    if (entry.Next >= -1 && comparer!.Equals(entry.Data.Key, key))
                        break;

                    if (++collisions > entries.Length)
                        ThrowHelper.ThrowNotSupportedException_ConcurrentOperationsNotSupported();

                    i = entry.Next;
                    entry = ref Unsafe.NullRef<Entry>();
                }
            }
        }

        return ref entry;
    }

    private ref TValue GetValueRefOrAddDefault(TKey key, out bool exists)
    {
        if (ReferenceEquals(key, null))
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);

        var entries = _entries;
        var comparer = _comparer;
        var hashCode = typeof(TKey).IsValueType && comparer is null ? key.GetHashCode() : comparer!.GetHashCode(key);
        var bucketIndex = hashCode & (entries.Length - 1);

        if ((uint)bucketIndex < (uint)entries.Length)
        {
            var i = entries[bucketIndex].Bucket - 1;
            var collisions = 0;

            if (typeof(TKey).IsValueType && comparer is null)
            {
                while ((uint)i < (uint)entries.Length)
                {
                    ref var item = ref entries[i];
                    if (item.Next >= -1 && EqualityComparer<TKey>.Default.Equals(item.Data.Key, key))
                    {
                        exists = true;
                        return ref Unsafe.AsRef(in item.Data.Value);
                    }

                    if (++collisions > entries.Length)
                        ThrowHelper.ThrowNotSupportedException_ConcurrentOperationsNotSupported();

                    i = item.Next;
                }
            }
            else
            {
                while ((uint)i < (uint)entries.Length)
                {
                    ref var item = ref entries[i];

                    if (item.Next >= -1 && comparer!.Equals(item.Data.Key, key))
                    {
                        exists = true;
                        return ref Unsafe.AsRef(in item.Data.Value);
                    }

                    if (++collisions > entries.Length)
                        ThrowHelper.ThrowNotSupportedException_ConcurrentOperationsNotSupported();

                    i = item.Next;
                }
            }
        }

        var index = _count;
        var freeIndex = _freeIndex;

        if (_freeCount == 0)
        {
            if (index == entries.Length)
                entries = Resize();

            bucketIndex = hashCode & (entries.Length - 1);
            _count = index + 1;
        }
        else
        {
            index = freeIndex;

            _freeIndex = -3 - entries[freeIndex].Next;
            _freeCount--;
        }

        ref var bucket = ref entries[bucketIndex];
        ref var entry = ref entries[index];

        entry.Next = bucket.Bucket - 1;
        bucket.Bucket = index + 1;
        Unsafe.AsRef(in entry.Data.Key) = key;

        exists = false;
        return ref Unsafe.AsRef(in entry.Data.Value);
    }

    private void CopyEntries(Entry[] source, int count)
    {
        var target = _entries;
        var comparer = _comparer;

        for (var i = 0; i < count; i++)
        {
            if (source[i].Next >= -1)
            {
                var hashCode = typeof(TKey).IsValueType && comparer is null
                    ? source[i].Data.Key.GetHashCode()
                    : comparer!.GetHashCode(source[i].Data.Key);

                var bucketIndex = hashCode & (target.Length - 1);
                target[i].Next = target[bucketIndex].Bucket - 1;
                target[bucketIndex].Bucket = i + 1;
                target[i].Data = source[i].Data;
            }
        }
    }

    private Entry[] Resize()
    {
        var entries = _entries;
        var capacity = entries.Length == 0 ? 4 : entries.Length * 2;

        if ((uint)capacity > (uint)Array.MaxLength)
            capacity = Array.MaxLength;

        var newEntries = new Entry[capacity];
        var comparer = _comparer;

        for (var i = 0; i < entries.Length; i++)
        {
            if (entries[i].Next >= -1)
            {
                var hashCode = typeof(TKey).IsValueType && comparer is null
                    ? entries[i].Data.Key.GetHashCode()
                    : comparer!.GetHashCode(entries[i].Data.Key);

                var bucketIndex = hashCode & (newEntries.Length - 1);
                newEntries[i].Next = newEntries[bucketIndex].Bucket - 1;
                newEntries[bucketIndex].Bucket = i + 1;
                newEntries[i].Data = entries[i].Data;
            }
        }

        return _entries = newEntries;
    }

    #region Inner type: Entry

    /// <summary>
    /// Represents an entry in the dictionary, containing metadata and the actual key-value data.
    /// </summary>
    private struct Entry
    {
        /// <summary>
        /// The bucket index for this entry.
        /// </summary>
        internal int Bucket;

        /// <summary>
        /// The index of the next entry in the same bucket.
        /// </summary>
        /// <remarks>
        /// 0-based index of next entry in chain: -1 means end of chain
        /// also encodes whether this entry _itself_ is part of the free list by changing sign and subtracting 3,
        /// so -2 means end of free list, -3 means index 0 but on free list, -4 means index 1 but on free list, etc.
        /// </remarks>
        internal int Next;

        /// <summary>
        /// The key-value data associated with this entry.
        /// </summary>
        public EntryData Data;
    }

    #endregion

    #region Inner type: EntryData

    /// <summary>
    /// Represents a read-only entry in the dictionary, containing a key-value pair.
    /// </summary>
    public readonly struct EntryData
    {
        /// <summary>
        /// The key of the entry.
        /// </summary>
        // ReSharper disable once UnassignedReadonlyField
        public readonly TKey Key;

        /// <summary>
        /// The value associated with the key in the entry.
        /// </summary>
        // ReSharper disable once UnassignedReadonlyField
        public readonly TValue Value;
    }

    #endregion

    #region Inner type: KeyCollection

    /// <summary>
    /// Represents the collection of keys in a <see cref="DictionarySlim{TKey,TValue}"/>.
    /// </summary>
    public readonly struct KeyCollection : ICollection<TKey>, IReadOnlyCollection<TKey>, ICollection
    {
        private readonly DictionarySlim<TKey, TValue> _dictionary;

        /// <inheritdoc cref="ICollection{T}.Count" />
        public int Count => _dictionary.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyCollection"/> class that reflects the keys
        /// in the specified <see cref="DictionarySlim{TKey,TValue}"/>.
        /// </summary>
        /// <param name="dictionary">The <see cref="DictionarySlim{TKey,TValue}"/> whose keys are reflected
        /// in the new <see cref="KeyCollection"/>.</param>
        internal KeyCollection(DictionarySlim<TKey, TValue> dictionary) =>
            _dictionary = dictionary;

        /// <inheritdoc />
        public bool Contains(TKey item) =>
            _dictionary.ContainsKey(item);

        /// <summary>
        /// Copies the contents of this <see cref="KeyCollection"/> into a destination <see cref="Span{T}"/>.
        /// </summary>
        /// <param name="destination">The span to copy the items into.</param>
        /// <remarks>
        /// This method ensures that all items are copied to the destination span.
        /// If the destination span is too small, an exception is thrown.
        /// </remarks>
        public void CopyTo(Span<TKey> destination)
        {
            if (destination.Length < _dictionary.Count)
                ThrowHelper.ThrowArgumentException_DestinationTooShort(ExceptionArgument.destination);

            var source = _dictionary._entries.AsSpan(0, _dictionary._count);

            for (int j = 0, i = 0; i < source.Length; i++)
            {
                if (source[i].Next >= -1 && (uint)j < (uint)destination.Length)
                {
                    destination[j] = source[i].Data.Key;
                    j++;
                }
            }
        }

        /// <summary>
        /// Attempts to copy the contents of this <see cref="KeyCollection"/> into a destination <see cref="Span{T}"/>.
        /// </summary>
        /// <param name="destination">The span to copy the items into.</param>
        /// <returns>
        /// <see langword="true"/> if the items were successfully copied to the destination span;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method copies the items to the destination span only if the span is large enough to hold all the items.
        /// If the destination span is too small, no action is taken, and the method returns <see langword="false"/>.
        /// </remarks>
        public bool TryCopyTo(Span<TKey> destination)
        {
            if (destination.Length >= _dictionary.Count)
            {
                CopyTo(destination);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="KeyCollection"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Enumerator"/> structure for the <see cref="KeyCollection"/>.
        /// </returns>
        public Enumerator GetEnumerator() =>
            new(_dictionary);

        #region ICollection<T> implementation

        /// <inheritdoc />
        bool ICollection<TKey>.IsReadOnly => true;

        /// <inheritdoc />
        void ICollection<TKey>.Add(TKey item) =>
            ThrowHelper.ThrowNotSupportedException();

        /// <inheritdoc />
        void ICollection<TKey>.Clear() =>
            ThrowHelper.ThrowNotSupportedException();

        /// <inheritdoc />
        void ICollection<TKey>.CopyTo(TKey[] array, int arrayIndex) =>
            CopyTo(array.AsSpan(arrayIndex));

        /// <inheritdoc />
        bool ICollection<TKey>.Remove(TKey item)
        {
            ThrowHelper.ThrowNotSupportedException();
            return false;
        }

        #endregion

        #region ICollection implementation

        /// <inheritdoc />
        bool ICollection.IsSynchronized => false;

        /// <inheritdoc />
        object ICollection.SyncRoot => _dictionary;

        /// <inheritdoc />
        void ICollection.CopyTo(Array array, int index)
        {
            if (array is TKey[] keys)
            {
                CopyTo(keys.AsSpan(index));
            }
            else if (array is object?[] destination)
            {
                var target = destination.AsSpan(index, _dictionary.Count);
                var source = _dictionary._entries.AsSpan(0, _dictionary._count);

                for (int j = 0, i = 0; i < source.Length; i++)
                {
                    if (source[i].Next >= -1 && (uint)j < (uint)target.Length)
                    {
                        target[j] = source[i].Data.Key;
                        j++;
                    }
                }
            }
        }

        #endregion

        #region IEnumerable<T> implementation

        /// <inheritdoc />
        IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() =>
            new KeyCollectionEnumerator(_dictionary);

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() =>
            new KeyCollectionEnumerator(_dictionary);

        #endregion

        #region Inner type: Enumerator

        /// <summary>
        /// Represents an enumerator implementation that is used to iterate through
        /// the <see cref="KeyCollection"/> collection.
        /// </summary>
        public struct Enumerator
        {
            private readonly Entry[] _array;
            private int _index;
            private int _count;

            /// <summary>
            /// Gets a readonly reference to the element at the current position of the enumerator.
            /// </summary>
            public readonly ref readonly TKey Current => ref _array[_index].Data.Key;

            /// <summary>
            /// Initializes a new instance of the <see cref="Enumerator"/> structure.
            /// </summary>
            /// <param name="dictionary">The dictionary instance.</param>
            public Enumerator(DictionarySlim<TKey, TValue> dictionary)
            {
                _index = -1;
                _count = dictionary._count;
                _array = dictionary._entries;

                //
                // Hint to JIT that _array is not null
                //
                _ = _array.Length;

                Debug.Assert(_count >= 0 && _count < _array.Length);
            }

            /// <summary>
            /// Advances the enumerator to the next element of the <see cref="KeyCollection"/>.
            /// </summary>
            /// <returns>
            /// <see lanword="true"/> if the enumerator was successfully advanced to the next element; <see lanword="false"/>
            /// if the enumerator has passed the end of the collection.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                while (--_count >= 0)
                    if (_array.GetRawArrayData(++_index).Next >= -1)
                        return true;

                return false;
            }
        }

        #endregion

        #region Inner type: KeyCollectionEnumerator

        /// <summary>
        /// Represents an enumerator implementation that is used to iterate through
        /// the <see cref="KeyCollection"/> collection.
        /// </summary>
        private sealed class KeyCollectionEnumerator : IEnumerator<TKey>
        {
            private readonly Entry[] _array;
            private int _index;
            private int _count;

            /// <inheritdoc />
            object IEnumerator.Current => Current;

            /// <inheritdoc />
            public TKey Current => _array[_index].Data.Key;

            /// <summary>
            /// Initializes a new instance of the <see cref="DictionarySlimEnumerator"/> class.
            /// </summary>
            /// <param name="dictionary">The dictionary instance.</param>
            public KeyCollectionEnumerator(DictionarySlim<TKey, TValue> dictionary)
            {
                _index = -1;
                _count = dictionary._count;
                _array = dictionary._entries;

                Debug.Assert(_count >= 0 && _count < _array.Length);
            }

            /// <inheritdoc />
            public bool MoveNext()
            {
                while (--_count >= 0)
                    if (_array.GetRawArrayData(++_index).Next >= -1)
                        return true;

                return false;
            }

            /// <inheritdoc />
            public void Reset() =>
                ThrowHelper.ThrowNotSupportedException();

            /// <inheritdoc />
            public void Dispose()
            {
            }
        }

        #endregion
    }

    #endregion

    #region Inner type: ValueCollection

    /// <summary>
    /// Represents the collection of values in a <see cref="DictionarySlim{TKey,TValue}"/>.
    /// </summary>
    public readonly struct ValueCollection : ICollection<TValue>, IReadOnlyCollection<TValue>, ICollection
    {
        private readonly DictionarySlim<TKey, TValue> _dictionary;
        public int Count => _dictionary.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueCollection"/> class that reflects the values
        /// in the specified <see cref="DictionarySlim{TKey,TValue}"/>.
        /// </summary>
        /// <param name="dictionary">The <see cref="DictionarySlim{TKey,TValue}"/> whose values are reflected
        /// in the new <see cref="ValueCollection"/>.</param>
        internal ValueCollection(DictionarySlim<TKey, TValue> dictionary) =>
            _dictionary = dictionary;

        /// <inheritdoc />
        public bool Contains(TValue item) =>
            _dictionary.ContainsValue(item);

        /// <summary>
        /// Copies the contents of this <see cref="ValueCollection"/> into a destination <see cref="Span{T}"/>.
        /// </summary>
        /// <param name="destination">The span to copy the items into.</param>
        /// <remarks>
        /// This method ensures that all items are copied to the destination span.
        /// If the destination span is too small, an exception is thrown.
        /// </remarks>
        public void CopyTo(Span<TValue> destination)
        {
            if (destination.Length < _dictionary.Count)
                ThrowHelper.ThrowArgumentException_DestinationTooShort(ExceptionArgument.destination);

            var source = _dictionary._entries.AsSpan(0, _dictionary._count);

            for (int j = 0, i = 0; i < source.Length; i++)
            {
                if (source[i].Next >= -1 && (uint)j < (uint)destination.Length)
                {
                    destination[j] = source[i].Data.Value;
                    j++;
                }
            }
        }

        /// <summary>
        /// Attempts to copy the contents of this <see cref="KeyCollection"/> into a destination <see cref="Span{T}"/>.
        /// </summary>
        /// <param name="destination">The span to copy the items into.</param>
        /// <returns>
        /// <see langword="true"/> if the items were successfully copied to the destination span;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method copies the items to the destination span only if the span is large enough to hold all the items.
        /// If the destination span is too small, no action is taken, and the method returns <see langword="false"/>.
        /// </remarks>
        public bool TryCopyTo(Span<TValue> destination)
        {
            if (destination.Length >= _dictionary.Count)
            {
                CopyTo(destination);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ValueCollection"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Enumerator"/> structure for the <see cref="ValueCollection"/>.
        /// </returns>
        public Enumerator GetEnumerator() =>
            new(_dictionary);

        #region ICollection<T> implementation

        /// <inheritdoc />
        bool ICollection<TValue>.IsReadOnly => true;

        /// <inheritdoc />
        void ICollection<TValue>.Add(TValue item) =>
            ThrowHelper.ThrowNotSupportedException();

        /// <inheritdoc />
        void ICollection<TValue>.Clear() =>
            ThrowHelper.ThrowNotSupportedException();

        /// <inheritdoc />
        void ICollection<TValue>.CopyTo(TValue[] array, int arrayIndex) =>
            CopyTo(array.AsSpan(arrayIndex));

        /// <inheritdoc />
        bool ICollection<TValue>.Remove(TValue item)
        {
            ThrowHelper.ThrowNotSupportedException();
            return false;
        }

        #endregion

        #region ICollection implementation

        /// <inheritdoc />
        bool ICollection.IsSynchronized => false;

        /// <inheritdoc />
        object ICollection.SyncRoot => _dictionary;

        /// <inheritdoc />
        void ICollection.CopyTo(Array array, int index)
        {
            if (array is TValue[] values)
            {
                CopyTo(values.AsSpan(index));
            }
            else if (array is object?[] destination)
            {
                var target = destination.AsSpan(index, _dictionary.Count);
                var source = _dictionary._entries.AsSpan(0, _dictionary._count);

                for (int j = 0, i = 0; i < source.Length; i++)
                {
                    if (source[i].Next >= -1 && (uint)j < (uint)target.Length)
                    {
                        target[j] = source[i].Data.Value;
                        j++;
                    }
                }
            }
        }

        #endregion

        #region IEnumerable<T> implementation

        /// <inheritdoc />
        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() =>
            new ValueCollectionEnumerator(_dictionary);

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() =>
            new ValueCollectionEnumerator(_dictionary);

        #endregion

        #region Inner type: Enumerator

        /// <summary>
        /// Represents an enumerator implementation that is used to iterate through
        /// the <see cref="ValueCollection"/> collection.
        /// </summary>
        public struct Enumerator
        {
            private readonly Entry[] _array;
            private int _index;
            private int _count;

            /// <summary>
            /// Gets a readonly reference to the element at the current position of the enumerator.
            /// </summary>
            public readonly ref readonly TValue Current => ref _array[_index].Data.Value;

            /// <summary>
            /// Initializes a new instance of the <see cref="Enumerator"/> structure.
            /// </summary>
            /// <param name="dictionary">The dictionary instance.</param>
            public Enumerator(DictionarySlim<TKey, TValue> dictionary)
            {
                _index = -1;
                _count = dictionary._count;
                _array = dictionary._entries;

                //
                // Hint to JIT that _array is not null
                //
                _ = _array.Length;

                Debug.Assert(_count >= 0 && _count < _array.Length);
            }

            /// <summary>
            /// Advances the enumerator to the next element of the <see cref="ValueCollection"/>.
            /// </summary>
            /// <returns>
            /// <see lanword="true"/> if the enumerator was successfully advanced to the next element; <see lanword="false"/>
            /// if the enumerator has passed the end of the collection.
            /// </returns>
            public bool MoveNext()
            {
                while (--_count >= 0)
                    if (_array.GetRawArrayData(++_index).Next >= -1)
                        return true;

                return false;
            }
        }

        #endregion

        #region Inner type: ValueCollectionEnumerator

        /// <summary>
        /// Represents an enumerator implementation that is used to iterate through
        /// the <see cref="ValueCollection"/> collection.
        /// </summary>
        private sealed class ValueCollectionEnumerator : IEnumerator<TValue>
        {
            private readonly Entry[] _array;
            private int _index;
            private int _count;

            /// <inheritdoc />
            object? IEnumerator.Current => Current;

            /// <inheritdoc />
            public TValue Current => _array[_index].Data.Value;

            /// <summary>
            /// Initializes a new instance of the <see cref="DictionarySlimEnumerator"/> class.
            /// </summary>
            /// <param name="dictionary">The dictionary instance.</param>
            public ValueCollectionEnumerator(DictionarySlim<TKey, TValue> dictionary)
            {
                _index = -1;
                _count = dictionary._count;
                _array = dictionary._entries;

                Debug.Assert(_count >= 0 && _count < _array.Length);
            }

            /// <inheritdoc />
            public bool MoveNext()
            {
                while (--_count >= 0)
                    if (_array.GetRawArrayData(++_index).Next >= -1)
                        return true;

                return false;
            }

            /// <inheritdoc />
            public void Reset() =>
                ThrowHelper.ThrowNotSupportedException();

            /// <inheritdoc />
            public void Dispose()
            {
            }
        }

        #endregion
    }

    #endregion

    #region Inner type: Enumerator

    /// <summary>
    /// Represents an enumerator implementation that is used to iterate through
    /// the <see cref="DictionarySlim{TKey,TValue}"/> collection.
    /// </summary>
    public struct Enumerator
    {
        private readonly Entry[] _array;
        private int _index;
        private int _count;

        /// <summary>
        /// Gets a readonly reference to the element at the current position of the enumerator.
        /// </summary>
        public ref readonly EntryData Current => ref _array[_index].Data;

        /// <summary>
        /// Initializes a new instance of the <see cref="Enumerator"/> structure.
        /// </summary>
        /// <param name="dictionary">The dictionary instance.</param>
        public Enumerator(DictionarySlim<TKey, TValue> dictionary)
        {
            _index = -1;
            _count = dictionary._count;
            _array = dictionary._entries;

            //
            // Hint to JIT that _array is not null
            //
            _ = _array.Length;

            Debug.Assert(_count >= 0 && _count < _array.Length);
        }

        /// <summary>
        /// Advances the enumerator to the next element of the <see cref="DictionarySlim{TKey,TValue}"/>.
        /// </summary>
        /// <returns>
        /// <see lanword="true"/> if the enumerator was successfully advanced to the next element; <see lanword="false"/>
        /// if the enumerator has passed the end of the collection.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            while (--_count >= 0)
                if (_array.GetRawArrayData(++_index).Next >= -1)
                    return true;

            _index = -1;
            return false;
        }
    }

    #endregion

    #region Inner type: DictionarySlimEnumerator

    /// <summary>
    /// Represents an enumerator implementation that is used to iterate through
    /// the <see cref="DictionarySlim{TKey,TValue}"/> collection.
    /// </summary>
    private sealed class DictionarySlimEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
    {
        private readonly Entry[] _array;
        private int _index;
        private int _count;
        private KeyValuePair<TKey, TValue> _current;
        private readonly bool _isGeneric;

        /// <inheritdoc />
        public KeyValuePair<TKey, TValue> Current => _current;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionarySlimEnumerator"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary instance.</param>
        /// <param name="isGeneric">A flag indicating whether the enumerator should return
        /// generic <see cref="KeyValuePair{TKey, TValue}"/> objects. If <see langword="false"/>, the enumerator
        /// will return non-generic <see cref="DictionaryEntry"/> objects.</param>
        public DictionarySlimEnumerator(DictionarySlim<TKey, TValue> dictionary, bool isGeneric)
        {
            _index = -1;
            _count = dictionary._count;
            _isGeneric = isGeneric;
            _array = dictionary._entries;

            Debug.Assert(_count >= 0 && _count < _array.Length);
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            while (--_count >= 0)
            {
                var entry = _array.GetRawArrayData(++_index);
                if (entry.Next >= -1)
                {
                    _current = new KeyValuePair<TKey, TValue>(entry.Data.Key, entry.Data.Value);
                    return true;
                }
            }

            _index = -1;
            return false;
        }

        /// <inheritdoc />
        public void Reset() =>
            ThrowHelper.ThrowNotSupportedException();

        /// <inheritdoc />
        public void Dispose()
        {
        }

        #region IEnumerator implementation

        /// <inheritdoc />
        object IEnumerator.Current => _isGeneric ? _current : new DictionaryEntry(_current.Key, _current.Value);

        #endregion

        #region IDictionaryEnumerator implementation

        /// <inheritdoc />
        public object Key => _current.Key;

        /// <inheritdoc />
        public object? Value => _current.Value;

        /// <inheritdoc />
        public DictionaryEntry Entry => new(_current.Key, _current.Value);

        #endregion
    }

    #endregion
}
