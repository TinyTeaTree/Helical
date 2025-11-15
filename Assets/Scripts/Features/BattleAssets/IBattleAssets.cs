using Core;

namespace Game
{
    public interface IBattleAssets : IFeature
    {
        public int GoldAmount { get; }
        
        //TODO: Add API to Add/Remove Gold
    }
}