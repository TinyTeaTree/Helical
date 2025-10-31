using UnityEngine;

namespace Services
{
    public static class Summoner
    {
        public static ISummoningService SummoningService { get; set; }

        public static T CreateAsset<T>(T loadedAsset, Transform parent)
            where T : UnityEngine.Object
        {
            return SummoningService.CreateAsset(loadedAsset, parent);
        }
    }
}