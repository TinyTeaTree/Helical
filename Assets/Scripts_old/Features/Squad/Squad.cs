using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChessRaid
{
    public class Squad : WagMonoton<Squad>
    {
        List<Champion> _champions;

        public IEnumerable<Champion> GetChampions(Team team)
        {
            foreach(var c in _champions)
            {
                if(c.Team == team)
                {
                    yield return c;
                }
            }
        }

        public void SetUp()
        {
            _champions = new List<Champion>();

            var startingState = PlayerManager.Single.State.Grid;

            foreach (var state in startingState.Champions)
            {
                var championPrefab = RulesManager.Single.GetChampionDef(state.ChampionId).Prefab;
                var hex = GridManager.Single.GetHex(state.Location);
                var championInstance = Instantiate(championPrefab, hex.transform);

                _champions.Add(championInstance);

                hex.SetChampion(championInstance);

                championInstance.State = state;

                championInstance.SetDirection(state.Direction);
            }
        }

        public void RemoveChampion(Champion champion)
        {
            _champions.RemoveAt(_champions.FindIndex(o => o == champion));
        }
    }
}