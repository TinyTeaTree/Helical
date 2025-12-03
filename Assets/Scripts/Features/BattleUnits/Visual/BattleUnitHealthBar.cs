using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class BattleUnitHealthBar : Widget
    {
        [SerializeField] private Image _fill;

        public void UpdateFill(float fillAmount)
        {
            if (_fill != null)
            {
                _fill.fillAmount = Mathf.Clamp01(fillAmount);
            }
        }
    }
}