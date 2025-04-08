using System.Numerics;

namespace Ramstack.Internal;

/// <summary>
/// Provides utility methods for hash-related operations.
/// </summary>
internal static class HashHelper
{
    /// <summary>
    /// Rounds up the specified capacity to the nearest power of two.
    /// </summary>
    /// <param name="capacity">The capacity to round up. Must be greater than zero.</param>
    /// <returns>
    /// The smallest power of two that is greater than or equal to the specified capacity.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RoundUpToPowerOf2(int capacity)
    {
        Debug.Assert(capacity > 0);
        return (int)BitOperations.RoundUpToPowerOf2((uint)capacity);
    }
}
