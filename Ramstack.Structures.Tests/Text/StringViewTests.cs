namespace Ramstack.Text;

[TestFixture]
[SuppressMessage("ReSharper", "ReplaceSliceWithRangeIndexer")]
public sealed class StringViewTests
{
    [Test]
    public void Empty_HaveZeroLength()
    {
        Assert.That(StringView.Empty.Length, Is.Zero);
    }

    [Test]
    public void Default_DoesNotThrow()
    {
        var view = default(StringView);

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

        Assert.That(view.ToArray(), Is.EquivalentTo(Array.Empty<char>()));
        Assert.That(view.ToString(), Is.EqualTo(""));
        Assert.That(view.AsMemory().ToString(), Is.EqualTo(""));
        Assert.That(view.Slice(0).Length, Is.Zero);
        Assert.That(view.Slice(0, 0).Length, Is.Zero);

        Assert.That(view == "", Is.True);
        Assert.That(view == "".AsView(), Is.True);
    }

    [TestCase("", 0, 0)]
    [TestCase("    ", 0, 4)]
    [TestCase("    ", 1, 3)]
    [TestCase("    ", 3, 1)]
    [TestCase("value", 0, 5)]
    [TestCase("   value", 0, 8)]
    [TestCase("   value", 3, 5)]
    [TestCase("   value", 2, 6)]
    [TestCase("value ", 0, 6)]
    public void TrimStart(string value, int index, int length)
    {
        Assert.That(
            value.AsView(index, length).TrimStart().ToString(),
            Is.EqualTo(value.AsSpan(index, length).TrimStart().ToString()));
    }

    [TestCase("", '-')]
    [TestCase("--------", '-')]
    [TestCase("--++value", '-')]
    [TestCase("--++value", '+')]
    [TestCase("    value", '-')]
    [TestCase("    value", ' ')]
    [TestCase("value", '-')]
    public void TrimStart_TrimChar(string value, char trimChar)
    {
        Assert.That(
            value.AsView().TrimStart(trimChar).ToString(),
            Is.EqualTo(value.TrimStart(trimChar)));
    }

    [TestCase("", "-+")]
    [TestCase("--++--++-", "-+")]
    [TestCase("--++value", "-+")]
    [TestCase("--++value", "?=")]
    [TestCase("    value", "-+")]
    [TestCase("    value", "-+")]
    [TestCase("    value", "")]
    [TestCase("    value", null)]
    [TestCase("value", "+-")]
    [TestCase("value", null)]
    public void TrimStart_TrimChars(string value, string? trimChars)
    {
        Assert.That(
            value.AsView().TrimStart(trimChars?.ToCharArray()).ToString(),
            Is.EqualTo(value.TrimStart(trimChars?.ToCharArray())));

        Assert.That(
            value.AsView().TrimStart(trimChars.AsSpan()).ToString(),
            Is.EqualTo(value.TrimStart(trimChars?.ToCharArray())));
    }

    [TestCase("", 0, 0)]
    [TestCase("    ", 0, 4)]
    [TestCase("    ", 1, 3)]
    [TestCase("    ", 3, 1)]
    [TestCase("value", 0, 5)]
    [TestCase("value   ", 0, 8)]
    [TestCase("value   ", 0, 5)]
    [TestCase("value   ", 0, 6)]
    [TestCase(" value", 0, 6)]
    public void TrimEnd(string value, int index, int length)
    {
        Assert.That(
            value.AsView(index, length).TrimEnd().ToString(),
            Is.EqualTo(value.AsSpan(index, length).TrimEnd().ToString()));
    }

    [TestCase("", '-')]
    [TestCase("---------", '-')]
    [TestCase("value--++", '-')]
    [TestCase("value--++", '-')]
    [TestCase("value--++", '+')]
    [TestCase("value    ", '-')]
    [TestCase("value    ", ' ')]
    [TestCase("value", '-')]
    public void TrimEnd_TrimChar(string value, char trimChar)
    {
        Assert.That(
            value.AsView().TrimEnd(trimChar).ToString(),
            Is.EqualTo(value.TrimEnd(trimChar)));
    }

    [TestCase("", "-+")]
    [TestCase("++--++--+", "-+")]
    [TestCase("value++--", "-+")]
    [TestCase("value++--", "?=")]
    [TestCase("value    ", "-+")]
    [TestCase("value    ", "-+")]
    [TestCase("value    ", "")]
    [TestCase("value    ", null)]
    [TestCase("value", "+-")]
    [TestCase("value", null)]
    public void TrimEnd_TrimChars(string value, string? trimChars)
    {
        Assert.That(
            value.AsView().TrimEnd(trimChars?.ToCharArray()).ToString(),
            Is.EqualTo(value.TrimEnd(trimChars?.ToCharArray())));

        Assert.That(
            value.AsView().TrimEnd(trimChars.AsSpan()).ToString(),
            Is.EqualTo(value.TrimEnd(trimChars?.ToCharArray())));
    }

    [TestCase("", 0, 0)]
    [TestCase("    ", 0, 4)]
    [TestCase("    ", 1, 3)]
    [TestCase("    ", 3, 1)]
    [TestCase("value", 0, 5)]
    [TestCase("   value", 0, 8)]
    [TestCase("   value", 3, 5)]
    [TestCase("   value", 2, 6)]
    [TestCase("value   ", 0, 8)]
    [TestCase("value   ", 0, 5)]
    [TestCase("value   ", 0, 6)]
    [TestCase("   value   ", 0, 11)]
    [TestCase("   value   ", 1, 9)]
    public void Trim(string value, int index, int length)
    {
        Assert.That(
            value.AsView(index, length).Trim().ToString(),
            Is.EqualTo(value.AsSpan(index, length).Trim().ToString()));
    }

    [TestCase("", '-')]
    [TestCase("---------", '-')]
    [TestCase("++--value--++", '-')]
    [TestCase("++--value--++", '-')]
    [TestCase("++--value--++", '+')]
    [TestCase("    value    ", '-')]
    [TestCase("    value    ", ' ')]
    [TestCase("value", '-')]
    public void Trim_TrimChar(string value, char trimChar)
    {
        Assert.That(
            value.AsView().Trim(trimChar).ToString(),
            Is.EqualTo(value.Trim(trimChar)));
    }

    [TestCase("", "-+")]
    [TestCase("++--++--+", "-+")]
    [TestCase("--++value++--", "-+")]
    [TestCase("--++value++--", "?=")]
    [TestCase("    value    ", "-+")]
    [TestCase("    value    ", "-+")]
    [TestCase("    value    ", "")]
    [TestCase("    value    ", null)]
    [TestCase("value", "+-")]
    [TestCase("value", null)]
    public void Trim_TrimChars(string value, string? trimChars)
    {
        Assert.That(
            value.AsView().Trim(trimChars?.ToCharArray()).ToString(),
            Is.EqualTo(value.Trim(trimChars?.ToCharArray())));

        Assert.That(
            value.AsView().Trim(trimChars.AsSpan()).ToString(),
            Is.EqualTo(value.Trim(trimChars?.ToCharArray())));
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
        var view = new StringView(value, index, length);
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
            new StringView(value).ToString(),
            Is.EqualTo(value.AsSpan().ToString()));
    }

    [TestCase("value", 0)]
    [TestCase("value", 5)]
    [TestCase("key=value", 4)]
    public void Ctor_Index(string value, int index)
    {
        Assert.That(
            new StringView(value, index).ToString(),
            Is.EqualTo(value.AsSpan(index).ToString()));
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
            new StringView(value, index, length).ToString(),
            Is.EqualTo(value.AsSpan(index, length).ToString()));
    }

    [Test]
    public void Ctor_NullInput_ShouldThrow()
    {
        Assert.That(() => new StringView(null!), Throws.TypeOf<NullReferenceException>());
        Assert.That(() => new StringView(null!, 0), Throws.TypeOf<NullReferenceException>());
        Assert.That(() => new StringView(null!, 0, 0), Throws.TypeOf<NullReferenceException>());
    }

    [TestCase("", 1)]
    [TestCase("value", -1)]
    [TestCase("value", 6)]
    public void Ctor_InvalidIndex_ShouldThrow(string value, int index)
    {
        Assert.That(
            () => new StringView(value, index),
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
            () => new StringView(value, index, length),
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void Slice()
    {
        var view = default(StringView);

        Assert.That(view.Slice(0).Length, Is.Zero);
        Assert.That(view.Slice(0, 0).Length, Is.Zero);
    }

    [TestCase("value", 0)]
    [TestCase("value", 5)]
    [TestCase("key=value", 4)]
    public void Slice_Index(string value, int index)
    {
        Assert.That(
            new StringView(value).Slice(index).ToString(),
            Is.EqualTo(value.AsSpan(index).ToString()));
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
            new StringView(value).Slice(index, length).ToString(),
            Is.EqualTo(value.AsSpan(index, length).ToString()));
    }

    [TestCase("", 1)]
    [TestCase("value", -1)]
    [TestCase("value", 6)]
    public void Slice_InvalidIndex_ShouldThrow(string value, int index)
    {
        Assert.That(
            () => new StringView(value).Slice(index),
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
            () => new StringView(value).Slice(index, length),
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [TestCase("value", "value", 0, 5)]
    [TestCase("key=value", "value", 4, 5)]
    [TestCase("key=value", "key", 0, 3)]
    [TestCase("key=value", "=", 3, 1)]
    [TestCase("key=value", ">", 3, 1)]
    [TestCase("value", "final", 0, 5)]
    [TestCase("key=value", "final", 0, 5)]
    [TestCase("key=value", "source", 0, 5)]
    [TestCase(null, null, 0, 0)]
    [TestCase(null, "", 0, 0)]
    [TestCase("", null, 0, 0)]
    [TestCase("", "", 0, 0)]
    public void Equals(string? value, string? other, int index, int length)
    {
        var view1 = value.AsView(index, length);
        var view2 = other.AsView();

        var span1 = value.AsSpan(index, length);
        var span2 = other.AsSpan();

        Assert.That(view1.Equals(other), Is.EqualTo(span1.SequenceEqual(span2)));

        // ReSharper disable once SuspiciousTypeConversion.Global
        Assert.That(view1.Equals((object?)other), Is.EqualTo(span1.SequenceEqual(span2)));
        Assert.That(view1.Equals((object?)other.AsView()), Is.EqualTo(span1.SequenceEqual(span2)));

        Assert.That(StringView.Equals(view1, other), Is.EqualTo(span1.SequenceEqual(span2)));
        Assert.That(StringView.Equals(view1, view2), Is.EqualTo(span1.SequenceEqual(span2)));
    }

    [TestCase("value", "VALUE", 0, 5)]
    [TestCase("key=value", "VALUE", 4, 5)]
    [TestCase("key=value", "Key", 0, 3)]
    [TestCase("key=value", "=", 3, 1)]
    [TestCase("key=value", ">", 3, 1)]
    [TestCase("value", "final", 0, 5)]
    [TestCase("key=value", "final", 0, 5)]
    [TestCase("key=value", "source", 0, 5)]
    [TestCase(null, null, 0, 0)]
    [TestCase(null, "", 0, 0)]
    [TestCase("", null, 0, 0)]
    [TestCase("", "", 0, 0)]
    public void Equals_StringComparison(string? value, string? other, int index, int length)
    {
        var view1 = value.AsView(index, length);
        var view2 = other.AsView();

        var span1 = value.AsSpan(index, length);
        var span2 = other.AsSpan();

        Assert.That(
            view1.Equals(other, StringComparison.OrdinalIgnoreCase),
            Is.EqualTo(span1.Equals(span2, StringComparison.OrdinalIgnoreCase)));

        Assert.That(
            view1.Equals(view2, StringComparison.OrdinalIgnoreCase),
            Is.EqualTo(span1.Equals(span2, StringComparison.OrdinalIgnoreCase)));

        Assert.That(
            StringView.Equals(view1, other, StringComparison.OrdinalIgnoreCase),
            Is.EqualTo(span1.Equals(span2, StringComparison.OrdinalIgnoreCase)));

        Assert.That(
            StringView.Equals(view1, view2, StringComparison.OrdinalIgnoreCase),
            Is.EqualTo(span1.Equals(span2, StringComparison.OrdinalIgnoreCase)));
    }

    [TestCase("")]
    [TestCase("value")]
    [TestCase(null)]
    public void GetHashCode(string? value)
    {
        Assert.That(
            value.AsView().GetHashCode(),
            Is.EqualTo(string.GetHashCode(value.AsSpan())));

        Assert.That(
            (value?.ToUpper()).AsView().GetHashCode(StringComparison.OrdinalIgnoreCase),
            Is.EqualTo(string.GetHashCode(value.AsSpan(), StringComparison.OrdinalIgnoreCase)));
    }

    [TestCase("Abcd", "Abcd")]
    [TestCase("abcd", "abcd")]
    [TestCase("1234", "2345")]
    [TestCase("2345", "1234")]
    [TestCase("", "1234")]
    [TestCase("1234", "")]
    [TestCase("", null)]
    [TestCase(null, "")]
    [TestCase("ABCD", "abcd")]
    [TestCase("abcde", "abc")]
    [TestCase("abc", "abcde")]
    public void Compare(string? value, string? other)
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        var view1 = value.AsView();
        var view2 = other.AsView();

        var span1 = value.AsSpan();
        var span2 = other.AsSpan();

        Assert.That(
            StringView.Compare(view1, other, StringComparison.CurrentCulture),
            Is.EqualTo(span1.CompareTo(span2, StringComparison.CurrentCulture)));

        Assert.That(
            StringView.Compare(view1, view2, StringComparison.CurrentCulture),
            Is.EqualTo(span1.CompareTo(span2, StringComparison.CurrentCulture)));

        Assert.That(
            view1.CompareTo(view2),
            Is.EqualTo(span1.CompareTo(span2, StringComparison.CurrentCulture)));

        Assert.That(
            view1.CompareTo(other),
            Is.EqualTo(span1.CompareTo(span2, StringComparison.CurrentCulture)));

        Assert.That(
            view1.CompareTo(view2, StringComparison.OrdinalIgnoreCase),
            Is.EqualTo(span1.CompareTo(span2, StringComparison.OrdinalIgnoreCase)));

        Assert.That(
            view1.CompareTo(other, StringComparison.OrdinalIgnoreCase),
            Is.EqualTo(span1.CompareTo(span2, StringComparison.OrdinalIgnoreCase)));
    }

    [TestCase("Abcd", "Abcd")]
    [TestCase("abcd", "abcd")]
    [TestCase("1234", "2345")]
    [TestCase("2345", "1234")]
    [TestCase("", "1234")]
    [TestCase("1234", "")]
    [TestCase("", null)]
    [TestCase(null, "")]
    [TestCase("ABCD", "abcd")]
    [TestCase("abcde", "abc")]
    [TestCase("abc", "abcde")]
    public void CompareOrdinal(string? value, string? other)
    {
        var view1 = value.AsView();
        var view2 = other.AsView();

        var span1 = value.AsSpan();
        var span2 = other.AsSpan();

        Assert.That(StringView.CompareOrdinal(view1, other), Is.EqualTo(span1.CompareTo(span2, StringComparison.Ordinal)));
        Assert.That(StringView.CompareOrdinal(view1, view2), Is.EqualTo(span1.CompareTo(span2, StringComparison.Ordinal)));
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
        var view = source.AsView(index, length);

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
        var view = source.AsView(index, length);

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
        var view = source.AsView(index, length);

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
        var view = source.AsView(index, length);

        Assert.That(view.TryCopyTo(data.AsSpan()), Is.False);
        Assert.That(data, Is.EquivalentTo(new char[length - 1]));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("value")]
    [TestCase("key=value")]
    public void AsView(string? value)
    {
        Assert.That(
            value.AsView().ToString(),
            Is.EqualTo(value.AsSpan().ToString()));
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
        Assert.That(
            value.AsView(index).ToString(),
            Is.EqualTo(value.AsSpan(index).ToString()));
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
        Assert.That(
            value.AsView(index, length).ToString(),
            Is.EqualTo(value.AsSpan(index, length).ToString()));
    }

    [TestCase(null, 1)]
    [TestCase("", 1)]
    [TestCase("value", -1)]
    [TestCase("value", 6)]
    public void AsView_InvalidIndex_ShouldThrow(string? value, int index)
    {
        Assert.That(
            () => value.AsView(index),
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
        Assert.That(
            () => value.AsView(index, length),
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [TestCase("Hello", 'H', true)]
    [TestCase("Hello", 'h', false)]
    [TestCase("", 'A', false)]
    [TestCase(null, 'A', false)]
    public void StartsWith_Char(string? source, char needle, bool expected)
    {
        Assert.That(source.AsView().StartsWith(needle), Is.EqualTo(expected));
    }

    [TestCase("Hello", "H")]
    [TestCase("Hello", "h")]
    [TestCase("Hello", "O")]
    [TestCase("", "A")]
    [TestCase(null, "A")]
    [TestCase("", "")]
    [TestCase(null, "")]
    [TestCase(null, null)]
    [TestCase("", null)]
    public void StartsWith_Text(string? source, string? needle)
    {
        Assert.That(
            source.AsView().StartsWith(needle),
            Is.EqualTo(source.AsSpan().StartsWith(needle)));

        Assert.That(
            source.AsView().StartsWith(needle, StringComparison.OrdinalIgnoreCase),
            Is.EqualTo(source.AsSpan().StartsWith(needle, StringComparison.OrdinalIgnoreCase)));
    }

    [TestCase("Hello", 'o', true)]
    [TestCase("Hello", 'h', false)]
    [TestCase("", 'a', false)]
    [TestCase(null, 'a', false)]
    public void EndsWith_Char(string? source, char needle, bool expected)
    {
        Assert.That(
            source.AsView().EndsWith(needle),
            Is.EqualTo(expected));
    }

    [TestCase("Hello", "o")]
    [TestCase("Hello", "O")]
    [TestCase("Hello", "h")]
    [TestCase("", "A")]
    [TestCase(null, "A")]
    [TestCase("", "")]
    [TestCase(null, "")]
    [TestCase(null, null)]
    [TestCase("", null)]
    public void EndsWith_Text(string? source, string? needle)
    {
        Assert.That(
            source.AsView().EndsWith(needle),
            Is.EqualTo(source.AsSpan().EndsWith(needle)));

        Assert.That(
            source.AsView().EndsWith(needle, StringComparison.OrdinalIgnoreCase),
            Is.EqualTo(source.AsSpan().EndsWith(needle, StringComparison.OrdinalIgnoreCase)));
    }

    [TestCase("Hello, world!", 'H')]
    [TestCase("Hello, world!", 'd')]
    [TestCase("Hello, world!", 'o')]
    [TestCase("Hello, world!", '!')]
    [TestCase("Hello, world!", ',')]
    [TestCase("Hello, world!", 'A')]
    [TestCase("", 'A')]
    [TestCase(null, 'A')]
    public void IndexOf_Char(string? source, char needle)
    {
        Assert.That(
            source.AsView().IndexOf(needle),
            Is.EqualTo(source.AsSpan().IndexOf(needle)));
    }

    [TestCase("Hello, world!", "world")]
    [TestCase("Hello, world!", "World")]
    [TestCase("Hello, world!", "hello")]
    [TestCase("Hello, world!", "Hello")]
    [TestCase("Hello, world!", "w")]
    [TestCase("Hello, world!", ",")]
    [TestCase("Hello, world!", "?")]
    [TestCase("Hello, world!", "")]
    [TestCase("", "")]
    [TestCase(null, "")]
    public void IndexOf_Text(string? source, string needle)
    {
        Assert.That(
            source.AsView().IndexOf(needle),
            Is.EqualTo(source.AsSpan().IndexOf(needle)));

        Assert.That(
            source.AsView().IndexOf(needle, StringComparison.OrdinalIgnoreCase),
            Is.EqualTo(source.AsSpan().IndexOf(needle, StringComparison.OrdinalIgnoreCase)));
    }

    [TestCase("Hello, world!", 'H')]
    [TestCase("Hello, world!", 'd')]
    [TestCase("Hello, world!", 'o')]
    [TestCase("Hello, world!", '!')]
    [TestCase("Hello, world!", ',')]
    [TestCase("Hello, world!", 'A')]
    [TestCase("", 'A')]
    [TestCase(null, 'A')]
    public void LastIndexOf_Char(string? source, char needle)
    {
        Assert.That(
            source.AsView().LastIndexOf(needle),
            Is.EqualTo(source.AsSpan().LastIndexOf(needle)));
    }

    [TestCase("Hello, world!", "World")]
    [TestCase("Hello, world!", "hello")]
    [TestCase("Hello, world!", "?")]
    [TestCase("", "")]
    [TestCase(null, "")]
    public void LastIndexOf_Text(string? source, string needle)
    {
        Assert.That(
            source.AsView().LastIndexOf(needle),
            Is.EqualTo(source.AsSpan().LastIndexOf(needle)));

        Assert.That(
            source.AsView().LastIndexOf(needle, StringComparison.OrdinalIgnoreCase),
            Is.EqualTo(source.AsSpan().LastIndexOf(needle, StringComparison.OrdinalIgnoreCase)));
    }

    [TestCase("Hello, world!", 'H')]
    [TestCase("Hello, world!", '!')]
    [TestCase("Hello, world!", ',')]
    [TestCase("Hello, world!", 'A')]
    [TestCase("", 'A')]
    [TestCase(null, 'A')]
    public void Contains_Char(string? source, char needle)
    {
        Assert.That(
            source.AsView().Contains(needle),
            Is.EqualTo(source.AsSpan().Contains(needle)));
    }

    [TestCase("Hello, world!", "world")]
    [TestCase("Hello, world!", "World")]
    [TestCase("Hello, world!", "w")]
    [TestCase("Hello, world!", ",")]
    [TestCase("Hello, world!", "?")]
    [TestCase("Hello, world!", "")]
    [TestCase("", "")]
    [TestCase(null, "")]
    public void Contains_Text(string? source, string needle)
    {
        Assert.That(
            source.AsView().Contains(needle),
            Is.EqualTo(source.AsSpan().Contains(needle, StringComparison.Ordinal)));

        Assert.That(
            source.AsView().Contains(needle, StringComparison.OrdinalIgnoreCase),
            Is.EqualTo(source.AsSpan().Contains(needle, StringComparison.OrdinalIgnoreCase)));
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
            value.AsView(index, length).Length,
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
    public void ToString(string? value, int index, int length)
    {
        Assert.That(
            value.AsView(index, length).ToString(),
            Is.EqualTo(value.AsSpan(index, length).ToString()));
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
            value.AsView(index, length).ToArray(),
            Is.EquivalentTo(value.AsSpan(index, length).ToArray()));
    }

    [TestCase("", 0, 0)]
    [TestCase(null, 0, 0)]
    [TestCase("id;name;lastname", 3, 0)]
    [TestCase("id;name;lastname", 3, 4)]
    [TestCase("id;name;lastname", 0, 2)]
    [TestCase("id;name;lastname", 8, 8)]
    public void GetPinnableReference(string? value, int index, int length)
    {
        var view = value.AsView(index, length);
        var span = value.AsSpan(index, length);

        Assert.That(
            Unsafe.AreSame(
                ref Unsafe.AsRef(in view.GetPinnableReference()),
                ref Unsafe.AsRef(in span.GetPinnableReference())),
            Is.True);
    }

    [Test]
    public void GetPinnableReference_Default()
    {
        var view = default(StringView);
        view.GetPinnableReference();
    }

    [TestCase("value", "value", 0, 5, true)]
    [TestCase("key=value", "value", 4, 5, true)]
    [TestCase("key=value", "key", 0, 3, true)]
    [TestCase("key=value", "=", 3, 1, true)]
    [TestCase("key=value", ">", 3, 1, false)]
    [TestCase("value", "final", 0, 5, false)]
    [TestCase("key=value", "final", 0, 5, false)]
    [TestCase("key=value", "source", 0, 5, false)]
    [TestCase("key=value", "", 0, 0, true)]
    [TestCase("key=value", null, 0, 0, true)]
    [TestCase(null, null, 0, 0, true)]
    [TestCase(null, "", 0, 0, true)]
    [TestCase("", null, 0, 0, true)]
    [TestCase("", "", 0, 0, true)]
    public void Operator_Equals(string? source, string? value, int index, int length, bool expected)
    {
        Assert.That(source.AsView(index, length) == value, Is.EqualTo(expected));
        Assert.That(source.AsView(index, length) == value.AsView(), Is.EqualTo(expected));
    }

    [TestCase("value", "key", true)]
    [TestCase("key", "value", true)]
    [TestCase("key", "Key", true)]
    [TestCase("key", "key", false)]
    [TestCase("key", "", true)]
    [TestCase("key", null, true)]
    [TestCase("", "key", true)]
    [TestCase(null, "key", true)]
    public void Operator_NotEquals(string? source, string? value, bool expected)
    {
        Assert.That(source.AsView() != value, Is.EqualTo(expected));
        Assert.That(source.AsView() != value.AsView(), Is.EqualTo(expected));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("test")]
    public void Operator_Conversions(string? value)
    {
        StringView view = value;
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
}
