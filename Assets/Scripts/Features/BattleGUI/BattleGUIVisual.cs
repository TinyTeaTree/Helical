using Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class BattleGUIVisual : BaseVisual<BattleGUI>
    {
        [SerializeField] private BattleUnitSelectionOperator _unitSelectionOperator;
        [SerializeField] private Button _attackButton;
        [SerializeField] private Button _moveButton;
        [SerializeField] private Button _rotateButton;

        private void Awake()
        {
            _attackButton.onClick.AddListener(OnAttackButtonClicked);
            _moveButton.onClick.AddListener(OnMoveButtonClicked);
            _rotateButton.onClick.AddListener(OnRotateButtonClicked);
        }

        private void OnDestroy()
        {
            _attackButton.onClick.RemoveListener(OnAttackButtonClicked);
            _moveButton.onClick.RemoveListener(OnMoveButtonClicked);
            _rotateButton.onClick.RemoveListener(OnRotateButtonClicked);
        }

        private void OnAttackButtonClicked()
        {
            Feature.OnAttackButtonClicked();
        }
        
        private void OnMoveButtonClicked()
        {
            Feature.OnMoveButtonClicked();
        }
        
        private void OnRotateButtonClicked()
        {
            Feature.OnRotateButtonClicked();
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