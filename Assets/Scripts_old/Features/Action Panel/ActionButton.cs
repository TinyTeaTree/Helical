using UnityEngine;
using UnityEngine.UI;

namespace ChessRaid
{
    public class ActionButton : MonoBehaviour
    {
        [SerializeField] private ActionPanel _panel;
        [SerializeField] private Button _button;
        [SerializeField] private Image _mark;
        [SerializeField] private ActionType _actionType;


        private bool IsSelected => _panel.SelectedAction == _actionType;


        public ActionType ActionType => _actionType;

        private void Start()
        {
            _button.onClick.AddListener(HandleClick);
            MarkSelection(false);
        }

        private void HandleClick()
        {
            if (IsSelected)
            {
                _panel.UnSelect(this);
            }
            else
            {
                _panel.Select(this);
            }
        }

        public void MarkSelection(bool selected)
        {
            _mark.color = selected ? Color.green : Color.white;
        }

        public void SetVisiblity(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}