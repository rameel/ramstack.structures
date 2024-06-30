using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using NUnit.Framework;

namespace Ramstack.Collections;

[TestFixture]
[SuppressMessage("ReSharper", "ReplaceSliceWithRangeIndexer")]
public class ArrayViewTests
{
    [Test]
    public void Empty_HaveZeroLength()
    {
        Assert.That(ArrayView<int>.Empty.Length, Is.Zero);
    }

    [Test]
    public void Default_DoesNotThrow()
    {
        var view = default(ArrayView<int>);

        Assert.That(view.Length, Is.Zero);
        Assert.That(view.IsDefault, Is.True);

        for (var i = 0; i < view.Length; i++)
            Assert.Fail();

        foreach (var _ in view)
            Assert.Fail();

        foreach (var _ in view.AsEnumerable())
            Assert.Fail();

        foreach (var _ in view.AsSpan())
            Assert.Fail();

        Assert.That(view.ToArray(), Is.EquivalentTo(Array.Empty<int>()));
        Assert.That(view.AsMemory().ToArray(), Is.EquivalentTo(Array.Empty<int>()));
        Assert.That(view.Slice(0).Length, Is.Zero);
        Assert.That(view.Slice(0, 0).Length, Is.Zero);
        Assert.That(view, Is.EquivalentTo(Array.Empty<int>()));
    }

    [TestCase("value", 0, 0)]
    [TestCase("value", 0, 5)]
    [TestCase("value", 1, 4)]
    [TestCase("value", 2, 3)]
    [TestCase("value", 3, 2)]
    [TestCase("value", 4, 1)]
    [TestCase("value", 5, 0)]
    public void Enumeration(string value, int index, int length)
    {
        var view = new ArrayView<char>(value.ToCharArray(), index, length);
        var span = value.AsSpan(index, length);

        Assert.That(view.Length, Is.EqualTo(span.Length));

        for (var i = 0; i < view.Length; i++)
            Assert.That(view[i], Is.EqualTo(span[i]));

        var j = 0;
        foreach (var c in view)
            Assert.That(c, Is.EqualTo(span[j++]));

        j = 0;
        foreach (var c in view.AsEnumerable())
            Assert.That(c, Is.EqualTo(span[j++]));

        for (var i = 0; i < view.Length; i++)
            Assert.That(view[i], Is.EqualTo(span[i]));
    }

    [TestCase("")]
    [TestCase("value")]
    [TestCase("key=value")]
    public void Ctor(string value)
    {
        Assert.That(
            new ArrayView<char>(value.ToCharArray()),
            Is.EquivalentTo(value));
    }

    [TestCase("value", 0)]
    [TestCase("value", 5)]
    [TestCase("key=value", 4)]
    public void Ctor_Index(string value, int index)
    {
        Assert.That(
            new ArrayView<char>(value.ToCharArray(), index),
            Is.EquivalentTo(value.AsSpan(index).ToArray()));
    }

    [TestCase("", 0, 0)]
    [TestCase("value", 0, 0)]
    [TestCase("value", 5, 0)]
    [TestCase("value", 0, 5)]
    [TestCase("key=value", 0, 3)]
    [TestCase("key=value", 3, 1)]
    [TestCase("key=value", 4, 5)]
    public void Ctor_Range(string value, int index, int length)
    {
        Assert.That(
            new ArrayView<char>(value.ToCharArray(), index, length),
            Is.EquivalentTo(value.AsSpan(index, length).ToArray()));
    }

    [Test]
    public void Ctor_NullInput_ShouldThrow()
    {
        Assert.That(() => new ArrayView<int>(null!), Throws.TypeOf<NullReferenceException>());
        Assert.That(() => new ArrayView<int>(null!, 0), Throws.TypeOf<NullReferenceException>());
        Assert.That(() => new ArrayView<int>(null!, 0, 0), Throws.TypeOf<NullReferenceException>());
    }

    [TestCase("", 1)]
    [TestCase("value", -1)]
    [TestCase("value", 6)]
    public void Ctor_InvalidIndex_ShouldThrow(string value, int index)
    {
        Assert.That(
            () => new ArrayView<char>(value.ToCharArray(), index),
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [TestCase("", 1, 0)]
    [TestCase("", 0, 1)]
    [TestCase("value", -1, 0)]
    [TestCase("value", 6, 0)]
    [TestCase("value", 0, 6)]
    [TestCase("value", 5, 1)]
    [TestCase("value", 4, 2)]
    public void Ctor_InvalidRange_ShouldThrow(string value, int index, int length)
    {
        Assert.That(
            () => new ArrayView<char>(value.ToCharArray(), index, length),
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void Slice()
    {
        var view = default(ArrayView<int>);

        Assert.That(view.Slice(0).Length, Is.Zero);
        Assert.That(view.Slice(0, 0).Length, Is.Zero);
    }

    [TestCase("value", 0)]
    [TestCase("value", 5)]
    [TestCase("key=value", 4)]
    public void Slice_Index(string value, int index)
    {
        Assert.That(
            new ArrayView<char>(value.ToCharArray()).Slice(index),
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
            new ArrayView<char>(value.ToCharArray()).Slice(index, length),
            Is.EquivalentTo(value.AsSpan(index, length).ToArray()));
    }

    [TestCase("", 1)]
    [TestCase("value", -1)]
    [TestCase("value", 6)]
    public void Slice_InvalidIndex_ShouldThrow(string value, int index)
    {
        Assert.That(
            () => new ArrayView<char>(value.ToCharArray()).Slice(index),
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [TestCase("", 1, 0)]
    [TestCase("", 0, 1)]
    [TestCase("value", -1, 0)]
    [TestCase("value", 6, 0)]
    [TestCase("value", 0, 6)]
    [TestCase("value", 5, 1)]
    [TestCase("value", 4, 2)]
    public void Slice_InvalidRange_ShouldThrow(string value, int index, int length)
    {
        Assert.That(
            () => new ArrayView<char>(value.ToCharArray()).Slice(index, length),
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [TestCase(null, 0, 0)]
    [TestCase("", 0, 0)]
    [TestCase("value", 0, 0)]
    [TestCase("value", 5, 0)]
    [TestCase("value", 0, 5)]
    [TestCase("key=value", 0, 3)]
    [TestCase("key=value", 3, 1)]
    [TestCase("key=value", 4, 5)]
    public void Length(string? value, int index, int length)
    {
        Assert.That(
            (value?.ToCharArray()).AsView(index, length).Length,
            Is.EqualTo(length));
    }

    [TestCase(null, 0, 0)]
    [TestCase("", 0, 0)]
    [TestCase("value", 0, 0)]
    [TestCase("value", 5, 0)]
    [TestCase("value", 0, 5)]
    [TestCase("key=value", 0, 3)]
    [TestCase("key=value", 3, 1)]
    [TestCase("key=value", 4, 5)]
    public void ToArray(string? value, int index, int length)
    {
        Assert.That(
            (value?.ToCharArray()).AsView(index, length).ToArray(),
            Is.EquivalentTo(value.AsSpan(index, length).ToArray()));
    }

    [TestCase("", 0, 0)]
    [TestCase("id;name;lastname", 3, 0)]
    [TestCase("id;name;lastname", 3, 4)]
    [TestCase("id;name;lastname", 0, 2)]
    [TestCase("id;name;lastname", 8, 8)]
    public void GetPinnableReference(string value, int index, int length)
    {
        var list = value.ToCharArray();
        var view = list.AsView(index, length);
        var span = list.AsSpan(index, length);

        Assert.That(
            Unsafe.AreSame(
                ref Unsafe.AsRef(in view.GetPinnableReference()),
                ref Unsafe.AsRef(in span.GetPinnableReference())),
            Is.True);
    }

    [Test]
    public void GetPinnableReference_Default()
    {
        var view = default(ArrayView<int>);
        var span = default(Span<int>);

        Assert.That(
            Unsafe.AreSame(
                ref Unsafe.AsRef(in view.GetPinnableReference()),
                ref Unsafe.AsRef(in span.GetPinnableReference())),
            Is.True);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("test")]
    public void Operator_Conversions(string? value)
    {
        ArrayView<char> view = value?.ToCharArray();
        ReadOnlyMemory<char> memory = view;
        ReadOnlySpan<char> span = view;

        Assert.That(
            span.ToString() == value.AsSpan().ToString(),
            Is.True);

        Assert.That(
            memory.ToString() == value.AsMemory().ToString(),
            Is.True);

        for (var i = 0; i < view.Length; i++)
        {
            Assert.That(view[i], Is.EqualTo(span[i]));
            Assert.That(view[i], Is.EqualTo(memory.Span[i]));
        }

        var j = 0;
        foreach (var c in view)
        {
            Assert.That(c, Is.EqualTo(span[j]));
            Assert.That(c, Is.EqualTo(memory.Span[j]));
            j++;
        }
    }

    [TestCase("", 0, 0)]
    [TestCase(null, 0, 0)]
    [TestCase("text", 0, 4)]
    [TestCase("key=value", 0, 3)]
    [TestCase("key=value", 3, 1)]
    [TestCase("key=value", 4, 5)]
    [TestCase("key=value", 5, 0)]
    [TestCase("key=value", 3, 0)]
    public void CopyTo(string? source, int index, int length)
    {
        var data = new char[length];
        var list = source?.ToCharArray();
        var view = list.AsView(index, length);

        view.CopyTo(data.AsSpan());

        Assert.That(view, Is.EquivalentTo(data));
    }

    [TestCase("text", 0, 4)]
    [TestCase("key=value", 0, 3)]
    [TestCase("key=value", 3, 1)]
    [TestCase("key=value", 4, 5)]
    public void CopyTo_InsufficientRoom_ShouldThrow(string? source, int index, int length)
    {
        var data = new char[length - 1];
        var list = source?.ToCharArray();
        var view = list.AsView(index, length);

        var e = Assert.Throws<ArgumentException>(() => view.CopyTo(data.AsSpan()))!;
        Assert.That(e.Message, Is.EqualTo("Destination is too short. (Parameter 'destination')"));
    }

    [TestCase("", 0, 0)]
    [TestCase(null, 0, 0)]
    [TestCase("text", 0, 4)]
    [TestCase("key=value", 0, 3)]
    [TestCase("key=value", 3, 1)]
    [TestCase("key=value", 4, 5)]
    [TestCase("key=value", 5, 0)]
    public void TryCopyTo(string? source, int index, int length)
    {
        var data = new char[length];
        var list = source?.ToCharArray();
        var view = list.AsView(index, length);

        Assert.That(view.TryCopyTo(data.AsSpan()), Is.True);
        Assert.That(view, Is.EquivalentTo(data));
    }

    [TestCase("text", 0, 4)]
    [TestCase("key=value", 0, 3)]
    [TestCase("key=value", 3, 1)]
    [TestCase("key=value", 4, 5)]
    public void TryCopyTo_InsufficientRoom_ShouldReturnFalse(string? source, int index, int length)
    {
        var data = new char[length - 1];
        var list = source?.ToCharArray();
        var view = list.AsView(index, length);

        Assert.That(view.TryCopyTo(data.AsSpan()), Is.False);
        Assert.That(data, Is.EquivalentTo(new char[length - 1]));
    }

    [TestCase("kiwi")]
    [TestCase("KIWI")]
    [TestCase("lemon")]
    public void IndexOf(string needle)
    {
        var source = new[] { "apple", "banana", "cherry", "kiwi", "elderberry" };

        Assert.That(
            source.AsView().IndexOf(needle),
            Is.EqualTo(source.AsSpan().IndexOf(needle)));

        Assert.That(
            source.AsView().IndexOf(needle, null),
            Is.EqualTo(Array.IndexOf(source, needle)));

        Assert.That(
            source.AsView().IndexOf(needle, StringComparer.OrdinalIgnoreCase),
            Is.EqualTo(source.ToList().FindIndex(s => s.Equals(needle, StringComparison.OrdinalIgnoreCase))));
    }

    [TestCase("kiwi")]
    [TestCase("KIWI")]
    [TestCase("lemon")]
    public void LastIndexOf(string needle)
    {
        var source = new[] { "apple", "banana", "cherry", "kiwi", "elderberry" };

        Assert.That(
            source.AsView().LastIndexOf(needle),
            Is.EqualTo(source.AsSpan().LastIndexOf(needle)));

        Assert.That(
            source.AsView(1, 3).LastIndexOf(needle),
            Is.EqualTo(source.AsSpan(1, 3).LastIndexOf(needle)));

        Assert.That(
            source.AsView().LastIndexOf(needle, null),
            Is.EqualTo(Array.LastIndexOf(source, needle)));

        Assert.That(
            source.AsView().LastIndexOf(needle, StringComparer.OrdinalIgnoreCase),
            Is.EqualTo(source.ToList().FindLastIndex(s => s.Equals(needle, StringComparison.OrdinalIgnoreCase))));
    }

    [Test]
    public void IndexOf()
    {
        var source = new[] { "apple", "banana", "cherry", "kiwi", "elderberry" };

        Assert.That(
            source.AsView().IndexOf("kiwi"),
            Is.EqualTo(source.AsSpan().IndexOf("kiwi")));

        Assert.That(
            source.AsView().IndexOf("kiwi", null),
            Is.EqualTo(Array.IndexOf(source, "kiwi")));

        Assert.That(
            source.AsView().IndexOf("KIWI", StringComparer.OrdinalIgnoreCase),
            Is.EqualTo(source.AsSpan().IndexOf("kiwi")));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("value")]
    [TestCase("key=value")]
    public void AsView(string? value)
    {
        var list = value?.ToCharArray();
        Assert.That(
            list.AsView(),
            Is.EquivalentTo(value.AsSpan().ToArray()));
    }

    [TestCase(null, 0)]
    [TestCase("", 0)]
    [TestCase("value", 0)]
    [TestCase("value", 5)]
    [TestCase("key=value", 0)]
    [TestCase("key=value", 3)]
    [TestCase("key=value", 4)]
    public void AsView_Index(string? value, int index)
    {
        var list = value?.ToCharArray();

        Assert.That(
            list.AsView(index),
            Is.EquivalentTo(value.AsSpan(index).ToArray()));
    }

    [TestCase(null, 0, 0)]
    [TestCase("", 0, 0)]
    [TestCase("value", 0, 0)]
    [TestCase("value", 5, 0)]
    [TestCase("value", 0, 5)]
    [TestCase("key=value", 0, 3)]
    [TestCase("key=value", 3, 1)]
    [TestCase("key=value", 4, 5)]
    public void AsView_Range(string? value, int index, int length)
    {
        var list = value?.ToCharArray();

        Assert.That(
            list.AsView(index, length),
            Is.EquivalentTo(value.AsSpan(index, length).ToArray()));
    }

    [TestCase(null, 1)]
    [TestCase("", 1)]
    [TestCase("value", -1)]
    [TestCase("value", 6)]
    public void AsView_InvalidIndex_ShouldThrow(string? value, int index)
    {
        var list = value?.ToCharArray();

        Assert.That(
            () => list.AsView(index),
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [TestCase(null, 1, 0)]
    [TestCase("", 1, 0)]
    [TestCase("", 0, 1)]
    [TestCase("value", -1, 0)]
    [TestCase("value", 6, 0)]
    [TestCase("value", 0, 6)]
    [TestCase("value", 5, 1)]
    [TestCase("value", 4, 2)]
    public void AsView_InvalidRange_ShouldThrow(string? value, int index, int length)
    {
        var list = value?.ToCharArray();

        Assert.That(
            () => list.AsView(index, length),
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }
}
