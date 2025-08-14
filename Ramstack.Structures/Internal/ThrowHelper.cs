namespace Ramstack.Internal;

/// <summary>
/// Provides helper methods for throwing exceptions.
/// </summary>
internal static class ThrowHelper
{
    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    [DoesNotReturn]
    public static void ThrowArgumentOutOfRangeException() =>
        throw new ArgumentOutOfRangeException();

    /// <summary>
    /// Throws a <see cref="NotSupportedException"/>.
    /// </summary>
    [DoesNotReturn]
    public static void ThrowNotSupportedException() =>
        throw new NotSupportedException();
}
