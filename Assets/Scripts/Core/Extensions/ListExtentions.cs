using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public static class ListExtentions
    {
        public static IEnumerable<int> Range(this int number)
        {
            for (int i = 0; i < number; i++)
            {
                yield return i;
            }
        }

        public static T GetRandom<T>(this List<T> list)
        {
            if (list == null)
            {
                Notebook.NoteError($"Null list for GetRandom");
                return default;
            }
            else if (list.Count == 1)
            {
                return list[0];
            }
            
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        
        public static T GetRandom<T>(this List<T> list, System.Func<T, int> scoreFunction)
        {
            if (list == null)
            {
                Notebook.NoteError($"Null list for GetRandom");
                return default;
            }
            else if (list.Count == 1)
            {
                return list[0];
            }

            int fullScore = list.Sum(scoreFunction);
            int random = UnityEngine.Random.Range(0, fullScore);

            int count = -1;
            for (int i = 0; i < list.Count; ++i)
            {
                count += scoreFunction(list[i]);

                if (count >= random)
                {
                    return list[i];
                }
            }
            
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
    }
}