using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CollectionUtils
{
    public static void ForEachDo<T>(this IEnumerable<T> collection, System.Action<T> doAction)
    {
        foreach(var item in collection)
        {
            doAction(item);
        }
    }

    public static T GetRandom<T>(this IList<T> collection)
    {
        if (collection.IsNullOrEmpty())
        {
            return default;
        }
        
        return collection[Random.Range(0, collection.Count)];
    }

    public static bool IsNullOrEmpty<T>(this IList<T> collection)
    {
        return collection == null || collection.Count == 0;
    }

    public static bool HasContent<T>(this IList<T> collection)
    {
        return !collection.IsNullOrEmpty();
    }
    
    public static T GetRandom<T>(this IEnumerable<T> collection)
    {
        var list = collection.ToList();
        return list.GetRandom();
    }

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
    {
        return collection == null || !collection.Any();
    }

    public static bool HasContent<T>(this IEnumerable<T> collection)
    {
        return !collection.IsNullOrEmpty();
    }
}
