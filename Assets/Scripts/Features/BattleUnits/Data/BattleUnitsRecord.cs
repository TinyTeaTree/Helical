using System.Collections.Generic;
using Core;

namespace Game
{
    public class BattleUnitsRecord : BaseRecord
    {
        public List<BattleUnitData> BattleUnits { get; private set; } = new List<BattleUnitData>();
    }
}