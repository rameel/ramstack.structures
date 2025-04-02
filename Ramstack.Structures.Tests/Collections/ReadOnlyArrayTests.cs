using System.Collections.Immutable;

namespace Ramstack.Collections;

[TestFixture]
public class ReadOnlyArrayTests
{
    [Test]
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Default_BehavesAsUnderlyingArray()
    {
        var array = new ReadOnlyArray<int>();

        Assert.That(array.IsDefault, Is.True);

        Assert.That(() => array.AsSpan(), Throws.Nothing);
        Assert.That(() => array.AsSpan(0), Throws.Nothing);
        Assert.That(() => array.AsSpan(0, 0), Throws.Nothing);
        Assert.That(() => array.AsView(), Throws.Nothing);
        Assert.That(() => array.AsView(0), Throws.Nothing);
        Assert.That(() => array.AsView(0, 0), Throws.Nothing);
        Assert.That(() => array.GetHashCode(), Throws.Nothing);

        Assert.Throws<NullReferenceException>(() => _ = array.Length);
        Assert.Throws<NullReferenceException>(() => _ = array[0]);
        Assert.Throws<NullReferenceException>(() => array.GetEnumerator());
        Assert.Throws<NullReferenceException>(() => array.GetPinnableReference());
        Assert.Throws<NullReferenceException>(() => array.Any());
        Assert.Throws<NullReferenceException>(() => array.Any(_ => true));
        Assert.Throws<NullReferenceException>(() => array.All(_ => true));
        Assert.Throws<NullReferenceException>(() => array.ToArray());
        Assert.Throws<NullReferenceException>(() => array.Slice(0));
        Assert.Throws<NullReferenceException>(() => array.Slice(0, 0));

        Assert.Throws<ArgumentNullException>(() => array.IndexOf(0));
        Assert.Throws<ArgumentNullException>(() => array.LastIndexOf(0));
    }

    [Test]
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Default_Boxed()
    {
        var array = (IReadOnlyList<int>)new ReadOnlyArray<int>();

        Assert.Throws<InvalidOperationException>(() => _ = array.Count);
        Assert.Throws<InvalidOperationException>(() => _ = array[0]);
        Assert.Throws<InvalidOperationException>(() => array.GetEnumerator());
    }

    [Test]
    public void Ctor()
    {
        for (var count = 1; count < 10; count++)
        {
            var items = CreateArray(count);
            var array = count switch
            {
                1 => new ReadOnlyArray<int>(items[0]),
                2 => new ReadOnlyArray<int>(items[0], items[1]),
                3 => new ReadOnlyArray<int>(items[0], items[1], items[2]),
                4 => new ReadOnlyArray<int>(items[0], items[1], items[2], items[3]),
                _ => new ReadOnlyArray<int>(items)
            };

            Assert.That(array.Length, Is.EqualTo(items.Length));
            Assert.That(array, Is.EquivalentTo(items));
        }
    }

    [TestCase(0)]
    [TestCase(4)]
    [TestCase(9)]
    public void Ctor_Span(int length)
    {
        ReadOnlySpan<int> span = CreateArray(length);
        var array = ReadOnlyArray.Create(span);

        Assert.That(array.Length, Is.EqualTo(length));
        Assert.That(array, Is.EquivalentTo(span.ToArray()));
    }

    [Test]
    public void Create()
    {
        #pragma warning disable IDE0303
        for (var count = 1; count < 10; count++)
        {
            var items = Enumerable.Range(1, count).ToArray();
            var array = count switch
            {
                1 => ReadOnlyArray.Create(items[0]),
                2 => ReadOnlyArray.Create(items[0], items[1]),
                3 => ReadOnlyArray.Create(items[0], items[1], items[2]),
                4 => ReadOnlyArray.Create(items[0], items[1], items[2], items[3]),
                _ => ReadOnlyArray.Create(items)
            };

            Assert.That(array.Length, Is.EqualTo(items.Length));
            Assert.That(array, Is.EquivalentTo(items));
        }
        #pragma warning restore IDE0303
    }

    [TestCase(0)]
    [TestCase(4)]
    [TestCase(9)]
    public void Create_Span(int length)
    {
        ReadOnlySpan<int> span = Enumerable.Range(1, length).ToArray();
        var array = ReadOnlyArray.Create(span);

        Assert.That(array.Length, Is.EqualTo(length));
        Assert.That(array, Is.EquivalentTo(span.ToArray()));
    }

    [TestCase(0)]
    [TestCase(4)]
    [TestCase(9)]
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public void Create_Enumerable(int length)
    {
        var enumerable = Enumerable.Range(1, length);
        var array = ReadOnlyArray.Create(enumerable);

        Assert.That(array.Length, Is.EqualTo(length));
        Assert.That(array, Is.EquivalentTo(enumerable));
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(5)]
    [TestCase(9)]
    public void Indexer(int count)
    {
        var array = Enumerable.Range(1, count).ToReadOnlyArray();

        Assert.That(array.Length, Is.EqualTo(count));

        for (var i = 0; i < array.Length; i++)
            Assert.That(array[i], Is.EqualTo(i + 1));
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(5)]
    [TestCase(9)]
    public void GetEnumerator(int count)
    {
        var array = Enumerable.Range(1, count).ToReadOnlyArray();

        var x = 1;
        foreach (var v in array)
            Assert.That(v, Is.EqualTo(x++));

        Assert.That(array.Length, Is.EqualTo(count));
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(5)]
    [TestCase(9)]
    public void GetEnumerator_Boxed(int count)
    {
        var array = Enumerable.Range(1, count).ToReadOnlyArray();

        var x = 1;
        foreach (var v in (IEnumerable<int>)array)
            Assert.That(v, Is.EqualTo(x++));

        Assert.That(array.Length, Is.EqualTo(count));
    }

    [Test]
    public void Indexer_Ref()
    {
        var array = ReadOnlyArray.Create((Key: 1, Value: 1), (Key: 2, Value: 2));

        Assert.That(array[0].Key, Is.EqualTo(1));
        Assert.That(array[1].Key, Is.EqualTo(2));

        Unsafe.AsRef(in array[0]).Key = 3;
        Unsafe.AsRef(in array[1]).Key = 4;

        Assert.That(array[0].Key, Is.EqualTo(3));
        Assert.That(array[1].Key, Is.EqualTo(4));
    }

    [Test]
    public void Indexer_OutOfBounds()
    {
        Assert.Throws<IndexOutOfRangeException>(() => _ = ReadOnlyArray.Create(1, 2, 3, 4)[5]);
        Assert.Throws<IndexOutOfRangeException>(() => _ = ReadOnlyArray<int>.Empty[5]);
        Assert.Throws<IndexOutOfRangeException>(() => _ = ReadOnlyArray.Empty<int>()[5]);
    }

    [Test]
    public void Empty()
    {
        Assert.That(ReadOnlyArray<int>.Empty.Length, Is.Zero);
        Assert.That(ReadOnlyArray.Empty<int>().Length, Is.Zero);

        Assert.That(ReadOnlyArray<int>.Empty.IsDefault, Is.False);
        Assert.That(ReadOnlyArray<int>.Empty.IsDefaultOrEmpty, Is.True);
        Assert.That(ReadOnlyArray.Empty<int>().IsDefault, Is.False);
        Assert.That(ReadOnlyArray.Empty<int>().IsDefaultOrEmpty, Is.True);
    }

    [TestCase("value", 0)]
    [TestCase("value", 5)]
    [TestCase("key=value", 4)]
    public void Slice_Index(string value, int index)
    {
        Assert.That(
            value.ToCharArray().ToReadOnlyArray().Slice(index),
            Is.EquivalentTo(value.AsSpan(index).ToArray()));
    }

    [TestCase("value", 0, 0)]
    [TestCase("value", 5, 0)]
    [TestCase("value", 0, 5)]
    [TestCase("key=value", 0, 3)]
    [TestCase("key=value", 3, 1)]
    [TestCase("key=value", 4, 5)]
    public void Slice_Range(string value, int index, int length)
    {
        Assert.That(
            value.ToCharArray().ToReadOnlyArray().Slice(index, length),
            Is.EquivalentTo(value.AsSpan(index, length).ToArray()));
    }

    [TestCase(1)]
    [TestCase(6)]
    [TestCase(-1)]
    public void Slice_InvalidIndex_ShouldThrow(int index)
    {
        Assert.That(
            () => Array.Empty<int>().ToReadOnlyArray().Slice(index),
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [TestCase(1, 0)]
    [TestCase(0, 1)]
    [TestCase(6, 0)]
    [TestCase(0, 6)]
    [TestCase(5, 1)]
    [TestCase(4, 2)]
    [TestCase(-1, 0)]
    [TestCase(4, -1)]
    public void Slice_InvalidRange_ShouldThrow(int index, int length)
    {
        Assert.That(
            () => Array.Empty<int>().ToReadOnlyArray().Slice(index, length),
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [TestCase("value", 0)]
    [TestCase("value", 5)]
    [TestCase("key=value", 4)]
    public void AsView_Index(string value, int index)
    {
        Assert.That(
            value.ToCharArray().ToReadOnlyArray().AsView(index),
            Is.EquivalentTo(value.AsSpan(index).ToArray()));
    }

    [TestCase("value", 0, 0)]
    [TestCase("value", 5, 0)]
    [TestCase("value", 0, 5)]
    [TestCase("key=value", 0, 3)]
    [TestCase("key=value", 3, 1)]
    [TestCase("key=value", 4, 5)]
    public void AsView_Range(string value, int index, int length)
    {
        Assert.That(
            value.ToCharArray().ToReadOnlyArray().AsView(index, length),
            Is.EquivalentTo(value.AsSpan(index, length).ToArray()));
    }

    [TestCase(1)]
    [TestCase(6)]
    [TestCase(-1)]
    public void AsView_InvalidIndex_ShouldThrow(int index)
    {
        Assert.That(
            () => Array.Empty<int>().ToReadOnlyArray().AsView(index),
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [TestCase(1, 0)]
    [TestCase(0, 1)]
    [TestCase(6, 0)]
    [TestCase(0, 6)]
    [TestCase(5, 1)]
    [TestCase(4, 2)]
    [TestCase(-1, 0)]
    [TestCase(4, -1)]
    public void AsView_InvalidRange_ShouldThrow(int index, int length)
    {
        Assert.That(
            () => Array.Empty<int>().ToReadOnlyArray().AsView(index, length),
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [TestCase("value", 0)]
    [TestCase("value", 5)]
    [TestCase("key=value", 4)]
    public void AsSpan_Index(string value, int index)
    {
        Assert.That(
            value.ToCharArray().ToReadOnlyArray().AsSpan(index).ToArray(),
            Is.EquivalentTo(value.AsSpan(index).ToArray()));
    }

    [TestCase("value", 0, 0)]
    [TestCase("value", 5, 0)]
    [TestCase("value", 0, 5)]
    [TestCase("key=value", 0, 3)]
    [TestCase("key=value", 3, 1)]
    [TestCase("key=value", 4, 5)]
    public void AsSpan_Range(string value, int index, int length)
    {
        Assert.That(
            value.ToCharArray().ToReadOnlyArray().AsSpan(index, length).ToArray(),
            Is.EquivalentTo(value.AsSpan(index, length).ToArray()));
    }

    [TestCase(1)]
    [TestCase(6)]
    [TestCase(-1)]
    public void AsSpan_InvalidIndex_ShouldThrow(int index)
    {
        Assert.That(
            () => Array.Empty<int>().ToReadOnlyArray().AsSpan(index),
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [TestCase(1, 0)]
    [TestCase(0, 1)]
    [TestCase(6, 0)]
    [TestCase(0, 6)]
    [TestCase(5, 1)]
    [TestCase(4, 2)]
    [TestCase(-1, 0)]
    [TestCase(4, -1)]
    public void AsSpan_InvalidRange_ShouldThrow(int index, int length)
    {
        Assert.That(
            () => Array.Empty<int>().ToReadOnlyArray().AsSpan(index, length),
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [TestCase(0)]
    [TestCase(3)]
    [TestCase(9)]
    public void GetPinnableReference(int count)
    {
        var items = new int[count];
        var array = ReadOnlyArray.UnsafeWrap(items);

        Assert.That(
            Unsafe.AreSame(
                ref Unsafe.AsRef(in array.GetPinnableReference()),
                ref Unsafe.AsRef(in MemoryMarshal.GetArrayDataReference(items))),
            Is.True);
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(5)]
    [TestCase(9)]
    public void CopyTo(int length)
    {
        var source = CreateArray(length).ToReadOnlyArray();
        var destination = new int[length];

        source.CopyTo(destination.AsSpan());

        Assert.That(source, Is.EquivalentTo(destination));
    }

    [TestCase(1)]
    [TestCase(5)]
    [TestCase(9)]
    public void CopyTo_InsufficientRoom_ShouldThrow(int length)
    {
        var source = CreateArray(length).ToReadOnlyArray();
        var destination = new int[length - 1];

        Assert.Throws<ArgumentException>(() => source.CopyTo(destination.AsSpan()));
        Assert.That(destination, Is.EquivalentTo(new int[length - 1]));
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(5)]
    [TestCase(9)]
    public void TryCopyTo(int length)
    {
        var source = CreateArray(length).ToReadOnlyArray();
        var destination = new int[length];

        Assert.That(source.TryCopyTo(destination.AsSpan()), Is.True);
        Assert.That(source, Is.EquivalentTo(destination));
    }

    [TestCase(1)]
    [TestCase(5)]
    [TestCase(9)]
    public void TryCopyTo_InsufficientRoom_ShouldThrow(int length)
    {
        var source = CreateArray(length).ToReadOnlyArray();
        var destination = new int[length - 1];

        Assert.That(source.TryCopyTo(destination.AsSpan()), Is.False);
        Assert.That(destination, Is.EquivalentTo(new int[length - 1]));
    }

    [Test]
    public void UnsafeWrap()
    {
        var array = new int[10];
        var readonlyArray = ReadOnlyArray.UnsafeWrap(array);

        array[5] = 0xABCD;
        Assert.That(readonlyArray[5], Is.EqualTo(0xABCD));
    }

    [Test]
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public void ToReadOnlyArray_Enumerable()
    {
        var enumerable = Enumerable.Range(1, 100);

        Assert.That(
            enumerable.ToReadOnlyArray(),
            Is.EquivalentTo(enumerable));
    }

    [Test]
    public void ToReadOnlyArray_ImmutableArray()
    {
        var immutable = Enumerable.Range(1, 100).ToImmutableArray();
        var array = immutable.ToReadOnlyArray();

        Assert.That(
            Unsafe.AreSame(
                ref Unsafe.AsRef(in array[0]),
                ref Unsafe.AsRef(in immutable.ItemRef(0))),
            Is.True);
    }

    [Test]
    public void ToReadOnlyArray_Span()
    {
        var source = Enumerable.Range(1, 100).ToArray();

        Span<int> span1 = source;
        ReadOnlySpan<int> span2 = span1;

        var array1 = span1.ToReadOnlyArray();
        var array2 = span2.ToReadOnlyArray();

        Assert.That(array1, Is.EquivalentTo(source));
        Assert.That(array2, Is.EquivalentTo(source));
    }

    [Test]
    public void ToReadOnlyArray_Array()
    {
        var items = Enumerable.Range(1, 100).ToArray();
        var array = items.ToReadOnlyArray();

        Assert.That(array, Is.EquivalentTo(items));
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(9)]
    public void IndexOf(int length)
    {
        var array = CreateArray(length);
        var readonlyArray = array.ToReadOnlyArray();

        for (var i = 0; i <= length; i++)
            Assert.That(
                readonlyArray.IndexOf(i),
                Is.EqualTo(Array.IndexOf(array, i)));
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(9)]
    public void LastIndexOf(int length)
    {
        var array = CreateArray(length);
        var readonlyArray = array.ToReadOnlyArray();

        for (var i = 0; i <= length; i++)
            Assert.That(
                readonlyArray.LastIndexOf(i),
                Is.EqualTo(Array.LastIndexOf(array, i)));
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(9)]
    public void Contains(int length)
    {
        var array = CreateArray(length);
        var readonlyArray = array.ToReadOnlyArray();

        for (var i = -1; i <= length; i++)
            Assert.That(
                readonlyArray.Contains(i),
                Is.EqualTo(Array.LastIndexOf(array, i) >= 0));
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(9)]
    public void Interface_IEnumerable(int count)
    {
        var array = CreateArray(count);
        var collection = (IEnumerable<int>)array.ToReadOnlyArray();

        var value = 0;
        foreach (var item in collection)
            Assert.That(item, Is.EqualTo(value++));
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(9)]
    [SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
    public void Interface_IReadOnlyList(int count)
    {
        var array = CreateArray(count);
        var collection = (IReadOnlyList<int>)array.ToReadOnlyArray();

        Assert.That(collection.Count, Is.EqualTo(count));

        var value = 0;
        for (var i = 0; i < collection.Count; i++)
            Assert.That(collection[i], Is.EqualTo(value++));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("test")]
    public void Operator_Conversions(string? value)
    {
        var array = ReadOnlyArray.UnsafeWrap(value?.ToCharArray()!);

        ArrayView<char> view = array;
        ReadOnlyMemory<char> memory = array;
        ReadOnlySpan<char> span = array;
        ImmutableArray<char> immutable = array;
        ReadOnlyArray<char> @readonly = immutable;

        Assert.That(view.Length, Is.EqualTo(value?.Length ?? 0));
        Assert.That(span.Length, Is.EqualTo(value?.Length ?? 0));
        Assert.That(memory.Length, Is.EqualTo(value?.Length ?? 0));

        Assert.That(array.IsDefault, Is.EqualTo(immutable.IsDefault));
        Assert.That(array.IsDefault, Is.EqualTo(@readonly.IsDefault));
        Assert.That(array.IsDefaultOrEmpty, Is.EqualTo(immutable.IsDefaultOrEmpty));
        Assert.That(array.IsDefaultOrEmpty, Is.EqualTo(@readonly.IsDefaultOrEmpty));

        if (!array.IsDefault)
        {
            Assert.That(array.Length, Is.EqualTo(value!.Length));
            Assert.That(immutable.Length, Is.EqualTo(value.Length));
            Assert.That(@readonly.Length, Is.EqualTo(value.Length));
        }

        if (view.Length != 0)
        {
            Assert.That(Unsafe.AreSame(ref Unsafe.AsRef(in array[0]), ref Unsafe.AsRef(in view[0])), Is.True);
            Assert.That(Unsafe.AreSame(ref Unsafe.AsRef(in array[0]), ref Unsafe.AsRef(in span[0])), Is.True);
            Assert.That(Unsafe.AreSame(ref Unsafe.AsRef(in array[0]), ref Unsafe.AsRef(in memory.Span[0])), Is.True);
            Assert.That(Unsafe.AreSame(ref Unsafe.AsRef(in array[0]), ref Unsafe.AsRef(in immutable.ItemRef(0))), Is.True);
            Assert.That(Unsafe.AreSame(ref Unsafe.AsRef(in array[0]), ref Unsafe.AsRef(in @readonly[0])), Is.True);
        }
    }

    private static int[] CreateArray(int length) =>
        Enumerable.Range(0, length).ToArray();
}
