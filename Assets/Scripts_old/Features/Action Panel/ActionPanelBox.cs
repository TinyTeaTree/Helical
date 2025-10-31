namespace ChessRaid
{
    public class ActionPanelBox : DataBox
    {
        public override string Id => "Action Panel";

        private ActionButton _selectedAction;
        public ActionButton SelectedAction
        {
            get
            {
                return _selectedAction;
            }
            set
            {
                _selectedAction = value;
                OnChanged.Invoke();
            }
        }

        public ActionType ActionType => _selectedAction == null ? ActionType.None : _selectedAction.ActionType;
    }
}