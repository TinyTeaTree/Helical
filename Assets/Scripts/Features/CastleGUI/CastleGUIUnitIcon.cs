using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class CastleGUIUnitIcon : MonoBehaviour
    {
        [SerializeField] private Image _unitIcon;
        [SerializeField] private TMP_Text _unitName;
        [SerializeField] private TMP_Text _unitGoldPrice;

        [SerializeField] private Button _unitButton;

        private string _unitId;
        private System.Action<string> _onPurchaseRequested;

        public void SetupUnit(System.Action<string> onPurchaseRequested, string unitId, Sprite icon, string name, int goldCost)
        {
            _onPurchaseRequested = onPurchaseRequested;
            _unitId = unitId;
            _unitIcon.sprite = icon;
            _unitName.text = name;
            _unitGoldPrice.text = goldCost.ToString();

            // Wire up button click
            _unitButton.onClick.RemoveAllListeners();
            _unitButton.onClick.AddListener(OnPurchaseButtonClicked);
        }

        private void OnPurchaseButtonClicked()
        {
            _onPurchaseRequested?.Invoke(_unitId);
        }
    }
}