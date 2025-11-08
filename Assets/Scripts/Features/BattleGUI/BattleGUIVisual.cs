using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class BattleGUIVisual : BaseVisual<BattleGUIFeature>
    {
        [SerializeField] private GameObject _gui;
        [SerializeField] private GameObject _controls;
        
        [SerializeField] private Button _attackButton;
        [SerializeField] private Button _moveButton;
        [SerializeField] private Button _rotateButton;
        
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _level;
        [SerializeField] private UnityEngine.UI.Image _photo;

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

        public void UpdateUnitInfo(string unitName, int level, Sprite photo)
        {
            _name.text = $"{unitName}";
            _level.text = $"{level}";
            
            if (_photo != null && photo != null)
            {
                _photo.sprite = photo;
            }
        }

        public void ShowUnitSelection(bool isMyUnit)
        {
            _gui.SetActive(true);
            _controls.SetActive(isMyUnit);
        }

        public void HideUnitSelection()
        {
            _gui.SetActive(false);
        }
    }
}