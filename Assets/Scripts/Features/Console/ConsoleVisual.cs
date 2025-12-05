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
            if (_inputField != null)
            {
                _inputField.onEndEdit.AddListener(OnInputSubmitted);
                _inputField.gameObject.SetActive(false); // Start hidden
            }

            if (_submitButton != null)
            {
                _submitButton.onClick.AddListener(SubmitInput);
                _submitButton.gameObject.SetActive(false); // Start hidden
            }

            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(() => Feature.HideConsole());
                _closeButton.gameObject.SetActive(false); // Start hidden
            }
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
            if (_inputField != null) _inputField.gameObject.SetActive(true);
            if (_submitButton != null) _submitButton.gameObject.SetActive(true);
            if (_closeButton != null) _closeButton.gameObject.SetActive(true);

            ActivateInput();
        }

        public void HideConsole()
        {
            // Disable UI elements
            if (_inputField != null) _inputField.gameObject.SetActive(false);
            if (_submitButton != null) _submitButton.gameObject.SetActive(false);
            if (_closeButton != null) _closeButton.gameObject.SetActive(false);
        }

        public void ActivateInput()
        {
            if (_inputField != null)
            {
                _inputField.text = "";
                _inputField.ActivateInputField();
                _inputField.Select();
            }
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
            if (_inputField != null && !string.IsNullOrEmpty(_inputField.text))
            {
                Feature.ExecuteCommand(_inputField.text);
                _inputField.text = "";
                Feature.HideConsole();
            }
        }
    }
}
