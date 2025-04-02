using System.Collections;
using Ramstack.Internal;

namespace Ramstack.Text;

/// <summary>
/// Represents a read-only view of a string, providing safe and efficient access to a subset of its characters.
/// </summary>
[DebuggerDisplay("{ToStringDebugger(),nq}")]
public readonly struct StringView : IReadOnlyList<char>, IComparable<StringView>, IComparable<string?>, IEquatable<StringView>, IEquatable<string?>
{
    private readonly string? _value;
    private readonly int _index;
    private readonly int _length;

    /// <summary>
    /// Gets the empty string view.
    /// </summary>
    public static StringView Empty => default;

    /// <summary>
    /// Gets the number of characters in the current <see cref="StringView"/> object.
    /// </summary>
    public int Length => _length;

    /// <summary>
    /// Gets a value indicating whether the <see cref="StringView"/> was declared but not initialized.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsDefault => _value is null;

    /// <inheritdoc cref="IReadOnlyList{T}.this"/>
    public char this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if ((uint)index >= (uint)_length)
                ThrowHelper.ThrowArgumentOutOfRangeException();

            return _value!.GetRawStringData(_index + index);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringView"/> structure that creates a view
    /// for the all characters in the specified string.
    /// </summary>
    /// <param name="value">The string to wrap.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StringView(string value) : this(value, 0, value.Length, dummy: 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringView"/> structure that creates
    /// a view for the specified range of the characters in the specified string.
    /// </summary>
    /// <param name="value">The string to wrap.</param>
    /// <param name="index">The zero-based index of the first character in the range.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StringView(string value, int index)
    {
        if ((uint)index > (uint)value.Length)
            ThrowHelper.ThrowArgumentOutOfRangeException();

        _index = index;
        _length = value.Length - index;
        _value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringView"/> structure that creates
    /// a view for the specified range of the characters in the specified string.
    /// </summary>
    /// <param name="value">The string to wrap.</param>
    /// <param name="index">The zero-based index of the first character in the range.</param>
    /// <param name="length">The number of characters in the range.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StringView(string value, int index, int length)
    {
        if (IntPtr.Size == 8)
        {
            if ((ulong)(uint)index + (uint)length > (uint)value.Length)
                ThrowHelper.ThrowArgumentOutOfRangeException();
        }
        else
        {
            if ((uint)index > (uint)value.Length || (uint)length > (uint)(value.Length - index))
                ThrowHelper.ThrowArgumentOutOfRangeException();
        }

        _index = index;
        _length = length;
        _value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringView"/> structure that creates
    /// a view for the specified range of the characters in the specified string.
    /// </summary>
    /// <param name="value">The string to wrap.</param>
    /// <param name="index">The zero-based index of the first character in the range.</param>
    /// <param name="length">The number of characters in the range.</param>
    /// <param name="dummy">The dummy parameter.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private StringView(string value, int index, int length, int dummy)
    {
        _index = index;
        _length = length;
        _value = value;
        _ = dummy;
    }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    public Enumerator GetEnumerator() =>
        new(this);

    /// <summary>
    /// Returns a substring from the current string view starting at the specified index.
    /// </summary>
    /// <remarks>
    /// This method is a synonym for the <see cref="Slice(int)"/> method.
    /// </remarks>
    /// <param name="start">The index at which to begin the substring.</param>
    /// <returns>
    /// A string view that consists of all characters of the current string view
    /// from <paramref name="start"/> to the end of the string view.
    /// </returns>
    public StringView Substring(int start) =>
        Slice(start);

    /// <summary>
    /// Returns a substring of the specified length out of the current string view starting at the specified index.
    /// </summary>
    /// <remarks>
    /// This method is a synonym for the <see cref="Slice(int, int)"/> method.
    /// </remarks>
    /// <param name="start">The index at which to begin the substring.</param>
    /// <param name="length">The length of the substring.</param>
    /// <returns>
    /// A string view of <paramref name="length"/> characters starting at <paramref name="start"/>.
    /// </returns>
    public StringView Substring(int start, int length) =>
        Slice(start, length);

    /// <summary>
    /// Forms a slice out of the current string view starting at the specified index.
    /// </summary>
    /// <param name="start">The index at which to begin the slice.</param>
    /// <returns>
    /// A string view that consists of all characters of the current string view
    /// from <paramref name="start"/> to the end of the string view.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StringView Slice(int start)
    {
        if ((uint)start > (uint)_length)
            ThrowHelper.ThrowArgumentOutOfRangeException();

        return new StringView(_value!, _index + start, _length - start, dummy: 0);
    }

    /// <summary>
    /// Forms a slice of the specified length out of the current string view starting at the specified index.
    /// </summary>
    /// <param name="start">The index at which to begin the slice.</param>
    /// <param name="length">The length of the slice.</param>
    /// <returns>
    /// A string view of <paramref name="length"/> characters starting at <paramref name="start"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StringView Slice(int start, int length)
    {
        if (IntPtr.Size == 8)
        {
            if ((ulong)(uint)start + (uint)length > (uint)_length)
                ThrowHelper.ThrowArgumentOutOfRangeException();
        }
        else
        {
            if ((uint)start > (uint)_length || (uint)length > (uint)(_length - start))
                ThrowHelper.ThrowArgumentOutOfRangeException();
        }

        return new StringView(_value!, _index + start, length, dummy: 0);
    }

    /// <summary>
    /// Determines whether this instance starts with the specified character.
    /// </summary>
    /// <param name="value">The character to compare.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="value"/> parameter matches the beginning of this instance; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool StartsWith(char value) =>
        _length != 0 && _value!.GetRawStringData(_index) == value;

    /// <summary>
    /// Determines whether the end of this instance matches the specified character.
    /// </summary>
    /// <param name="value">The character to compare.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="value"/> matches the end of this instance; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool EndsWith(char value) =>
        _length != 0 && _value!.GetRawStringData(_index + _length - 1) == value;

    /// <summary>
    /// Determines whether the beginning of this instance matches the specified value when compared using the specified comparison option.
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="comparisonType">One of the enumeration values that determines how this instance and <paramref name="value"/> are compared.
    /// The default value is <see cref="StringComparison.CurrentCulture"/>.</param>
    /// <returns>
    /// <see langword="true"/> if this instance begins with <paramref name="value"/> parameter; otherwise, <see langword="false"/>.
    /// </returns>
    public bool StartsWith(ReadOnlySpan<char> value, StringComparison comparisonType = StringComparison.CurrentCulture) =>
        AsSpan().StartsWith(value, comparisonType);

    /// <summary>
    /// Determines whether the end of this instance matches the specified value when compared using the specified comparison option.
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="comparisonType">One of the enumeration values that determines how this instance and <paramref name="value"/> are compared.
    /// The default value is <see cref="StringComparison.CurrentCulture"/>.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="value"/> parameter matches the end of this instance; otherwise, <see langword="false"/>.
    /// </returns>
    public bool EndsWith(ReadOnlySpan<char> value, StringComparison comparisonType = StringComparison.CurrentCulture) =>
        AsSpan().EndsWith(value, comparisonType);

    /// <summary>
    /// Removes all leading whitespace characters from the current instance.
    /// </summary>
    /// <returns>
    /// The trimmed <see cref="StringView"/>.
    /// </returns>
    public StringView TrimStart()
    {
        var start = _index;
        var final = _index + _length;
        var value = _value;

        if (value != null)
            for (; start < final; start++)
                if (!char.IsWhiteSpace(value.GetRawStringData(start)))
                    break;

        return new StringView(value!, start, final - start, dummy: 0);
    }

    /// <summary>
    /// Removes all leading occurrences of a specified character from the current instance.
    /// </summary>
    /// <param name="trimChar">The specified character to look for and remove.</param>
    /// <returns>
    /// The trimmed <see cref="StringView"/>.
    /// </returns>
    public StringView TrimStart(char trimChar)
    {
        var start = _index;
        var final = _index + _length;
        var value = _value;

        if (value != null)
            for (; start < final; start++)
                if (value.GetRawStringData(start) != trimChar)
                    break;

        return new StringView(value!, start, final - start, dummy: 0);
    }

    /// <summary>
    /// Removes all leading occurrences of a set of characters specified in an array from the current instance.
    /// </summary>
    /// <remarks>
    /// If <paramref name="trimChars"/> is <c>null</c> or an empty array, whitespace characters are removed instead.
    /// </remarks>
    /// <param name="trimChars">An array which contains a set of characters to remove.</param>
    /// <returns>
    /// The trimmed <see cref="StringView"/>.
    /// </returns>
    public StringView TrimStart(params char[]? trimChars) =>
        TrimStart(trimChars.AsSpan());

    /// <summary>
    /// Removes all leading occurrences of a set of characters specified in a read-only span from the current instance.
    /// </summary>
    /// <remarks>
    /// If <paramref name="trimChars"/> is empty, whitespace characters are removed instead.
    /// </remarks>
    /// <param name="trimChars">A read-only span which contains s set of characters to remove.</param>
    /// <returns>
    /// The trimmed <see cref="StringView"/>.
    /// </returns>
    public StringView TrimStart(ReadOnlySpan<char> trimChars)
    {
        if (trimChars.Length == 0)
            return TrimStart();

        var start = _index;
        var final = _index + _length;
        var value = _value;

        if (value != null)
        {
            for (; start < final; start++)
            {
                for (var i = 0; i < trimChars.Length; i++)
                    if (value.GetRawStringData(start) == trimChars[i])
                        goto MATCHED;

                break;
                MATCHED: ;
            }
        }

        return new StringView(value!, start, final - start, dummy: 0);
    }

    /// <summary>
    /// Removes all trailing whitespace characters from the current instance.
    /// </summary>
    /// <returns>
    /// The trimmed <see cref="StringView"/>.
    /// </returns>
    public StringView TrimEnd()
    {
        var start = _index;
        var final = _index + _length - 1;
        var value = _value;

        if (value != null)
            for (; final >= start; final--)
                if (!char.IsWhiteSpace(value.GetRawStringData(final)))
                    break;

        return new StringView(value!, start, final + 1 - start, dummy: 0);
    }

    /// <summary>
    /// Removes all trailing occurrences of a specified character from the current instance.
    /// </summary>
    /// <param name="trimChar">The specified character to look for and remove.</param>
    /// <returns>
    /// The trimmed <see cref="StringView"/>.
    /// </returns>
    public StringView TrimEnd(char trimChar)
    {
        var start = _index;
        var final = _index + _length - 1;
        var value = _value;

        if (value != null)
            for (; final >= start; final--)
                if (value.GetRawStringData(final) != trimChar)
                    break;

        return new StringView(value!, start, final + 1 - start, dummy: 0);
    }

    /// <summary>
    /// Removes all trailing occurrences of a set of characters specified in an array from the current instance.
    /// </summary>
    /// <remarks>
    /// If <paramref name="trimChars"/> is <c>null</c> or an empty array, whitespace characters are removed instead.
    /// </remarks>
    /// <param name="trimChars">An array which contains a set of characters to remove.</param>
    /// <returns>
    /// The trimmed <see cref="StringView"/>.
    /// </returns>
    public StringView TrimEnd(params char[]? trimChars) =>
        TrimEnd(trimChars.AsSpan());

    /// <summary>
    /// Removes all leading occurrences of a set of characters specified in a read-only span from the current instance.
    /// </summary>
    /// <remarks>
    /// If <paramref name="trimChars"/> is empty, whitespace characters are removed instead.
    /// </remarks>
    /// <param name="trimChars">A read-only span which contains a set of characters to remove.</param>
    /// <returns>
    /// The trimmed <see cref="StringView"/>.
    /// </returns>
    public StringView TrimEnd(ReadOnlySpan<char> trimChars)
    {
        if (trimChars.Length == 0)
            return TrimEnd();

        var start = _index;
        var final = _index + _length - 1;
        var value = _value;

        if (value != null)
        {
            for (; final >= start; final--)
            {
                for (var i = 0; i < trimChars.Length; i++)
                    if (value.GetRawStringData(final) == trimChars[i])
                        goto MATCHED;

                break;
                MATCHED: ;
            }
        }

        return new StringView(value!, start, final + 1 - start, dummy: 0);
    }

    /// <summary>
    /// Removes all leading and trailing whitespace characters from the current instance.
    /// </summary>
    /// <returns>
    /// The trimmed <see cref="StringView"/>.
    /// </returns>
    public StringView Trim()
    {
        var start = _index;
        var final = _index + _length - 1;
        var value = _value;

        if (value != null)
        {
            for (; start <= final; start++)
                if (!char.IsWhiteSpace(value.GetRawStringData(start)))
                    break;

            for (; final > start; final--)
                if (!char.IsWhiteSpace(value.GetRawStringData(final)))
                    break;
        }

        return new StringView(value!, start, final + 1 - start, dummy: 0);
    }

    /// <summary>
    /// Removes all leading and trailing occurrences of a specified character from the current instance.
    /// </summary>
    /// <param name="trimChar">The specified character to look for and remove.</param>
    /// <returns>
    /// The trimmed <see cref="StringView"/>.
    /// </returns>
    public StringView Trim(char trimChar)
    {
        var start = _index;
        var final = _index + _length - 1;
        var value = _value;

        if (value != null)
        {
            for (; start <= final; start++)
                if (value.GetRawStringData(start) != trimChar)
                    break;

            for (; final > start; final--)
                if (value.GetRawStringData(final) != trimChar)
                    break;
        }

        return new StringView(value!, start, final + 1 - start, dummy: 0);
    }

    /// <summary>
    /// Removes all leading and trailing occurrences of a set of characters specified in an array from the current instance.
    /// </summary>
    /// <remarks>
    /// If <paramref name="trimChars"/> is <c>null</c> or an empty array, whitespace characters are removed instead.
    /// </remarks>
    /// <param name="trimChars">An array which contains a set of characters to remove.</param>
    /// <returns>
    /// The trimmed <see cref="StringView"/>.
    /// </returns>
    public StringView Trim(params char[]? trimChars) =>
        Trim(trimChars.AsSpan());

    /// <summary>
    /// Removes all leading and trailing occurrences of a set of characters specified in a read-only span from the current instance.
    /// </summary>
    /// <remarks>
    /// If <paramref name="trimChars"/> is empty, whitespace characters are removed instead.
    /// </remarks>
    /// <param name="trimChars">A read-only span which contains a set of characters to remove.</param>
    /// <returns>
    /// The trimmed <see cref="StringView"/>.
    /// </returns>
    public StringView Trim(ReadOnlySpan<char> trimChars)
    {
        if (trimChars.Length == 0)
            return Trim();

        var start = _index;
        var final = _index + _length - 1;
        var value = _value;

        if (value != null)
        {
            for (; start <= final; start++)
            {

                for (var i = 0; i < trimChars.Length; i++)
                    if (value.GetRawStringData(start) == trimChars[i])
                        goto MATCHED;

                break;
                MATCHED: ;
            }

            for (; final > start; final--)
            {
                for (var i = 0; i < trimChars.Length; i++)
                    if (value.GetRawStringData(final) == trimChars[i])
                        goto MATCHED;

                break;
                MATCHED: ;
            }
        }

        return new StringView(value!, start, final + 1 - start, dummy: 0);
    }

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified Unicode character in this instance.
    /// </summary>
    /// <param name="value">A Unicode character to seek.</param>
    /// <returns>
    /// The zero-based index position of <paramref name="value"/> if that character is found, or -1 if it is not.
    /// </returns>
    public int IndexOf(char value) =>
        AsSpan().IndexOf(value);

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified character sequence in this instance.
    /// </summary>
    /// <param name="value">The sequence of characters to search.</param>
    /// <returns>
    /// The zero-based index position of <paramref name="value"/> if that sequence is found, or -1 if it is not.
    /// </returns>
    public int IndexOf(ReadOnlySpan<char> value) =>
        AsSpan().IndexOf(value);

    /// <summary>
    /// Reports the zero-based index of the last occurrence of the specified character sequence in this instance.
    /// A parameter specifies the type of search to use for the specified character sequence.
    /// </summary>
    /// <param name="value">The sequence of characters to search.</param>
    /// <param name="comparisonType">An enumeration value that specifies the rules for the search.</param>
    /// <returns>
    /// The zero-based index position of <paramref name="value"/> if that sequence is found, or -1 if it is not.
    /// </returns>
    public int IndexOf(ReadOnlySpan<char> value, StringComparison comparisonType) =>
        AsSpan().IndexOf(value, comparisonType);

    /// <summary>
    /// Reports the zero-based index of the last occurrence of the specified Unicode character in this instance.
    /// </summary>
    /// <param name="value">A Unicode character to seek.</param>
    /// <returns>
    /// The zero-based index position of <paramref name="value"/> if that character is found, or -1 if it is not.
    /// </returns>
    public int LastIndexOf(char value) =>
        AsSpan().LastIndexOf(value);

    /// <summary>
    /// Reports the zero-based index of the last occurrence of the specified character sequence in this instance.
    /// </summary>
    /// <param name="value">The sequence of characters to search.</param>
    /// <returns>
    /// The zero-based index position of <paramref name="value"/> if that sequence is found, or -1 if it is not.
    /// </returns>
    public int LastIndexOf(ReadOnlySpan<char> value) =>
        AsSpan().LastIndexOf(value);

    /// <summary>
    /// Reports the zero-based index of the last occurrence of the specified character sequence in this instance.
    /// A parameter specifies the type of search to use for the specified character sequence.
    /// </summary>
    /// <param name="value">The sequence of characters to search.</param>
    /// <param name="comparisonType">An enumeration value that specifies the rules for the search.</param>
    /// <returns>
    /// The zero-based index position of <paramref name="value"/> if that sequence is found, or -1 if it is not.
    /// </returns>
    public int LastIndexOf(ReadOnlySpan<char> value, StringComparison comparisonType) =>
        AsSpan().LastIndexOf(value, comparisonType);

    /// <summary>
    /// Returns a value indicating whether a specified character occurs within this instance.
    /// </summary>
    /// <param name="value">A Unicode character to seek.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="value"/> parameter occurs within this instance; otherwise, <see langword="false"/>.
    /// </returns>
    public bool Contains(char value) =>
        AsSpan().Contains(value);

    /// <summary>
    /// Returns a value indicating whether a specified character sequence occurs within this instance.
    /// </summary>
    /// <param name="value">The sequence of characters to seek.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="value"/> parameter occurs within this instance; otherwise, <see langword="false"/>.
    /// </returns>
    public bool Contains(ReadOnlySpan<char> value) =>
        AsSpan().IndexOf(value) >= 0;

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified character sequence in this instance, using the specified comparison rules.
    /// </summary>
    /// <param name="value">The sequence of characters to search.</param>
    /// <param name="comparisonType">An enumeration value that specifies the rules for the search.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="value"/> parameter occurs within this instance; otherwise, <see langword="false"/>.
    /// </returns>
    public bool Contains(ReadOnlySpan<char> value, StringComparison comparisonType) =>
        AsSpan().Contains(value, comparisonType);

    /// <inheritdoc />
    public int CompareTo(StringView other) =>
        Compare(this, other, StringComparison.CurrentCulture);

    /// <summary>
    /// Compares one view with another using a specified string comparison,
    /// and returns an integer that indicates their relative position in the sort order.
    /// </summary>
    /// <param name="other">The view to compare with the current instance.</param>
    /// <param name="comparisonType">An enumeration value that determines how current view and other are compared.</param>
    /// <returns>
    /// A signed integer that indicates the relative order of view and other:
    ///     - If less than 0, this instance precedes than <paramref name="other"/>.
    ///     - If 0, this instance equals <paramref name="other"/>.
    ///     - If greater than 0, this instance follows <paramref name="other"/>.
    /// </returns>
    public int CompareTo(StringView other, StringComparison comparisonType) =>
        Compare(this, other, comparisonType);

    /// <inheritdoc />
    public int CompareTo(string? other) =>
        Compare(this, other, StringComparison.CurrentCulture);

    /// <summary>
    /// Compares one view with another using a specified string comparison,
    /// and returns an integer that indicates their relative position in the sort order.
    /// </summary>
    /// <param name="other">The view to compare with the current instance.</param>
    /// <param name="comparisonType">An enumeration value that determines how current view and other are compared.</param>
    /// <returns>
    /// A signed integer that indicates the relative order of view and other:
    ///     - If less than 0, this instance precedes than <paramref name="other"/>.
    ///     - If 0, this instance equals <paramref name="other"/>.
    ///     - If greater than 0, this instance follows <paramref name="other"/>.
    /// </returns>
    public int CompareTo(string? other, StringComparison comparisonType) =>
        Compare(this, other, comparisonType);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null)
            return _length == 0;

        if (obj is StringView)
            return Equals(this, Unsafe.Unbox<StringView>(obj));

        return obj is string s && Equals(s);
    }

    /// <inheritdoc />
    public bool Equals(StringView other) =>
        Equals(this, other);

    /// <inheritdoc />
    public bool Equals(string? other) =>
        Equals(this, other);

    /// <summary>
    /// Determines whether this view and the specified other view have the same characters when compared using the specified <paramref name="comparisonType"/> option.
    /// </summary>
    /// <param name="other">The value to compare with the current instance.</param>
    /// <param name="comparisonType">An enumeration value that determines how current instance and <paramref name="other"/> are compared.</param>
    /// <returns>
    /// true if equal, false otherwise.
    /// </returns>
    public bool Equals(StringView other, StringComparison comparisonType) =>
        Equals(this, other, comparisonType);

    /// <summary>
    /// Determines whether this view and the specified other view have the same characters when compared using the specified <paramref name="comparisonType"/> option.
    /// </summary>
    /// <param name="other">The value to compare with the current instance.</param>
    /// <param name="comparisonType">An enumeration value that determines how current instance and <paramref name="other"/> are compared.</param>
    /// <returns>
    /// true if equal, false otherwise.
    /// </returns>
    public bool Equals(string? other, StringComparison comparisonType) =>
        Equals(this, other, comparisonType);

    /// <inheritdoc />
    public override int GetHashCode() =>
        string.GetHashCode(AsSpan());

    /// <summary>
    /// Returns the hash code for this instance using the specified rules.
    /// </summary>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules to use in the comparison.</param>
    /// <returns>
    /// A 32-bit signed integer hash code.
    /// </returns>
    public int GetHashCode(StringComparison comparisonType) =>
        string.GetHashCode(AsSpan(), comparisonType);

    /// <summary>
    /// Copies the contents of this string view into a new string.
    /// </summary>
    /// <returns>
    /// A string containing the data in the current string view.
    /// </returns>
    public override string ToString()
    {
        var value = _value ?? "";
        if (_length == value.Length && _index == 0)
            return value;

        return new string(
            MemoryMarshal.CreateReadOnlySpan(
                ref Unsafe.AsRef(in value.GetRawStringData(_index)),
                _length));
    }

    /// <summary>
    /// Copies the contents of this string view into a new array.
    /// </summary>
    /// <returns>
    /// An array containing the data in the current string view.
    /// </returns>
    public char[] ToArray() =>
        ((ReadOnlySpan<char>)this).ToArray();

    /// <summary>
    /// Returns a read-only span over the current string view.
    /// </summary>
    /// <returns>
    /// The read-only span representation of the string view.
    /// </returns>
    public ReadOnlySpan<char> AsSpan() =>
        this;

    /// <summary>
    /// Returns a read-only memory region over the current string view.
    /// </summary>
    /// <returns>
    /// The read-only memory region over the string view.
    /// </returns>
    public ReadOnlyMemory<char> AsMemory() =>
        this;

    /// <summary>
    /// Returns a reference to the element of the <see cref="StringView"/> at index zero.
    /// </summary>
    /// <returns>
    /// A reference to the element of the <see cref="StringView"/> at index zero, or <see langword="null"/> if <see cref="IsDefault"/> is true.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref readonly char GetPinnableReference()
    {
        // To match the behavior of ReadOnlySpan<T>

        ref readonly var p = ref Unsafe.NullRef<char>();

        if (_length != 0)
            p = ref _value!.GetRawStringData(_index);

        return ref p;
    }

    /// <summary>
    /// Copies the contents of this <see cref="StringView"/> into a destination <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="destination">The span to copy items into.</param>
    public void CopyTo(Span<char> destination) =>
        AsSpan().CopyTo(destination);

    /// <summary>
    /// Attempts to copy the contents of this <see cref="StringView"/> into a destination <see cref="Span{T}"/>
    /// and returns a value to indicate whether the operation succeeded.
    /// </summary>
    /// <param name="destination">The span to copy items into.</param>
    /// <returns>
    /// <see langword="true"/> if the copy operation succeeded; otherwise, <see langword="false"/>.
    /// </returns>
    public bool TryCopyTo(Span<char> destination) =>
        AsSpan().TryCopyTo(destination);

    /// <summary>
    /// Determines whether two specified <see cref="StringView"/> objects have the same value.
    /// </summary>
    /// <param name="a">The first value to compare.</param>
    /// <param name="b">The second value to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the value of <paramref name="a"/> is the same as the value of <paramref name="b"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool Equals(StringView a, StringView b) =>
        a.AsSpan().SequenceEqual(b);

    /// <summary>
    /// Determines whether two specified <see cref="StringView"/> objects have the same value.
    /// </summary>
    /// <param name="a">The first value to compare.</param>
    /// <param name="b">The second value to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the value of <paramref name="a"/> is the same as the value of <paramref name="b"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool Equals(StringView a, string? b) =>
        a.AsSpan().SequenceEqual(b);

    /// <summary>
    /// Determines whether two specified <see cref="StringView"/> objects have the same value
    /// with the specified comparison rules.
    /// </summary>
    /// <param name="a">The first value to compare.</param>
    /// <param name="b">The second value to compare.</param>
    /// <param name="comparison">An enumeration value that determines how values are compared.</param>
    /// <returns>
    /// <see langword="true"/> if the value of the <paramref name="a"/> parameter is equal to the value of <paramref name="b"/> parameter;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool Equals(StringView a, StringView b, StringComparison comparison) =>
        a.AsSpan().Equals(b, comparison);

    /// <summary>
    /// Determines whether two specified <see cref="StringView"/> objects have the same value
    /// with the specified comparison rules.
    /// </summary>
    /// <param name="a">The first value to compare.</param>
    /// <param name="b">The second value to compare.</param>
    /// <param name="comparison">An enumeration value that determines how values are compared.</param>
    /// <returns>
    /// <see langword="true"/> if the value of the <paramref name="a"/> parameter is equal to the value of <paramref name="b"/> parameter;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool Equals(StringView a, string? b, StringComparison comparison) =>
        a.AsSpan().Equals(b, comparison);

    /// <summary>
    /// Compares two specified <see cref="StringView"/> objects and returns an integer that indicates their relative position in the sort order.
    /// </summary>
    /// <param name="a">The first value to compare.</param>
    /// <param name="b">The second value to compare.</param>
    /// <returns>
    /// A 32-bit signed integer that indicates the lexical relationship between the two comparands.
    /// A signed integer that indicates the relative order of view and other:
    ///     - If less than 0, <paramref name="a"/>> precedes <paramref name="b"/> in the sort order.
    ///     - If 0, <paramref name="a"/>> equals <paramref name="b"/>.
    ///     - If greater than 0, <paramref name="a"/>> follows <paramref name="b"/> in the sort order.
    /// </returns>
    public static int CompareOrdinal(StringView a, StringView b) =>
        a.AsSpan().SequenceCompareTo(b);

    /// <summary>
    /// Compares two specified <see cref="StringView"/> objects and returns an integer that indicates their relative position in the sort order.
    /// </summary>
    /// <param name="a">The first value to compare.</param>
    /// <param name="b">The second value to compare.</param>
    /// <returns>
    /// A 32-bit signed integer that indicates the lexical relationship between the two comparands.
    /// A signed integer that indicates the relative order of view and other:
    ///     - If less than 0, <paramref name="a"/>> precedes <paramref name="b"/> in the sort order.
    ///     - If 0, <paramref name="a"/>> equals <paramref name="b"/>.
    ///     - If greater than 0, <paramref name="a"/>> follows <paramref name="b"/> in the sort order.
    /// </returns>
    public static int CompareOrdinal(StringView a, string? b) =>
        a.AsSpan().SequenceCompareTo(b);

    /// <summary>
    /// Compares two specified <see cref="StringView"/> objects and returns an integer that indicates their relative position in the sort order.
    /// </summary>
    /// <param name="a">The first value to compare.</param>
    /// <param name="b">The second value to compare.</param>
    /// <param name="comparison">One of the enumeration values that specifies the rules to use in the comparison.</param>
    /// <returns>
    /// A 32-bit signed integer that indicates the lexical relationship between the two comparands.
    /// A signed integer that indicates the relative order of view and other:
    ///     - If less than 0, <paramref name="a"/>> precedes <paramref name="b"/> in the sort order.
    ///     - If 0, <paramref name="a"/>> equals <paramref name="b"/>.
    ///     - If greater than 0, <paramref name="a"/>> follows <paramref name="b"/> in the sort order.
    /// </returns>
    public static int Compare(StringView a, StringView b, StringComparison comparison) =>
        a.AsSpan().CompareTo(b, comparison);

    /// <summary>
    /// Compares two specified <see cref="StringView"/> objects and returns an integer that indicates their relative position in the sort order.
    /// </summary>
    /// <param name="a">The first value to compare.</param>
    /// <param name="b">The second value to compare.</param>
    /// <param name="comparison">One of the enumeration values that specifies the rules to use in the comparison.</param>
    /// <returns>
    /// A 32-bit signed integer that indicates the lexical relationship between the two comparands.
    /// A signed integer that indicates the relative order of view and other:
    ///     - If less than 0, <paramref name="a"/>> precedes <paramref name="b"/> in the sort order.
    ///     - If 0, <paramref name="a"/>> equals <paramref name="b"/>.
    ///     - If greater than 0, <paramref name="a"/>> follows <paramref name="b"/> in the sort order.
    /// </returns>
    public static int Compare(StringView a, string? b, StringComparison comparison) =>
        a.AsSpan().CompareTo(b, comparison);

    /// <summary>
    /// Determines whether two <see cref="StringView"/> instances are equal.
    /// </summary>
    /// <param name="a">The first value to compare.</param>
    /// <param name="b">The second value to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the two <see cref="StringView"/> instances are equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(StringView a, StringView b) =>
        Equals(a, b);

    /// <summary>
    /// Determines whether two <see cref="StringView"/> instances are not equal.
    /// </summary>
    /// <param name="a">The first value to compare.</param>
    /// <param name="b">The second value to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the two <see cref="StringView"/> instances are not equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(StringView a, StringView b) =>
        !Equals(a, b);

    /// <summary>
    /// Determines whether two <see cref="StringView"/> instances are equal.
    /// </summary>
    /// <param name="a">The first value to compare.</param>
    /// <param name="b">The second value to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the two <see cref="StringView"/> instances are equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(StringView a, string? b) =>
        Equals(a, b);

    /// <summary>
    /// Determines whether two <see cref="StringView"/> instances are not equal.
    /// </summary>
    /// <param name="a">The first value to compare.</param>
    /// <param name="b">The second value to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the two <see cref="StringView"/> instances are not equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(StringView a, string? b) =>
        !Equals(a, b);

    /// <summary>
    /// Defines an implicit conversion of a string view.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>
    /// A string view representation of the string.
    /// </returns>
    public static implicit operator StringView(string? value) =>
        new(value ?? "");

    /// <summary>
    /// Defines an implicit conversion of the <see cref="StringView"/> to the <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    /// <param name="view">The <see cref="StringView"/> to convert.</param>
    /// <returns>
    /// A <see cref="ReadOnlyMemory{T}"/> representation of the <see cref="StringView"/>.
    /// </returns>
    public static implicit operator ReadOnlyMemory<char>(StringView view) =>
        view._value.AsMemory(view._index, view._length);

    /// <summary>
    /// Defines an implicit conversion of a string view to the <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="view">The string view to convert.</param>
    /// <returns>
    /// A <see cref="ReadOnlySpan{T}"/> representation of the string view.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<char>(StringView view)
    {
        var value = view._value ?? "";

        return MemoryMarshal.CreateReadOnlySpan(
            ref Unsafe.AsRef(in value.GetRawStringData(view._index)),
            view._length);
    }

    /// <summary>
    /// Returns a string representation of the current instance's state, intended for debugging purposes.
    /// </summary>
    /// <returns>
    /// A string containing information about the current instance.
    /// </returns>
    private string ToStringDebugger() =>
        _value is null ? "Uninitialized" : ToString();

    #region IEnumerable<char> implementation

    IEnumerator<char> IEnumerable<char>.GetEnumerator() =>
        new StringViewEnumerator(this);

    IEnumerator IEnumerable.GetEnumerator() =>
        new StringViewEnumerator(this);

    #endregion

    #region IReadOnlyCollection<char> implementation

    int IReadOnlyCollection<char>.Count => _length;

    #endregion

    #region IReadOnlyList<char> implementation

    char IReadOnlyList<char>.this[int index] => this[index];

    #endregion

    #region Inner type: Enumerator

    /// <summary>
    /// Provides the ability to iterate through the characters of the <see cref="StringView"/>.
    /// </summary>
    public struct Enumerator
    {
        private readonly string? _value;
        private int _index;
        private readonly int _final;

        /// <inheritdoc cref="IEnumerator{T}.Current" />
        public readonly char Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if ((uint)_index >= (uint)_final)
                    ThrowHelper.ThrowArgumentOutOfRangeException();

                return _value!.GetRawStringData(_index);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Enumerator"/> structure.
        /// </summary>
        /// <param name="view">The <see cref="StringView"/> to iterate through its characters.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(StringView view)
        {
            _value = view._value;
            _index = view._index - 1;
            _final = view._index + view._length;
        }

        /// <inheritdoc cref="IEnumerator.MoveNext" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            ++_index;
            return (uint)_index < (uint)_final;
        }
    }

    #endregion

    #region Inner type: StringViewEnumerator

    /// <summary>
    /// Provides the ability to iterate through the characters of the <see cref="StringView"/>.
    /// </summary>
    private sealed class StringViewEnumerator : IEnumerator<char>
    {
        private readonly string? _value;
        private int _index;
        private readonly int _final;

        /// <inheritdoc />
        public char Current
        {
            get
            {
                if ((uint)_index >= (uint)_final)
                    ThrowHelper.ThrowArgumentOutOfRangeException();

                return _value!.GetRawStringData(_index);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringViewEnumerator"/> class.
        /// </summary>
        /// <param name="view">The <see cref="StringView"/> to iterate through its characters.</param>
        public StringViewEnumerator(StringView view)
        {
            _value = view._value;
            _index = view._index - 1;
            _final = view._index + view._length;
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
        object IEnumerator.Current => Current;

        /// <inheritdoc />
        void IEnumerator.Reset() =>
            ThrowHelper.ThrowNotSupportedException();
    }

    #endregion
}
