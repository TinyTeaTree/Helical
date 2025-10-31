using System.Collections;
using Core;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class LoadingScreenPage : MonoBehaviour
    {
        [SerializeField] private Image _fillBar;
        [SerializeField] private TMP_Text _fillText;
        [SerializeField] private bool _activateProgressLoadingText;
        [SerializeField] private float _dotDelay;

        private Coroutine _progressRoutine;

        private void Awake()
        {
            if(_activateProgressLoadingText && _fillText != null)
            {
                _progressRoutine = StartCoroutine(ProgressRoutine());
            }
        }

        private void SetProgress(float progress, string progressText)
        {
            if (_progressRoutine != null)
            {
                StopCoroutine(_progressRoutine);
                _progressRoutine = null;
            }

            if (progressText.HasContent())
            {
                if (_fillText)
                {
                    _fillText.text = progressText;
                }
            }

            if (_fillBar)
            {
                _fillBar.fillAmount = progress;
            }
        }

        IEnumerator ProgressRoutine()
        {
            string[] labels = new[] { $"", $".", $". .", $". . ." };
            int index = 0;

            while (true)
            {
                _fillText.text = labels[index];

                yield return new WaitForSeconds(_dotDelay);

                index++;
                if(index >= labels.Length)
                {
                    index = 0;
                }
            }
        }
    }
}