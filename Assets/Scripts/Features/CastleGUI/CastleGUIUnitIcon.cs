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

        public void SetupUnit(Sprite icon, string name, int goldCost)
        {
            _unitIcon.sprite = icon;
            _unitName.text = name;
            _unitGoldPrice.text = goldCost.ToString();
        }
    }
}