using Core;

namespace Game
{
    public interface IBattleAssets : IFeature
    {
        public int GoldAmount { get; }

        bool CanAfford(int amount);
        bool TrySpendGold(int amount);
        void AddGold(int amount);
    }
}