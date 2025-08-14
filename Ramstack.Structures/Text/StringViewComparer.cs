namespace Ramstack.Text;

/// <summary>
/// Represents a <see cref="StringView"/> comparison operation
/// that uses specific case and culture-based or ordinal comparison rules.
/// </summary>
public abstract class StringViewComparer : IEqualityComparer<StringView>, IComparer<StringView>
{
    /// <summary>
    /// Gets a <see cref="StringViewComparer"/> object that performs a case-sensitive ordinal string comparison.
    /// </summary>
    public static StringViewComparer Ordinal => OrdinalComparer.Instance;

    /// <summary>
    /// Gets a <see cref="StringViewComparer"/> object that performs a case-insensitive ordinal string comparison.
    /// </summary>
    public static StringViewComparer OrdinalIgnoreCase => OrdinalIgnoreCaseComparer.Instance;

    /// <summary>
    /// Gets a <see cref="StringViewComparer"/> object that performs case-insensitive string comparisons
    /// using the word comparison rules of the current culture.
    /// </summary>
    public static StringViewComparer CurrentCulture => new CultureAwareComparer(
        CultureInfo.CurrentCulture.CompareInfo, CompareOptions.None);

    /// <summary>
    /// Gets a <see cref="StringViewComparer"/> object that performs case-insensitive string comparisons
    /// using the word comparison rules of the current culture.
    /// </summary>
    public static StringViewComparer CurrentCultureIgnoreCase => new CultureAwareComparer(
        CultureInfo.CurrentCulture.CompareInfo, CompareOptions.IgnoreCase);

    /// <summary>
    /// Gets a <see cref="StringViewComparer"/> object that performs a case-sensitive string comparison
    /// using the word comparison rules of the invariant culture.
    /// </summary>
    public static StringViewComparer InvariantCulture => CultureAwareComparer.InvariantInstance;

    /// <summary>
    /// Gets a <see cref="StringViewComparer"/> object that performs a case-insensitive string comparison
    /// using the word comparison rules of the invariant culture.
    /// </summary>
    public static StringViewComparer InvariantCultureIgnoreCase => CultureAwareComparer.InvariantIgnoreCaseInstance;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringViewComparer"/> class.
    /// </summary>
    private protected StringViewComparer()
    {
    }

    /// <inheritdoc />
    public abstract bool Equals(StringView x, StringView y);

    /// <inheritdoc />
    public abstract int Compare(StringView x, StringView y);

    /// <inheritdoc />
    public abstract int GetHashCode(StringView obj);

    /// <summary>
    /// Creates a <see cref="StringViewComparer"/> that compares <see cref="StringView"/>
    /// based on the rules of a specified culture.
    /// </summary>
    /// <param name="culture">The culture whose linguistic rules are used for the string comparison.</param>
    /// <param name="ignoreCase"><see langword="true"/> for case-insensitive comparison;
    /// <see langword="false"/> for case-sensitive comparison.</param>
    /// <returns>
    /// A new <see cref="StringViewComparer"/> object that compares strings according
    /// to the rules of the specified culture and case sensitivity.
    /// </returns>
    public static StringViewComparer Create(CultureInfo culture, bool ignoreCase) =>
        Create(culture, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);

    /// <summary>
    /// Creates a <see cref="StringViewComparer"/> object that compares <see cref="StringView"/>
    /// based on the rules of a specified culture and comparison options.
    /// </summary>
    /// <param name="culture">The culture whose linguistic rules are used for the string comparison.</param>
    /// <param name="options">A combination of <see cref="CompareOptions"/> values
    /// that specify the comparison options.</param>
    /// <returns>
    /// A new <see cref="StringViewComparer"/> object that compares strings
    /// according to the rules of the specified culture and options.
    /// </returns>
    public static StringViewComparer Create(CultureInfo culture, CompareOptions options) =>
        new CultureAwareComparer(culture.CompareInfo, options);

    /// <summary>
    /// Creates a <see cref="StringViewComparer"/> instance from the specified <see cref="StringComparison"/> value.
    /// </summary>
    /// <param name="comparisonType">The <see cref="StringComparison"/> instance to convert.</param>
    /// <returns>
    /// A <see cref="StringViewComparer"/> instance that represents
    /// the equivalent value of the specified <see cref="StringComparison"/> instance.
    /// </returns>
    public static StringViewComparer FromComparison(StringComparison comparisonType)
    {
        return comparisonType switch
        {
            StringComparison.CurrentCulture => CurrentCulture,
            StringComparison.CurrentCultureIgnoreCase => CurrentCultureIgnoreCase,
            StringComparison.InvariantCulture => InvariantCulture,
            StringComparison.InvariantCultureIgnoreCase => InvariantCultureIgnoreCase,
            StringComparison.Ordinal => Ordinal,
            StringComparison.OrdinalIgnoreCase => OrdinalIgnoreCase,
            _ => Error(comparisonType)
        };

        static StringViewComparer Error(StringComparison comparisonType) =>
            throw new ArgumentOutOfRangeException(nameof(comparisonType), comparisonType, "The specified string comparison is not supported");
    }

    #region Inner type: CultureAwareComparer

    /// <summary>
    /// Represents a comparer that performs culture-sensitive string comparisons.
    /// </summary>
    /// <param name="info">An instance of <see cref="CompareInfo"/> that provides
    /// culture-specific comparison information.</param>
    /// <param name="options">A bitwise combination of <see cref="CompareOptions"/> values
    /// that specify how the comparison should be performed.</param>
    private sealed class CultureAwareComparer(CompareInfo info, CompareOptions options) : StringViewComparer
        #if NET9_0_OR_GREATER
        , IAlternateEqualityComparer<ReadOnlySpan<char>, StringView>
        #endif
    {
        /// <summary>
        /// A singleton instance of the <see cref="StringViewComparer.CultureAwareComparer"/> class
        /// for invariant case-sensitive comparison.
        /// </summary>
        public static readonly CultureAwareComparer InvariantInstance =
            new(CultureInfo.InvariantCulture.CompareInfo, CompareOptions.None);

        /// <summary>
        /// A singleton instance of the <see cref="StringViewComparer.CultureAwareComparer"/> class
        /// for invariant case-insensitive comparison.
        /// </summary>
        public static readonly CultureAwareComparer InvariantIgnoreCaseInstance =
            new(CultureInfo.InvariantCulture.CompareInfo, CompareOptions.IgnoreCase);

        /// <inheritdoc />
        public override bool Equals(StringView x, StringView y) =>
            info.IndexOf(x, y, options) == 0;

        /// <inheritdoc />
        public override int GetHashCode(StringView obj) =>
            info.GetHashCode(obj, options);

        /// <inheritdoc />
        public override int Compare(StringView x, StringView y) =>
            info.IndexOf(x, y, options);

        #if NET9_0_OR_GREATER
        /// <inheritdoc />
        bool IAlternateEqualityComparer<ReadOnlySpan<char>, StringView>.Equals(ReadOnlySpan<char> alternate, StringView other) =>
            info.IndexOf(alternate, other, options) == 0;

        /// <inheritdoc />
        int IAlternateEqualityComparer<ReadOnlySpan<char>, StringView>.GetHashCode(ReadOnlySpan<char> alternate) =>
            info.GetHashCode(alternate, options);

        /// <inheritdoc />
        StringView IAlternateEqualityComparer<ReadOnlySpan<char>, StringView>.Create(ReadOnlySpan<char> alternate) =>
            alternate.ToString();
        #endif
    }

    #endregion

    #region Inner type: OrdinalComparer

    /// <summary>
    /// Represents a <see cref="StringView"/> comparison operation
    /// that performs a case-sensitive ordinal string comparison.
    /// </summary>
    private sealed class OrdinalComparer : StringViewComparer
        #if NET9_0_OR_GREATER
        , IAlternateEqualityComparer<ReadOnlySpan<char>, StringView>
        #endif
    {
        /// <summary>
        /// A singleton instance of the <see cref="StringViewComparer.OrdinalComparer"/> class.
        /// </summary>
        public static readonly OrdinalComparer Instance = new();

        /// <inheritdoc />
        public override bool Equals(StringView x, StringView y) =>
            StringView.Equals(x, y);

        /// <inheritdoc />
        public override int Compare(StringView x, StringView y) =>
            StringView.CompareOrdinal(x, y);

        /// <inheritdoc />
        public override int GetHashCode(StringView obj) =>
            obj.GetHashCode();

        #if NET9_0_OR_GREATER
        /// <inheritdoc />
        bool IAlternateEqualityComparer<ReadOnlySpan<char>, StringView>.Equals(ReadOnlySpan<char> alternate, StringView other) =>
            alternate.SequenceEqual(other);

        /// <inheritdoc />
        int IAlternateEqualityComparer<ReadOnlySpan<char>, StringView>.GetHashCode(ReadOnlySpan<char> alternate) =>
            string.GetHashCode(alternate);

        /// <inheritdoc />
        StringView IAlternateEqualityComparer<ReadOnlySpan<char>, StringView>.Create(ReadOnlySpan<char> alternate) =>
            alternate.ToString();
        #endif
    }

    #endregion

    #region Inner type: OrdinalIgnoreCaseComparer

    /// <summary>
    /// Represents a <see cref="StringView"/> comparison operation
    /// that performs a case-insensitive ordinal string comparison.
    /// </summary>
    private sealed class OrdinalIgnoreCaseComparer : StringViewComparer
        #if NET9_0_OR_GREATER
        , IAlternateEqualityComparer<ReadOnlySpan<char>, StringView>
        #endif
    {
        /// <summary>
        /// A singleton instance of the <see cref="StringViewComparer.OrdinalIgnoreCaseComparer"/> class.
        /// </summary>
        public static readonly OrdinalIgnoreCaseComparer Instance = new();

        /// <inheritdoc />
        public override bool Equals(StringView x, StringView y) =>
            StringView.Equals(x, y, StringComparison.OrdinalIgnoreCase);

        /// <inheritdoc />
        public override int Compare(StringView x, StringView y) =>
            StringView.Compare(x, y, StringComparison.OrdinalIgnoreCase);

        /// <inheritdoc />
        public override int GetHashCode(StringView obj) =>
            obj.GetHashCode(StringComparison.OrdinalIgnoreCase);

        #if NET9_0_OR_GREATER
        /// <inheritdoc />
        bool IAlternateEqualityComparer<ReadOnlySpan<char>, StringView>.Equals(ReadOnlySpan<char> alternate, StringView other) =>
            alternate.Equals(other, StringComparison.OrdinalIgnoreCase);

        /// <inheritdoc />
        int IAlternateEqualityComparer<ReadOnlySpan<char>, StringView>.GetHashCode(ReadOnlySpan<char> alternate) =>
            string.GetHashCode(alternate, StringComparison.OrdinalIgnoreCase);

        /// <inheritdoc />
        StringView IAlternateEqualityComparer<ReadOnlySpan<char>, StringView>.Create(ReadOnlySpan<char> alternate) =>
            alternate.ToString();
        #endif
    }

    #endregion
}
