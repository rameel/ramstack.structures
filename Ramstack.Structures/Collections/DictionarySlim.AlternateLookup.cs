#if NET9_0_OR_GREATER

namespace Ramstack.Collections;

partial class DictionarySlim<TKey, TValue>
{
    /// <summary>
    /// Returns an instance of a type that can be used to perform operations on the current
    /// <see cref="DictionarySlim{TKey,TValue}"/> using a <typeparamref name="TAlternateKey"/>
    /// as a key instead of a <typeparamref name="TKey"/>.
    /// </summary>
    /// <remarks>
    /// The dictionary must use a comparer that implements <see cref="IAlternateEqualityComparer{TAlternate,T}"/>
    /// with <typeparamref name="TAlternateKey"/> and <typeparamref name="TKey"/>.
    /// If it doesn't, an exception is thrown.
    /// </remarks>
    /// <typeparam name="TAlternateKey">The alternate type of key for performing lookups.</typeparam>
    /// <returns>
    /// The created lookup instance.
    /// </returns>
    public AlternateLookup<TAlternateKey> GetAlternateLookup<TAlternateKey>() where TAlternateKey : notnull, allows ref struct
    {
        if (_comparer is not IAlternateEqualityComparer<TAlternateKey, TKey>)
            ThrowHelper.ThrowInvalidOperationException_IncompatibleComparer();

        return new AlternateLookup<TAlternateKey>(this);
    }

    /// <summary>
    /// Provides a type that can be used to perform operations on a <see cref="DictionarySlim{TKey,TValue}"/>
    /// using a <typeparamref name="TAlternateKey"/> as a key.
    /// </summary>
    /// <typeparam name="TAlternateKey">The alternate type of key for performing lookups.</typeparam>
    public readonly struct AlternateLookup<TAlternateKey> where TAlternateKey : notnull, allows ref struct
    {
        private readonly DictionarySlim<TKey, TValue> _dictionary;

        /// <summary>
        /// Gets the <see cref="DictionarySlim{TKey,TValue}"/> against which this instance performs operations.
        /// </summary>
        public DictionarySlim<TKey, TValue> Dictionary => _dictionary;

        /// <summary>
        /// Gets or sets the value associated with the specified alternate key.
        /// </summary>
        /// <param name="key">The alternate key of the value to get or set.</param>
        public TValue this[TAlternateKey key]
        {
            get
            {
                ref var entry = ref FindEntry(key);
                if (Unsafe.IsNullRef(ref entry))
                    ThrowHelper.ThrowKeyNotFoundException(GetAlternateComparer(Dictionary).Create(key));

                return entry.Data.Value;
            }
            set => GetValueRefOrAddDefault(key, out _) = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlternateLookup{TAlternateKey}"/> structure.
        /// </summary>
        /// <param name="dictionary">The dictionary instance.</param>
        internal AlternateLookup(DictionarySlim<TKey, TValue> dictionary)
        {
            Debug.Assert(dictionary._comparer is IAlternateEqualityComparer<TAlternateKey, TKey>);
            _dictionary = dictionary;
        }

        /// <summary>
        /// Determines whether the <see cref="DictionarySlim{TKey,TValue}"/> contains the specified alternate key.
        /// </summary>
        /// <param name="key">The alternate key to check.</param>
        /// <returns>
        /// <see langword="true"/> if the key is in the dictionary; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ContainsKey(TAlternateKey key) =>
            !Unsafe.IsNullRef(in FindEntry(key));

        /// <summary>
        /// Attempts to add the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The alternate key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>
        /// <see langword="true"/> if the key/value pair was added to the dictionary successfully;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool TryAdd(TAlternateKey key, TValue value)
        {
            ref var v = ref GetValueRefOrAddDefault(key, out var exists);
            if (exists)
                return false;

            v = value;
            return true;
        }

        /// <summary>
        /// Gets the value associated with the specified alternate key.
        /// </summary>
        /// <param name="key">The alternate key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key,
        /// if the key is found; otherwise, the default value for the type of the value parameter.</param>
        /// <returns>
        /// <see langword="true"/> if an entry was found; otherwise, <see langword="false"/>.
        /// </returns>
        public bool TryGetValue(TAlternateKey key, [MaybeNullWhen(false)] out TValue value)
        {
            ref var entry = ref FindEntry(key);
            if (!Unsafe.IsNullRef(ref entry))
            {
                value = entry.Data.Value;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the value associated with the specified alternate key.
        /// </summary>
        /// <param name="key">The alternate key of the value to get.</param>
        /// <param name="actualKey">When this method returns, contains the actual key associated with the alternate key,
        /// if the key is found; otherwise, the default value for the type of the key parameter.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key,
        /// if the key is found; otherwise, the default value for the type of the value parameter.</param>
        /// <returns>
        /// <see langword="true"/> if an entry was found; otherwise, <see langword="false"/>.
        /// </returns>
        public bool TryGetValue(TAlternateKey key, [MaybeNullWhen(false)] out TKey actualKey, [MaybeNullWhen(false)] out TValue value)
        {
            ref var entry = ref FindEntry(key);
            if (!Unsafe.IsNullRef(ref entry))
            {
                actualKey = entry.Data.Key;
                value = entry.Data.Value;
                return true;
            }

            actualKey = default;
            value = default;
            return false;
        }

        /// <summary>
        /// Removes the value with the specified alternate key from the <see cref="DictionarySlim{TKey,TValue}"/>.
        /// </summary>
        /// <param name="key">The alternate key of the element to remove.</param>
        /// <returns>
        /// <see langword="true"/> if the element is successfully found and removed;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool Remove(TAlternateKey key) =>
            Remove(key, out _, out _);

        /// <summary>
        /// Removes the value with the specified alternate key from the <see cref="DictionarySlim{TKey,TValue}"/>,
        /// and copies the element to the value parameter.
        /// </summary>
        /// <param name="key">The alternate key of the element to remove.</param>
        /// <param name="actualKey">When this method returns, contains the removed key.</param>
        /// <param name="value">When this method returns, contains the removed element.</param>
        /// <returns>
        /// <see langword="true"/> if the element is successfully found and removed;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool Remove(TAlternateKey key, [MaybeNullWhen(false)] out TKey actualKey, [MaybeNullWhen(false)] out TValue value)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (key == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);

            var entries = _dictionary._entries;
            var comparer = GetAlternateComparer(_dictionary);
            var hashCode = comparer.GetHashCode(key);
            var bucketIndex = hashCode & (entries.Length - 1);
            var i = entries[bucketIndex].Bucket - 1;
            var last = -1;

            while ((uint)i < (uint)entries.Length)
            {
                ref var entry = ref entries[i];

                if (entry.Next >= -1 && comparer.Equals(key, entry.Data.Key))
                {
                    actualKey = entry.Data.Key;
                    value = entry.Data.Value;

                    if (last < 0)
                        entries[bucketIndex].Bucket = entry.Next + 1;
                    else
                        entries[last].Next = entry.Next;

                    if (!typeof(TKey).IsValueType)
                        Unsafe.AsRef(in entry.Data.Key) = default!;

                    if (!typeof(TValue).IsValueType)
                        Unsafe.AsRef(in entry.Data.Value) = default!;

                    entry.Next = -3 - _dictionary._freeIndex;
                    _dictionary._freeIndex = i;
                    _dictionary._freeCount++;

                    return true;
                }

                last = i;
                i = entry.Next;
            }

            actualKey = default!;
            value = default!;
            return false;
        }

        private ref Entry FindEntry(TAlternateKey key)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (key == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);

            ref var entry = ref Unsafe.NullRef<Entry>();

            var entries = _dictionary._entries;
            var comparer = GetAlternateComparer(_dictionary);
            var hashCode = comparer.GetHashCode(key);
            var bucketIndex = hashCode & (entries.Length - 1);

            if ((uint)bucketIndex < (uint)entries.Length)
            {
                var i = entries[bucketIndex].Bucket - 1;
                var collisions = 0;

                while ((uint)i < (uint)entries.Length)
                {
                    entry = ref entries[i];
                    if (entry.Next >= -1 && comparer.Equals(key, entry.Data.Key))
                        break;

                    if (++collisions > entries.Length)
                        ThrowHelper.ThrowNotSupportedException_ConcurrentOperationsNotSupported();

                    i = entry.Next;
                    entry = ref Unsafe.NullRef<Entry>();
                }
            }

            return ref entry;
        }

        private ref TValue GetValueRefOrAddDefault(TAlternateKey key, out bool exists)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (key == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);

            var entries = _dictionary._entries;
            var comparer = GetAlternateComparer(_dictionary);
            var hashCode = comparer.GetHashCode(key);
            var bucketIndex = hashCode & (entries.Length - 1);

            if ((uint)bucketIndex < (uint)entries.Length)
            {
                var i = entries[bucketIndex].Bucket - 1;
                var collisions = 0;

                while ((uint)i < (uint)entries.Length)
                {
                    ref var item = ref entries[i];

                    if (item.Next >= -1 && comparer.Equals(key, item.Data.Key))
                    {
                        exists = true;
                        return ref Unsafe.AsRef(in item.Data.Value);
                    }

                    if (++collisions > entries.Length)
                        ThrowHelper.ThrowNotSupportedException_ConcurrentOperationsNotSupported();

                    i = item.Next;
                }
            }

            var index = _dictionary._count;
            var freeIndex = _dictionary._freeIndex;

            if (_dictionary._freeCount == 0)
            {
                if (index == entries.Length)
                    entries = _dictionary.Resize();

                bucketIndex = hashCode & (entries.Length - 1);
                _dictionary._count = index + 1;
            }
            else
            {
                index = freeIndex;

                _dictionary._freeIndex = -3 - entries[freeIndex].Next;
                _dictionary._freeCount--;
            }

            ref var bucket = ref entries[bucketIndex];
            ref var entry = ref entries[index];

            entry.Next = bucket.Bucket - 1;
            bucket.Bucket = index + 1;
            Unsafe.AsRef(in entry.Data.Key) = comparer.Create(key);

            exists = false;
            return ref Unsafe.AsRef(in entry.Data.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAlternateEqualityComparer<TAlternateKey, TKey> GetAlternateComparer(DictionarySlim<TKey, TValue> dictionary) =>
            Unsafe.As<IAlternateEqualityComparer<TAlternateKey, TKey>>(dictionary._comparer!);
    }
}

#endif
