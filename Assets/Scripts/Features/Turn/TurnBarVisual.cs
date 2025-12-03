using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private Image _turnFill; //TODO: Use This
        
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

            // Calculate total action points used
            int totalActionPointsUsed = unitDataTurnOrder.Actions.Sum(action => action.ActionPoints);

            foreach (var action in unitDataTurnOrder.Actions)
            {
                var widgetPrefab = _widgets.Find(widget => widget.Ability == action.Ability);
                var widget = new TurnWidget()
                {
                    Ability = action.Ability,
                    Widget = Instantiate(widgetPrefab.Widget, _root)
                };

                // Position widget based on action point end (start + duration) (1000 units = 100 action points)
                float actionPointRatio = (float)(action.ActionPointStart + action.ActionPoints) / 100f;
                float xOffset = actionPointRatio * 1000f;
                var rectTransform = widget.Widget.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(xOffset, rectTransform.anchoredPosition.y);

                widget.Widget.SetActive(true);

                _orderedWidgets.Add(widget);
            }

            // Fill image with the amount of Action Points by ratio from 100 action points
            if (_turnFill != null)
            {
                float fillRatio = Mathf.Min((float)totalActionPointsUsed / 100f, 1f);
                _turnFill.fillAmount = fillRatio;
            }
        }
    }
}