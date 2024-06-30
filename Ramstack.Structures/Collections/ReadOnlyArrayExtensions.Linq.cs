using System.Runtime.CompilerServices;

using JetBrains.Annotations;

namespace Ramstack.Collections;

public static partial class ReadOnlyArrayExtensions
{
    /// <inheritdoc cref="Enumerable.Aggregate{TSource}(IEnumerable{TSource}, Func{TSource,TSource,TSource})"/>
    [LinqTunnel]
    public static T Aggregate<T>(this ReadOnlyArray<T> source, Func<T, T, T> func) =>
        source.Inner!.Aggregate(func);

    /// <inheritdoc cref="Enumerable.Aggregate{TSource,TAccumulate}(IEnumerable{TSource}, TAccumulate, Func{TAccumulate,TSource,TAccumulate})"/>
    [LinqTunnel]
    public static TAccumulate Aggregate<T, TAccumulate>(this ReadOnlyArray<T> source, TAccumulate seed, Func<TAccumulate, T, TAccumulate> func) =>
        source.Inner!.Aggregate(seed, func);

    /// <inheritdoc cref="Enumerable.Aggregate{TSource,TAccumulate,TResult}(IEnumerable{TSource}, TAccumulate, Func{TAccumulate,TSource,TAccumulate}, Func{TAccumulate,TResult})"/>
    [LinqTunnel]
    public static TResult Aggregate<T, TAccumulate, TResult>(this ReadOnlyArray<T> source, TAccumulate seed, Func<TAccumulate, T, TAccumulate> func, Func<TAccumulate, TResult> resultSelector) =>
        source.Inner!.Aggregate(seed, func, resultSelector);

    /// <inheritdoc cref="Enumerable.Select{TSource,TResult}(IEnumerable{TSource},Func{TSource,TResult})"/>
    [LinqTunnel]
    public static IEnumerable<TResult> Select<T, TResult>(this ReadOnlyArray<T> source, Func<T, TResult> selector) =>
        source.Inner!.Select(selector);

    /// <inheritdoc cref="Enumerable.Select{TSource,TResult}(IEnumerable{TSource},Func{TSource,int,TResult})"/>
    [LinqTunnel]
    public static IEnumerable<TResult> Select<T, TResult>(this ReadOnlyArray<T> source, Func<T, int, TResult> selector) =>
        source.Inner!.Select(selector);

    /// <inheritdoc cref="Enumerable.SelectMany{TSource,TResult}(IEnumerable{TSource},Func{TSource,IEnumerable{TResult}})"/>
    [LinqTunnel]
    public static IEnumerable<TResult> SelectMany<T, TResult>(this ReadOnlyArray<T> source, Func<T, IEnumerable<TResult>> selector) =>
        source.Inner!.SelectMany(selector);

    /// <inheritdoc cref="Enumerable.SelectMany{TSource,TResult}(IEnumerable{TSource},Func{TSource,int,IEnumerable{TResult}})"/>
    [LinqTunnel]
    public static IEnumerable<TResult> SelectMany<T, TResult>(this ReadOnlyArray<T> source, Func<T, int, IEnumerable<TResult>> selector) =>
        source.Inner!.SelectMany(selector);

    /// <inheritdoc cref="Enumerable.SelectMany{TSource,TCollection,TResult}(IEnumerable{TSource},Func{TSource,IEnumerable{TCollection}},Func{TSource,TCollection,TResult})"/>
    [LinqTunnel]
    public static IEnumerable<TResult> SelectMany<T, TCollection, TResult>(this ReadOnlyArray<T> source, Func<T, IEnumerable<TCollection>> collectionSelector, Func<T, TCollection, TResult> resultSelector) =>
        source.Inner!.SelectMany(collectionSelector, resultSelector);

    /// <inheritdoc cref="Enumerable.SelectMany{TSource,TCollection,TResult}(IEnumerable{TSource},Func{TSource,int,IEnumerable{TCollection}},Func{TSource,TCollection,TResult})"/>
    [LinqTunnel]
    public static IEnumerable<TResult> SelectMany<T, TCollection, TResult>(this ReadOnlyArray<T> source, Func<T, int, IEnumerable<TCollection>> collectionSelector, Func<T, TCollection, TResult> resultSelector) =>
        source.Inner!.SelectMany(collectionSelector, resultSelector);

    /// <summary>
    /// Sorts the elements of a sequence in ascending order.
    /// </summary>
    /// <typeparam name="T">The types of elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <returns>
    /// An <see cref="IOrderedEnumerable{TElement}"/> whose elements are sorted.
    /// </returns>
    [LinqTunnel]
    public static IEnumerable<T> Order<T>(this ReadOnlyArray<T> source)
    {
        #if NET6_0
        return source.Inner!.OrderBy(SortHelper<T>.Identity);
        #else
        return source.Inner!.Order();
        #endif
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending order.
    /// </summary>
    /// <typeparam name="T">The types of elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
    /// <returns>
    /// An <see cref="IOrderedEnumerable{TElement}"/> whose elements are sorted.
    /// </returns>
    [LinqTunnel]
    public static IEnumerable<T> Order<T>(this ReadOnlyArray<T> source, IComparer<T>? comparer)
    {
        #if NET6_0
        return source.Inner!.OrderBy(SortHelper<T>.Identity, comparer);
        #else
        return source.Inner!.Order(comparer);
        #endif
    }

    /// <summary>
    /// Sorts the elements of a sequence in descending order.
    /// </summary>
    /// <typeparam name="T">The types of elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <returns>
    /// An <see cref="IOrderedEnumerable{TElement}"/> whose elements are sorted.
    /// </returns>
    [LinqTunnel]
    public static IEnumerable<T> OrderDescending<T>(this ReadOnlyArray<T> source)
    {
        #if NET6_0
        return source.Inner!.OrderByDescending(SortHelper<T>.Identity);
        #else
        return source.Inner!.OrderDescending();
        #endif
    }

    /// <summary>
    /// Sorts the elements of a sequence in descending order.
    /// </summary>
    /// <typeparam name="T">The types of elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
    /// <returns>
    /// An <see cref="IOrderedEnumerable{TElement}"/> whose elements are sorted.
    /// </returns>
    [LinqTunnel]
    public static IEnumerable<T> OrderDescending<T>(this ReadOnlyArray<T> source, IComparer<T>? comparer)
    {
        #if NET6_0
        return source.Inner!.OrderByDescending(SortHelper<T>.Identity, comparer);
        #else
        return source.Inner!.OrderDescending(comparer);
        #endif
    }

    /// <inheritdoc cref="Enumerable.OrderBy{TSource,TKey}(IEnumerable{TSource},Func{TSource,TKey})"/>
    [LinqTunnel]
    public static IEnumerable<T> OrderBy<T, TKey>(this ReadOnlyArray<T> source, Func<T, TKey> keySelector) =>
        source.Inner!.OrderBy(keySelector);

    /// <inheritdoc cref="Enumerable.OrderBy{TSource,TKey}(IEnumerable{TSource},Func{TSource,TKey},IComparer{TKey})"/>
    [LinqTunnel]
    public static IEnumerable<T> OrderBy<T, TKey>(this ReadOnlyArray<T> source, Func<T, TKey> keySelector, IComparer<TKey>? comparer) =>
        source.Inner!.OrderBy(keySelector, comparer);

    /// <inheritdoc cref="Enumerable.OrderByDescending{TSource,TKey}(IEnumerable{TSource},Func{TSource,TKey})"/>
    [LinqTunnel]
    public static IEnumerable<T> OrderByDescending<T, TKey>(this ReadOnlyArray<T> source, Func<T, TKey> keySelector) =>
        source.Inner!.OrderByDescending(keySelector);

    /// <inheritdoc cref="Enumerable.OrderByDescending{TSource,TKey}(IEnumerable{TSource},Func{TSource,TKey},IComparer{TKey})"/>
    [LinqTunnel]
    public static IEnumerable<T> OrderByDescending<T, TKey>(this ReadOnlyArray<T> source, Func<T, TKey> keySelector, IComparer<TKey>? comparer) =>
        source.Inner!.OrderByDescending(keySelector, comparer);

    /// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource},Func{TSource,bool})"/>
    [LinqTunnel]
    public static IEnumerable<T> Where<T>(this ReadOnlyArray<T> source, Func<T, bool> selector) =>
        source.Inner!.Where(selector);

    /// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource},Func{TSource,int,bool})"/>
    [LinqTunnel]
    public static IEnumerable<T> Where<T>(this ReadOnlyArray<T> source, Func<T, int, bool> selector) =>
        source.Inner!.Where(selector);

    /// <inheritdoc cref="Enumerable.Distinct{TSource}(IEnumerable{TSource})"/>
    [LinqTunnel]
    public static IEnumerable<T> Distinct<T>(this ReadOnlyArray<T> source) =>
        source.Inner!.Distinct();

    /// <inheritdoc cref="Enumerable.Distinct{TSource}(IEnumerable{TSource},IEqualityComparer{TSource})"/>
    [LinqTunnel]
    public static IEnumerable<T> Distinct<T>(this ReadOnlyArray<T> source, IEqualityComparer<T>? comparer) =>
        source.Inner!.Distinct(comparer);

    /// <inheritdoc cref="Enumerable.DistinctBy{TSource,TKey}(IEnumerable{TSource},Func{TSource,TKey})"/>
    public static IEnumerable<T> DistinctBy<T, TKey>(this ReadOnlyArray<T> source, Func<T, TKey> keySelector) =>
        source.Inner!.DistinctBy(keySelector);

    /// <inheritdoc cref="Enumerable.DistinctBy{TSource,TKey}(IEnumerable{TSource},Func{TSource,TKey},IEqualityComparer{TKey})"/>
    [LinqTunnel]
    public static IEnumerable<T> DistinctBy<T, TKey>(this ReadOnlyArray<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey>? comparer) =>
        source.Inner!.DistinctBy(keySelector, comparer);

    /// <inheritdoc cref="Enumerable.Any{TSource}(IEnumerable{TSource})"/>
    public static bool Any<T>(this ReadOnlyArray<T> source) =>
        source.Inner!.Length != 0;

    /// <inheritdoc cref="Enumerable.Any{TSource}(IEnumerable{TSource},Func{TSource,bool})"/>
    public static bool Any<T>(this ReadOnlyArray<T> source, Func<T, bool> predicate)
    {
        foreach (var item in source.Inner!)
            if (predicate(item))
                return true;

        return false;
    }

    /// <inheritdoc cref="Enumerable.All{TSource}(IEnumerable{TSource},Func{TSource,bool})"/>
    public static bool All<T>(this ReadOnlyArray<T> source, Func<T, bool> predicate)
    {
        foreach (var item in source.Inner!)
            if (!predicate(item))
                return false;

        return true;
    }

    /// <inheritdoc cref="Enumerable.Count{TSource}(IEnumerable{TSource})"/>
    public static int Count<T>(this ReadOnlyArray<T> source) =>
        source.Inner!.Length;

    /// <inheritdoc cref="Enumerable.Count{TSource}(IEnumerable{TSource},Func{TSource,bool})"/>
    public static int Count<T>(this ReadOnlyArray<T> source, Func<T, bool> predicate)
    {
        var count = 0;

        foreach (var item in source.Inner!)
            if (predicate(item))
                count++;

        return count;
    }

    /// <inheritdoc cref="Enumerable.Take{TSource}(IEnumerable{TSource},int)"/>
    [LinqTunnel]
    public static IEnumerable<T> Take<T>(this ReadOnlyArray<T> source, int count) =>
        source.Inner!.Take(count);

    /// <inheritdoc cref="Enumerable.TakeLast{TSource}"/>
    [LinqTunnel]
    public static IEnumerable<T> TakeLast<T>(this ReadOnlyArray<T> source, int count) =>
        source.Inner!.TakeLast(count);

    /// <inheritdoc cref="Enumerable.TakeWhile{TSource}(IEnumerable{TSource},Func{TSource,bool})"/>
    [LinqTunnel]
    public static IEnumerable<T> TakeWhile<T>(this ReadOnlyArray<T> source, Func<T, bool> predicate) =>
        source.Inner!.TakeWhile(predicate);

    /// <inheritdoc cref="Enumerable.TakeWhile{TSource}(IEnumerable{TSource},Func{TSource,int,bool})"/>
    [LinqTunnel]
    public static IEnumerable<T> TakeWhile<T>(this ReadOnlyArray<T> source, Func<T, int, bool> predicate) =>
        source.Inner!.TakeWhile(predicate);

    /// <inheritdoc cref="Enumerable.Skip{TSource}"/>
    [LinqTunnel]
    public static IEnumerable<T> Skip<T>(this ReadOnlyArray<T> source, int count) =>
        source.Inner!.Skip(count);

    /// <inheritdoc cref="Enumerable.SkipLast{TSource}"/>
    [LinqTunnel]
    public static IEnumerable<T> SkipLast<T>(this ReadOnlyArray<T> source, int count) =>
        source.Inner!.SkipLast(count);

    /// <inheritdoc cref="Enumerable.SkipWhile{TSource}(IEnumerable{TSource},Func{TSource,bool})"/>
    [LinqTunnel]
    public static IEnumerable<T> SkipWhile<T>(this ReadOnlyArray<T> source, Func<T, bool> predicate) =>
        source.Inner!.SkipWhile(predicate);

    /// <inheritdoc cref="Enumerable.SkipWhile{TSource}(IEnumerable{TSource},Func{TSource,int,bool})"/>
    [LinqTunnel]
    public static IEnumerable<T> SkipWhile<T>(this ReadOnlyArray<T> source, Func<T, int, bool> predicate) =>
        source.Inner!.SkipWhile(predicate);

    /// <inheritdoc cref="Enumerable.Single{TSource}(IEnumerable{TSource})"/>
    public static T Single<T>(this ReadOnlyArray<T> source) =>
        source.Inner!.Single();

    /// <inheritdoc cref="Enumerable.Single{TSource}(IEnumerable{TSource},Func{TSource,bool})"/>
    public static T Single<T>(this ReadOnlyArray<T> source, Func<T, bool> predicate) =>
        source.Inner!.Single(predicate);

    /// <inheritdoc cref="Enumerable.SingleOrDefault{TSource}(IEnumerable{TSource})"/>
    public static T? SingleOrDefault<T>(this ReadOnlyArray<T> source) =>
        source.Inner!.SingleOrDefault();

    /// <inheritdoc cref="Enumerable.SingleOrDefault{TSource}(IEnumerable{TSource},Func{TSource,bool})"/>
    public static T? SingleOrDefault<T>(this ReadOnlyArray<T> source, Func<T, bool> predicate) =>
        source.Inner!.SingleOrDefault(predicate);

    /// <inheritdoc cref="Enumerable.First{TSource}(IEnumerable{TSource})"/>
    public static T First<T>(this ReadOnlyArray<T> source)
    {
        if (source.Length != 0)
            return source[0];

        // throw the same exception as LINQ
        return source.Inner!.First();
    }

    /// <inheritdoc cref="Enumerable.First{TSource}(IEnumerable{TSource},Func{TSource,bool})"/>
    public static T First<T>(this ReadOnlyArray<T> source, Func<T, bool> predicate)
    {
        foreach (var item in source.Inner!)
            if (predicate(item))
                return item;

        // throw the same exception as LINQ
        return source.Inner.First(predicate);
    }

    /// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? FirstOrDefault<T>(this ReadOnlyArray<T> source) =>
        source.Length == 0 ? default : source[0];

    /// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource},Func{TSource,bool})"/>
    public static T? FirstOrDefault<T>(this ReadOnlyArray<T> source, Func<T, bool> predicate)
    {
        foreach (var item in source.Inner!)
            if (predicate(item))
                return item;

        return default;
    }

    /// <inheritdoc cref="Enumerable.Last{TSource}(IEnumerable{TSource})"/>
    public static T Last<T>(this ReadOnlyArray<T> source)
    {
        if ((uint)source.Length - 1 < (uint)source.Length)
            return source[^1];

        // throw the same exception as LINQ
        return source.Inner!.Last();
    }

    /// <inheritdoc cref="Enumerable.Last{TSource}(IEnumerable{TSource},Func{TSource,bool})"/>
    public static T Last<T>(this ReadOnlyArray<T> source, Func<T, bool> predicate)
    {
        var index = source.Length;

        while (true)
        {
            if ((uint)--index >= (uint)source.Length)
                break;

            if (predicate(source[index]))
                return source[index];
        }

        // throw the same exception as LINQ
        return source.Inner!.Last(predicate);
    }

    /// <inheritdoc cref="Enumerable.LastOrDefault{TSource}(IEnumerable{TSource})"/>
    public static T? LastOrDefault<T>(this ReadOnlyArray<T> source)
    {
        if ((uint)source.Length - 1 < (uint)source.Length)
            return source[^1];

        return default;
    }

    /// <inheritdoc cref="Enumerable.LastOrDefault{TSource}(IEnumerable{TSource},Func{TSource,bool})"/>
    public static T? LastOrDefault<T>(this ReadOnlyArray<T> source, Func<T, bool> predicate)
    {
        var index = source.Length;

        while (true)
        {
            if ((uint)--index >= (uint)source.Length)
                break;

            if (predicate(source[index]))
                return source[index];
        }

        return default;
    }

    /// <inheritdoc cref="Enumerable.GroupBy{TSource,TKey}(IEnumerable{TSource}, Func{TSource,TKey})"/>
    public static IEnumerable<IGrouping<TKey, T>> GroupBy<T, TKey>(this ReadOnlyArray<T> source, Func<T, TKey> keySelector) =>
        source.Inner!.GroupBy(keySelector);

    /// <inheritdoc cref="Enumerable.GroupBy{TSource,TKey,TElement}(IEnumerable{TSource}, Func{TSource,TKey}, Func{TSource,TElement})"/>
    public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<T, TKey, TElement>(this ReadOnlyArray<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector) =>
        source.Inner!.GroupBy(keySelector, elementSelector);

    /// <inheritdoc cref="Enumerable.GroupBy{TSource,TKey}(IEnumerable{TSource}, Func{TSource,TKey}, IEqualityComparer{TKey})"/>
    public static IEnumerable<IGrouping<TKey, T>> GroupBy<T, TKey>(this ReadOnlyArray<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey>? comparer) =>
        source.Inner!.GroupBy(keySelector, comparer);

    /// <inheritdoc cref="Enumerable.GroupBy{TSource,TKey,TElement}(IEnumerable{TSource}, Func{TSource,TKey}, Func{TSource,TElement}, IEqualityComparer{TKey})"/>
    public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<T, TKey, TElement>(this ReadOnlyArray<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, IEqualityComparer<TKey>? comparer) =>
        source.Inner!.GroupBy(keySelector, elementSelector, comparer);

    /// <inheritdoc cref="Enumerable.AsEnumerable{TSource}"/>
    [LinqTunnel]
    public static IEnumerable<T> AsEnumerable<T>(this ReadOnlyArray<T> source) =>
        source.Inner!;

    /// <inheritdoc cref="Queryable.AsQueryable{TSource}(IEnumerable{TSource})"/>
    [LinqTunnel]
    public static IQueryable<T> AsQueryable<T>(this ReadOnlyArray<T> source) =>
        source.Inner!.AsQueryable();

    /// <inheritdoc cref="Enumerable.ToArray{TSource}"/>
    public static T[] ToArray<T>(this ReadOnlyArray<T> source)
    {
        _ = source.Length;
        return source.AsSpan().ToArray();
    }

    /// <inheritdoc cref="Enumerable.ToList{TSource}"/>
    public static List<T> ToList<T>(this ReadOnlyArray<T> source)
    {
        #if NET8_0_OR_GREATER
        var list = new List<T>(source.Length);

        System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, source.Length);
        source.CopyTo(System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list));

        return list;
        #else
        return source.Inner!.ToList();
        #endif
    }

    /// <inheritdoc cref="Enumerable.ToDictionary{TSource,TKey,TElement}(IEnumerable{TSource}, Func{TSource,TKey}, Func{TSource,TElement})"/>
    public static Dictionary<TKey, TElement> ToDictionary<T, TKey, TElement>(this ReadOnlyArray<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector) where TKey : notnull =>
        source.Inner!.ToDictionary(keySelector, elementSelector);

    /// <inheritdoc cref="Enumerable.ToDictionary{TSource,TKey}(IEnumerable{TSource}, Func{TSource,TKey})"/>
    public static Dictionary<TKey, T> ToDictionary<T, TKey>(this ReadOnlyArray<T> source, Func<T, TKey> keySelector) where TKey : notnull =>
        source.Inner!.ToDictionary(keySelector);

    /// <inheritdoc cref="Enumerable.ToDictionary{TSource,TKey,TElement}(IEnumerable{TSource}, Func{TSource,TKey}, Func{TSource,TElement}, IEqualityComparer{TKey})"/>
    public static Dictionary<TKey, TElement> ToDictionary<T, TKey, TElement>(this ReadOnlyArray<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, IEqualityComparer<TKey>? comparer) where TKey : notnull =>
        source.Inner!.ToDictionary(keySelector, elementSelector, comparer);

    /// <inheritdoc cref="Enumerable.ToDictionary{TSource,TKey}(IEnumerable{TSource}, Func{TSource,TKey}, IEqualityComparer{TKey})"/>
    public static Dictionary<TKey, T> ToDictionary<T, TKey>(this ReadOnlyArray<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey>? comparer) where TKey : notnull =>
        source.Inner!.ToDictionary(keySelector, comparer);

    /// <inheritdoc cref="Enumerable.ToHashSet{TSource}(IEnumerable{TSource})"/>
    public static HashSet<T> ToHashSet<T>(this ReadOnlyArray<T> source) =>
        new(source.Inner!, null);

    /// <inheritdoc cref="Enumerable.ToHashSet{TSource}(IEnumerable{TSource}, IEqualityComparer{TSource})"/>
    public static HashSet<T> ToHashSet<T>(this ReadOnlyArray<T> source, IEqualityComparer<T>? comparer) =>
        new(source.Inner!, comparer);

    /// <summary>
    /// Determines whether two sequences are equal by comparing the elements by using the default equality comparer for their type.
    /// </summary>
    /// <param name="a">The sequence to compare to second.</param>
    /// <param name="b">The sequence to compare to the first sequence.</param>
    /// <returns>
    /// <see langword="true"/> if the two sequences are of equal length and their corresponding elements are equal
    /// according to the default equality comparer for their type; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool SequenceEqual<T>(this ReadOnlyArray<T> a, [InstantHandle] IEnumerable<T> b) =>
        a.Inner!.SequenceEqual(b);

    /// <summary>
    /// Determines whether two sequences are equal by comparing the elements by using the specified equality comparer for their type.
    /// </summary>
    /// <param name="a">The sequence to compare to second.</param>
    /// <param name="b">The sequence to compare to the first sequence.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use to compare elements.</param>
    /// <returns>
    /// <see langword="true"/> if the two sequences are of equal length and their corresponding elements are equal
    /// according to the specified equality comparer for their type; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool SequenceEqual<T>(this ReadOnlyArray<T> a, [InstantHandle] IEnumerable<T> b, IEqualityComparer<T>? comparer) =>
        a.Inner!.SequenceEqual(b, comparer);

    #if NET6_0

    private static class SortHelper<T>
    {
        public static readonly Func<T, T> Identity = e => e;
    }

    #endif
}
