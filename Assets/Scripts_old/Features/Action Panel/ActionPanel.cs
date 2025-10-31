using UnityEngine;

namespace ChessRaid
{
    public class ActionPanel : WagMonoton<ActionPanel>
    {
        [SerializeField] private ActionButton[] _allButtons;
        
        private ActionPanelBox _box;

        public ActionType SelectedAction => _box.ActionType;

        public WagEvent OnBoxChanged => _box.OnChanged;

        private void Start()
        {
            BattleEventBus.OnSelectionChanged.AddListener(OnSelectionChanged);
            _box = DataWarehouse.Single.GetBox<ActionPanelBox>();
            OnSelectionChanged();
        }

        private void OnSelectionChanged()
        {
            TurnOff();

            var selectedHex = SelectionManager.Single.SelectedHex;

            if (selectedHex == null)
                return;

            var champion = selectedHex.Champion;
            if (champion == null)
                return;

            if (champion.Team != Team.Home)
                return;

            foreach (var b in _allButtons)
            {
                b.SetVisiblity(true);
            }
        }

        private void TurnOff()
        {
            if (_box.SelectedAction != null)
            {
                UnSelect(_box.SelectedAction);
            }

            foreach (var b in _allButtons)
            {
                b.SetVisiblity(false);
            }
        }

        public void Select(ActionButton actionButton)
        {
            if(_box.SelectedAction != null)
            {
                UnSelect(_box.SelectedAction);
            }

            actionButton.MarkSelection(true);
            _box.SelectedAction = actionButton;

            BattleEventBus.TurnActionChanged.Invoke();
        }

        public void UnSelect(ActionButton actionButton)
        {
            if (_box.SelectedAction == null)
                return;

            if(_box.SelectedAction != actionButton)
            {
                Debug.LogWarning($"{_box.SelectedAction} is not equal {actionButton}");
            }

            _box.SelectedAction.MarkSelection(false);

            _box.SelectedAction = null;
        }
    }
}