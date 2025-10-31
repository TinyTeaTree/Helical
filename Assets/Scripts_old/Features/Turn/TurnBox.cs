using System.Collections.Generic;
using System.Linq;

namespace ChessRaid
{
    public class TurnBox : DataBox
    {
        public const string BoxId = "Turn";
        public override string Id => BoxId;

        public List<TurnChain> Chains = new();

        public TurnChain GetChampionChain(Champion champion)
        {
            var championChain = Chains.FirstOrDefault(c => c.Champion == champion);
            if (championChain == null)
            {
                championChain = new TurnChain() { Champion = champion };
                Chains.Add(championChain);
            }

            return championChain;
        }

    }
}