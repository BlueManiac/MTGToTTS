using System;
using System.Collections.Generic;
using System.Linq;

public static class IEnumerableExtensions {
    public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int chunkSize)
    {
        return source.Where((x,i) => i % chunkSize == 0).Select((x,i) => source.Skip(i * chunkSize).Take(chunkSize));
    }

    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
    {
        return items.GroupBy(property).Select(x => x.First());
    }
}