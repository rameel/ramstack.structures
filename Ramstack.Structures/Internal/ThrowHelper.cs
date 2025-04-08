namespace Ramstack.Internal;

internal static class ThrowHelper
{
    [DoesNotReturn]
    public static void ThrowArgumentOutOfRangeException() =>
        throw new ArgumentOutOfRangeException();

    [DoesNotReturn]
    public static void ThrowArgumentOutOfRangeException_NeedNonNegative(ExceptionArgument argument) =>
        throw new ArgumentOutOfRangeException(GetArgumentName(argument), message: "Non-negative value required.");

    [DoesNotReturn]
    public static void ThrowArgumentException_DestinationTooShort(ExceptionArgument argument) =>
        throw new ArgumentException(message: "Destination too short.", paramName: GetArgumentName(argument));

    [DoesNotReturn]
    public static void ThrowArgumentException_AddingDuplicateKey<TKey>(TKey key) =>
        throw new ArgumentException($"An item with the same key has already been added. Key: {key}");

    [DoesNotReturn]
    public static void ThrowKeyNotFoundException<TKey>(TKey key) =>
        throw new KeyNotFoundException($"The key '{key}' was not present in the dictionary.");

    [DoesNotReturn]
    public static void ThrowArgumentNullException(ExceptionArgument argument) =>
        throw new ArgumentNullException(paramName: GetArgumentName(argument));

    [DoesNotReturn]
    public static void ThrowNotSupportedException() =>
        throw new NotSupportedException();

    [DoesNotReturn]
    public static void ThrowNotSupportedException_ConcurrentOperationsNotSupported() =>
        throw new InvalidOperationException("The collection's state was corrupted due to a concurrent update.");

    [DoesNotReturn]
    public static void ThrowInvalidOperationException_IncompatibleComparer() =>
        throw new InvalidOperationException("The collection's comparer does not support the requested operation.");

    private static string GetArgumentName(ExceptionArgument argument) => argument switch
    {
        ExceptionArgument.key => nameof(ExceptionArgument.key),
        ExceptionArgument.destination => nameof(ExceptionArgument.destination),
        ExceptionArgument.array => nameof(ExceptionArgument.array),
        _ => nameof(ExceptionArgument.capacity)
    };
}
