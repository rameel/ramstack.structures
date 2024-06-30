namespace Ramstack.Text;

/// <summary>
/// Provides extension methods for the <see cref="StringView"/> structure.
/// </summary>
public static class StringViewExtensions
{
    /// <summary>
    /// Creates a new <see cref="StringView"/> over a string.
    /// </summary>
    /// <param name="value">The target string.</param>
    /// <returns>
    /// The read-only view representation of the string.
    /// </returns>
    public static StringView AsView(this string? value) =>
        new(value ?? "");

    /// <summary>
    /// Creates a new <see cref="StringView"/> over a portion of the target string from a specified position to the end of the string.
    /// </summary>
    /// <param name="value">The target string.</param>
    /// <param name="index">The index at which to begin this slice.</param>
    /// <returns>
    /// The read-only view representation of the string.
    /// </returns>
    public static StringView AsView(this string? value, int index) =>
        new(value ?? "", index);

    /// <summary>
    /// Creates a new <see cref="StringView"/> over a portion of the target string from a specified position for a specified number of characters.
    /// </summary>
    /// <param name="value">The target string.</param>
    /// <param name="index">The index at which to begin this slice.</param>
    /// <param name="length">The desired length for the slice.</param>
    /// <returns>
    /// The read-only view representation of the string.
    /// </returns>
    public static StringView AsView(this string? value, int index, int length) =>
        new(value ?? "", index, length);
}
