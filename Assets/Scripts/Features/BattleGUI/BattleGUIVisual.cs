using Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class BattleGUIVisual : BaseVisual<BattleGUI>
    {
        [SerializeField] private BattleUnitSelectionOperator _unitSelectionOperator;
        [SerializeField] private Button _attackButton;

        private void Awake()
        {
            _attackButton.onClick.AddListener(OnAttackButtonClicked);
        }

        private void OnDestroy()
        {
            _attackButton.onClick.RemoveListener(OnAttackButtonClicked);
        }

        private void OnAttackButtonClicked()
        {
            Feature.OnAttackButtonClicked();
        }

        public void ShowUnitSelection()
        {
            _unitSelectionOperator.Show();
        }

        public void HideUnitSelection()
        {
            _unitSelectionOperator.Hide();
        }
    }
}