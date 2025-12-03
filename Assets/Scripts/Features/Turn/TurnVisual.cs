using Core;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class TurnVisual : BaseVisual<TurnFeature>
    {
        [SerializeField] private TMP_Text _turnLabel;
        [SerializeField] private Button _turnButton;

        void Start()
        {
            _turnButton.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            DJ.Play(DJ.Click_Sound);
            Feature.OnTurnClicked();
        }

        public void SetTurnData()
        {
            _turnLabel.text = "Turn: " + Feature.Record.Turn;
        }
    }
}