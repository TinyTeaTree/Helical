using UnityEngine;

namespace ChessRaid
{
    public class Hex : MonoBehaviour
    {
        [SerializeField] MeshRenderer _hexFloor;
        [SerializeField] HexDef _def;
        [SerializeField] MeshRenderer _indicatorRenderer;

        public Coord Location;

        public bool IsSelected => SelectionManager.Single.SelectedHex == this;

        public Champion Champion { get; private set; }

        public void SetChampion(Champion champion)
        {
            Champion = champion;
        }

        public void ToggleSelect(bool select)
        {
            _hexFloor.material.color = select ? _def.SelectedColor : _def.NotSelectedColor;
        }

        public void ToggleIndicator(bool indicate)
        {
            _indicatorRenderer.gameObject.SetActive(indicate);
        }

        public void SetActionSelection(ActionType action)
        {
            if(IsSelected)
            {
                Debug.LogWarning($"Can Action Select a Selected Hex, Not Definied");
                return;
            }

            switch (action)
            {
                case ActionType.None:
                    _hexFloor.material.color = _def.NotSelectedColor;
                    break;
                case ActionType.Attack:
                    _hexFloor.material.color = _def.AttackColor;
                    break;
                case ActionType.Move:
                    _hexFloor.material.color = _def.MoveColor;
                    break;
                case ActionType.Rotate:
                    _hexFloor.material.color = _def.RotateColor;
                    break;
            }
        }

        public void Clear()
        {
            ToggleSelect(false);
            ToggleIndicator(false);
        }
    }
}