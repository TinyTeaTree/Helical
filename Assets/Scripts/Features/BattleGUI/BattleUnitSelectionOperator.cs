using UnityEngine;

namespace Game
{
    public class BattleUnitSelectionOperator : MonoBehaviour
    {
        private void Awake()
        {
            // Start hidden
            Hide();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}