using Core;

namespace Game
{
    public interface IBattleGUI : IFeature
    {
        void ShowUnitSelection();
        void HideUnitSelection();
    }
}