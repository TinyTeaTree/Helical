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
        [SerializeField] private Transform _unitsRoot;

        private List<CastleGUIUnitIcon> _unitIcons;

        private void Awake()
        {
            _unitIcons = new List<CastleGUIUnitIcon>();
            _unitIconPrefab.gameObject.SetActive(false);
        }

        public void ShowCastleSelection(Vector2Int coordinate, string castleName, List<PurchasableUnitData> purchasableUnits)
        {
            _root.SetActive(true);

            // Update location info
            _locationLabel.text = $"Location: ({coordinate.x}, {coordinate.y})";

            // Update castle name
            _nameLabel.text = castleName;

            // Clear existing unit icons
            ClearUnitIcons();

            // Create unit icons for each purchasable unit
            foreach (var unitData in purchasableUnits)
            {
                CreateUnitIcon(unitData);
            }
        }

        public void HideCastleSelection()
        {
            _root.SetActive(false);
            ClearUnitIcons();
        }

        private void CreateUnitIcon(PurchasableUnitData unitData)
        {
            var unitIcon = Instantiate(_unitIconPrefab, _unitsRoot);
            unitIcon.SetupUnit(OnUnitPurchaseRequested, unitData.UnitId, unitData.UnitIcon, unitData.UnitName, unitData.GoldCost);
            unitIcon.gameObject.SetActive(true);
            _unitIcons.Add(unitIcon);
        }

        private void OnUnitPurchaseRequested(string unitId)
        {
            Feature.PurchaseUnit(unitId);
        }

        private void ClearUnitIcons()
        {
            foreach (var unitIcon in _unitIcons)
            {
                if (unitIcon != null)
                {
                    Destroy(unitIcon.gameObject);
                }
            }
            _unitIcons.Clear();
        }
    }
}