using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class MathUtils
{
    public static int ToIntRandomRound(this float f)
    {
        var remainder = f - Mathf.Floor(f);
        if(Random.value < remainder)
        {
            return Mathf.FloorToInt(f);
        }
        else
        {
            return Mathf.CeilToInt(f);
        }
    }

    public static T RandomRoll<T>(this IEnumerable<T> collection)
        where T : RandomWeightOption
    {
        var offers = collection.ToArray();

        int[] sums = new int[offers.Count()];
        sums[0] = offers[0].Probability;
        for (int i = 1; i < offers.Count(); ++i)
        {
            sums[i] = sums[i - 1] + offers[i].Probability;
        }

        var sum = offers.Sum(o => o.Probability);
        var randomRoll = UnityEngine.Random.Range(0f, sum);

        for (int i = 0; i < offers.Count(); ++i)
        {
            if (randomRoll < sums[i])
            {
                return offers[i];
            }
        }

        UnityEngine.Debug.LogError("Could not roll a new exchange");
        return null;
    }

    public static bool IsEven(this int i)
    {
        return i % 2 == 0;
    }

    public static bool IsOdd(this int i)
    {
        return i % 2 == 1;
    }
}
