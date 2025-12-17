using System.Collections.Generic;
using System.Linq;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class BattleGUIVisual : BaseVisual<BattleGUIFeature>
    {
        [System.Serializable]
        public class ButtonType
        {
            public GameObject Root;
            public Button Button;
            public AbilityMode Ability;
        }
        
        [SerializeField] private TurnBarVisual _turnBarVisual;
        [SerializeField] private List<ButtonType> _buttonTypes;

        [SerializeField] private GameObject _myUnitBack;
        [SerializeField] private GameObject _otherUnitBack;
        
        [SerializeField] private GameObject _gui;
        [SerializeField] private GameObject _controls;
        
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _level;
        [SerializeField] private UnityEngine.UI.Image _photo;
        
        private void Awake()
        {
            _buttonTypes.FirstOrDefault(b => b.Ability == AbilityMode.Attack).Button.onClick.AddListener(OnAttackButtonClicked);
            _buttonTypes.FirstOrDefault(b => b.Ability == AbilityMode.Move).Button.onClick.AddListener(OnMoveButtonClicked);
            _buttonTypes.FirstOrDefault(b => b.Ability == AbilityMode.Rotate).Button.onClick.AddListener(OnRotateButtonClicked);
            _buttonTypes.FirstOrDefault(b => b.Ability == AbilityMode.Wait).Button.onClick.AddListener(OnWaitButtonClicked);
            _buttonTypes.FirstOrDefault(b => b.Ability == AbilityMode.RangeAttack).Button.onClick.AddListener(OnShootButtonClicked);
            _buttonTypes.FirstOrDefault(b => b.Ability == AbilityMode.CleaveAttack).Button.onClick.AddListener(OnCleaveButtonClicked);
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

        private void OnWaitButtonClicked()
        {
            Feature.OnWaitButtonClicked();
        }

        private void OnShootButtonClicked()
        {
            Feature.OnShootButtonClicked();
        }
        
        private void OnCleaveButtonClicked()
        {
            Feature.OnCleaveButtonClicked();
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
            _myUnitBack.SetActive(isMyUnit);
            _otherUnitBack.SetActive(!isMyUnit);
            _gui.SetActive(true);
            _turnBarVisual.SetVisibility(isMyUnit);
            _controls.SetActive(isMyUnit);
        }

        public void HideUnitSelection()
        {
            _gui.SetActive(false);
        }

        public void SetUnitActions(IEnumerable<AbilityMode> select)
        {
            foreach (var button in _buttonTypes)
            {
                if (!select.Contains(button.Ability))
                {
                    button.Root.SetActive(false);
                }
                else
                {
                    button.Root.SetActive(true);
                }
            }
        }
    }
}