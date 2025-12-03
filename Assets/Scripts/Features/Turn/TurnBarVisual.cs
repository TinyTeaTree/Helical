using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Game
{
    public class TurnBarVisual  : BaseVisual<TurnFeature>
    {
        [System.Serializable]
        public class TurnWidget
        {
            public GameObject Widget;
            public AbilityMode Ability;
        }

        [SerializeField] private Transform _root;
        [SerializeField] private List<TurnWidget> _widgets;
        
        private List<TurnWidget> _orderedWidgets =  new List<TurnWidget>();

        private void Clean()
        {
            foreach (var widget in _widgets)
            {
                widget.Widget.SetActive(false);
            }

            foreach (var widget in _orderedWidgets)
            {
                Destroy(widget.Widget.gameObject);
            }
            
            _orderedWidgets.Clear();
        }
        
        public void SetVisibility(bool visible)
        {
            _root.gameObject.SetActive(visible);
        }

        public void ShowMyTurn(BattleUnitData.Turn unitDataTurnOrder)
        {
            Clean();

            foreach (var action in unitDataTurnOrder.Actions)
            {
                var widget = _widgets.Find(widget => widget.Ability == action.Ability);
                widget = new TurnWidget()
                {
                    Ability = action.Ability,
                    Widget = Instantiate(widget.Widget, widget.Widget.transform.parent)
                };
                
                widget.Widget.SetActive(true);
                
                _orderedWidgets.Add(widget);
            }
        }
    }
}