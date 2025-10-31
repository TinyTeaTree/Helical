using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChessRaid
{
    public class ChampionPanel : WagMonoton<ChampionPanel>
    {
        [SerializeField] private GameObject _activator;
        [SerializeField] private Image _profile;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _hp;
        [SerializeField] private Slider _hpBar;
        [SerializeField] private Slider _apBar;

        [SerializeField] private TMP_Text _attack;
        [SerializeField] private TMP_Text _defense;
        [SerializeField] private TMP_Text _accuracy;
        [SerializeField] private TMP_Text _resistance;
        [SerializeField] private TMP_Text _speed;

        [SerializeField] private Button _expandButton;
        [SerializeField] private Image _expandSection;

        private bool _isExpanded;

        private void Start()
        {
            BattleEventBus.OnSelectionChanged.AddListener(OnSelectionChanged);
            BattleEventBus.TurnActionChanged.AddListener(OnTurnActionChanged);

            TurnOff();

            _expandButton.onClick.AddListener(OnExpandButtonClicked);

            _expandSection.fillAmount = 0;
        }

        private void OnTurnActionChanged()
        {
            OnSelectionChanged();
        }

        private void OnExpandButtonClicked()
        {
            if (!_isExpanded)
            {
                Expand();
                _isExpanded = true;
            }
            else
            {
                Retract();
                _isExpanded = false;
            }
        }

        private void Retract()
        {
            this.DOKill();
            (_expandButton.transform as RectTransform).DOKill();

            DOTween.To(() => 1f, val =>
            {
                _expandSection.fillAmount = val;
            }, 0f, 0.3f).SetId(this);

            (_expandButton.transform as RectTransform).DORotate(new Vector3(0f, 0f, 180f), 0.3f);
        }

        private void Expand()
        {
            this.DOKill();
            (_expandButton.transform as RectTransform).DOKill();

            DOTween.To(() => 0f, val =>
            {
                _expandSection.fillAmount = val;
            }, 1f, 0.3f).SetId(this);

            (_expandButton.transform as RectTransform).DORotate(new Vector3(0f, 0f, 0f), 0.3f);
        }

        private void OnSelectionChanged()
        {
            if (SelectionManager.Single.SelectedHex?.Champion != null)
            {
                TurnOn();
                ResolveInfo();
            }
            else
            {
                TurnOff();
            }
        }

        private void ResolveInfo()
        {
            var champion = SelectionManager.Single.SelectedHex?.Champion;

            if (champion == null)
                return;

            _name.text = champion.Def.Name;
            _profile.sprite = champion.Def.ProfilePicture;
            _hp.text = champion.Health.ToString();

            var state = TurnModel.Single.GetChampionState(SelectionManager.Single.SelectedHex.Champion);

            _hpBar.value = champion.Health / (float)champion.Def.Stats.Health;
            _apBar.value = state.ActionPoints / ((float)champion.Def.Stats.Speed * 10);

            _attack.text = $"Att: {champion.Def.Stats.Attack}";
            _defense.text = $"DEF: {champion.Def.Stats.Defense}";
            _accuracy.text = $"ACC: {champion.Def.Stats.Accuracy}";
            _resistance.text = $"RST: {champion.Def.Stats.Resistance}";
            _speed.text = $"SPD: {champion.Def.Stats.Speed}";
        }

        private void TurnOn()
        {
            _activator.gameObject.SetActive(true);
        }

        private void TurnOff()
        {
            _activator.gameObject.SetActive(false);

            this.DOKill();
            (_expandButton.transform as RectTransform).rotation = Quaternion.Euler(0f, 0f, 180f);
            _isExpanded = false;
            _expandSection.fillAmount = 0f;
        }
    }
}