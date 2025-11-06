using Core;

namespace Game
{
    public interface IBattleGUI : IFeature
    {
        void ShowUnitSelection(BattleUnitData unitData);
        void HideUnitSelection();
        void OnAttackButtonClicked();
    }
}