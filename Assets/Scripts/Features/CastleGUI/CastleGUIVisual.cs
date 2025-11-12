using System.Collections.Generic;
using Core;
using TMPro;
using UnityEngine;

namespace Game
{
    public class CastleGUIVisual : BaseVisual<CastleGUIFeature>
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private TMP_Text _locationLabel;
        [SerializeField] private TMP_Text _nameLabel;

        [SerializeField] private CastleGUIUnitIcon _unitIconPrefab;

        private List<CastleGUIUnitIcon> _unitIcons;

        public void ShowCastleSelection(Vector2Int coordinate)
        {
            _root.SetActive(true);

            // Update location info
            _locationLabel.text = $"Location: ({coordinate.x}, {coordinate.y})";

            // For now, just show a basic name
            // TODO: Get castle name from castle data
            _nameLabel.text = "Castle";
        }

        public void HideCastleSelection()
        {
            _root.SetActive(false);
        }
    }
}