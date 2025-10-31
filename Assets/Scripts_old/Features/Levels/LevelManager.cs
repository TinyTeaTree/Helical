using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChessRaid
{
    public class LevelManager : WagSingleton<LevelManager>
    {
        public List<GridLevelSO> Levels;



        internal void Init()
        {
            Levels = Resources.Load<LevelsSO>("Level Collection").Levels;
        }

        public GridLevelSO GetLevel(string id)
        {
            return Levels.First(l => l.Id == id);
        }
    }
}