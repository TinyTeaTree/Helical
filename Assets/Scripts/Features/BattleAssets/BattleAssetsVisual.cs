using Core;
using TMPro;
using UnityEngine;

namespace Game
{
    public class BattleAssetsVisual : BaseVisual<BattleAssetsFeature>
    {
        [SerializeField] private GameObject _currencyBarRoot;
        [SerializeField] private TMP_Text _goldAmount;

        private void Awake()
        {
            // Start hidden
            _currencyBarRoot.SetActive(false);
        }

        public void Show()
        {
            _currencyBarRoot.SetActive(true);
            UpdateGoldDisplay();
        }

        public void Hide()
        {
            _currencyBarRoot.SetActive(false);
        }

        public void UpdateGoldDisplay()
        {
            _goldAmount.text = Feature.GoldAmount.ToString();
        }
    }
}