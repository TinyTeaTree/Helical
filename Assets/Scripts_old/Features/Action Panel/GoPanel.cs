using UnityEngine;
using UnityEngine.UI;

namespace ChessRaid
{
    public class GoPanel : WagMonoton<GoPanel>
    {
        [SerializeField] private Button _goButton;

        private void Start()
        {
            _goButton.onClick.AddListener(OnGoClicked);
        }

        private void OnGoClicked()
        {
            TurnManager.Single.ExecuteTurn().Forget();
        }
    }
}