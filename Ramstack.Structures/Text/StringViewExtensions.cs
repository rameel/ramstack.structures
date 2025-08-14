namespace Ramstack.Text;

/// <summary>
/// Provides extension methods for the <see cref="StringView"/> structure.
/// </summary>
public static class StringViewExtensions
{
    /// <summary>
    /// Creates a new <see cref="StringView"/> over a string.
    /// </summary>
    /// <param name="value">The string to create a view over.</param>
    /// <returns>
    /// A <see cref="StringView"/> representing the specified string.
    /// </returns>
    public static StringView AsView(this string? value) =>
        new(value ?? "");

    /// <summary>
    /// Creates a new <see cref="StringView"/> over a portion of the specified string
    /// starting at a specified position to the end of the string.
    /// </summary>
    /// <param name="value">The string to create a view over.</param>
    /// <param name="index">The zero-based starting position of the view in the string.</param>
    /// <returns>
    /// A <see cref="StringView"/> representing the specified portion of the string.
    /// </returns>
    public static StringView AsView(this string? value, int index) =>
        new(value ?? "", index);

    /// <summary>
    /// Creates a new <see cref="StringView"/> over a portion of the specified string
    /// starting at a specified position for a specified number of characters.
    /// </summary>
    /// <param name="value">The string to create a view over.</param>
    /// <param name="index">The zero-based starting position of the view in the string.</param>
    /// <param name="length">The number of characters to include in the view.</param>
    /// <returns>
    /// A <see cref="StringView"/> representing the specified portion of the string.
    /// </returns>
    public static StringView AsView(this string? value, int index, int length) =>
        new(value ?? "", index, length);
}
