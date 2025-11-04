using Core;
using UnityEngine;

namespace Game
{
    public class BattleGUIVisual : BaseVisual<BattleGUI>
    {
        [SerializeField] private BattleUnitSelectionOperator _unitSelectionOperator;

        public void ShowUnitSelection()
        {
            if (_unitSelectionOperator != null)
            {
                _unitSelectionOperator.Show();
            }
        }

        public void HideUnitSelection()
        {
            if (_unitSelectionOperator != null)
            {
                _unitSelectionOperator.Hide();
            }
        }
    }
}