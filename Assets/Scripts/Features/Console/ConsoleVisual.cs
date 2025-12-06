using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class ConsoleVisual : BaseVisual<ConsoleFeature>
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Button _submitButton;
        [SerializeField] private Button _closeButton;

        private void Awake()
        {
            _inputField.onEndEdit.AddListener(OnInputSubmitted);
            _inputField.gameObject.SetActive(false); // Start hidden

            _submitButton.onClick.AddListener(SubmitInput);
            _submitButton.gameObject.SetActive(false); // Start hidden

            _closeButton.onClick.AddListener(() => Feature.HideConsole());
            _closeButton.gameObject.SetActive(false); // Start hidden
        }

        public void SetFeature(ConsoleFeature feature)
        {
            _baseFeature = feature;
        }

        private void Update()
        {
            // Check for Left Ctrl + Enter to toggle console
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Return))
            {
                if (Feature != null)
                {
                    Feature.ToggleConsole();
                }
            }
        }

        public void ShowConsole()
        {
            // Enable UI elements
            _inputField.gameObject.SetActive(true);
            _submitButton.gameObject.SetActive(true);
            _closeButton.gameObject.SetActive(true);

            ActivateInput();
        }

        public void HideConsole()
        {
            // Disable UI elements
            _inputField.gameObject.SetActive(false);
            _submitButton.gameObject.SetActive(false);
            _closeButton.gameObject.SetActive(false);
        }

        public void ActivateInput()
        {
            _inputField.text = "";
            _inputField.ActivateInputField();
            _inputField.Select();
        }

        private void OnInputSubmitted(string input)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                SubmitInput();
            }
        }

        private void SubmitInput()
        {
            if (!string.IsNullOrEmpty(_inputField.text))
            {
                Feature.ExecuteCommand(_inputField.text);
                _inputField.text = "";
                Feature.HideConsole();
            }
        }
    }
}
