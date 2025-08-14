using Ramstack.Collections;
using Ramstack.Text;

namespace Ramstack.InteropServices;

/// <summary>
/// Provides extension methods for the <see cref="MemoryMarshal"/> class.
/// </summary>
public static class MemoryMarshalExtensions
{
    extension(MemoryMarshal)
    {
        /// <summary>
        /// Creates a new <see cref="StringView"/> over a portion of the specified string
        /// starting at a specified position to the end of the string.
        /// </summary>
        /// <param name="value">The string to create a view over.</param>
        /// <param name="index">The zero-based starting position of the view in the string.</param>
        /// <returns>
        /// A <see cref="StringView"/> representing the specified portion of the string.
        /// </returns>
        /// <remarks>
        /// This method should be used with caution as it doesn't perform argument validation.
        /// The caller is responsible for ensuring:
        /// <list type="bullet">
        ///   <item><description><paramref name="value"/> is not null.</description></item>
        ///   <item><description><paramref name="index"/> is within the bounds of the string.</description></item>
        /// </list>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringView CreateStringView(string value, int index)
        {
            Debug.Assert(value is not null);
            Debug.Assert(value.AsSpan(index).Length == value.Length - index);

            return new StringView(value, index, value.Length - index, unused: 0);
        }

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
        /// <remarks>
        /// This method should be used with caution as it doesn't perform argument validation.
        /// The caller is responsible for ensuring:
        /// <list type="bullet">
        ///   <item><description><paramref name="value"/> is not null.</description></item>
        ///   <item><description><paramref name="index"/> is within the bounds of the string.</description></item>
        ///   <item><description><paramref name="length"/> is valid for the specified index.</description></item>
        /// </list>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringView CreateStringView(string value, int index, int length)
        {
            Debug.Assert(value is not null);
            Debug.Assert(value.AsSpan(index, length).Length == length);

            return new StringView(value, index, length, unused: 0);
        }

        /// <summary>
        /// Creates a new <see cref="ArrayView{T}"/> over a portion of the specified array,
        /// starting at a specified position to the end of the array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The array to create a view over.</param>
        /// <param name="index">The zero-based starting position of the view in the array.</param>
        /// <returns>
        /// An <see cref="ArrayView{T}"/> representing the specified portion of the array.
        /// </returns>
        /// <remarks>
        /// This method should be used with caution as it doesn't perform argument validation.
        /// The caller is responsible for ensuring:
        /// <list type="bullet">
        ///   <item><description><paramref name="array"/> is not null.</description></item>
        ///   <item><description><paramref name="index"/> is within the bounds of the array.</description></item>
        /// </list>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArrayView<T> CreateArrayView<T>(T[] array, int index)
        {
            Debug.Assert(array is not null);
            Debug.Assert(array.AsSpan(index).Length == array.Length - index);

            return new ArrayView<T>(array, index, array.Length - index, unused: 0);
        }

        /// <summary>
        /// Creates a new <see cref="ArrayView{T}"/> over a portion of the specified array
        /// starting at a specified position for a specified number of elements.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The array to create a view over.</param>
        /// <param name="index">The zero-based starting position of the view in the array.</param>
        /// <param name="length">The number of elements to include in the view.</param>
        /// <returns>
        /// An <see cref="ArrayView{T}"/> representing the specified portion of the array.
        /// </returns>
        /// <remarks>
        /// This method should be used with caution as it doesn't perform argument validation.
        /// The caller is responsible for ensuring:
        /// <list type="bullet">
        ///   <item><description><paramref name="array"/> is not null.</description></item>
        ///   <item><description><paramref name="index"/> is within the bounds of the array.</description></item>
        ///   <item><description><paramref name="length"/> is valid for the specified index.</description></item>
        /// </list>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArrayView<T> CreateArrayView<T>(T[] array, int index, int length)
        {
            Debug.Assert(array is not null);
            Debug.Assert(array.AsSpan(index, length).Length == length);

            return new ArrayView<T>(array, index, length, unused: 0);
        }

        /// <summary>
        /// Attempts to get a <see cref="StringView"/> from the underlying memory buffer.
        /// </summary>
        /// <param name="memory">Read-only memory containing a block of characters.</param>
        /// <param name="view">When this method returns, contains the <see cref="StringView"/>
        /// retrieved from the underlying read-only memory buffer.</param>
        /// <returns>
        /// <see langword="true"/> if the method successfully retrieves the underlying string;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TryGetStringView(ReadOnlyMemory<char> memory, out StringView view)
        {
            if (MemoryMarshal.TryGetString(memory, out var value, out var start, out var length))
            {
                view = new StringView(value, start, length, unused: 0);
                return true;
            }

            view = default;
            return false;
        }

        /// <summary>
        /// Attempts to get a <see cref="StringView"/> from the underlying memory buffer.
        /// </summary>
        /// <typeparam name="T">The type of elements in the read-only memory buffer.</typeparam>
        /// <param name="memory">Read-only memory buffer.</param>
        /// <param name="view">When this method returns, contains the <see cref="ArrayView{T}"/>
        /// retrieved from the underlying read-only memory buffer.</param>
        /// <returns>
        /// <see langword="true"/> if the underlying memory buffer represents an array segment;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TryGetArrayView<T>(ReadOnlyMemory<T> memory, out ArrayView<T> view)
        {
            if (MemoryMarshal.TryGetArray(memory, out var segment))
            {
                view = segment;
                return true;
            }

            view = default;
            return false;
        }
    }
}
