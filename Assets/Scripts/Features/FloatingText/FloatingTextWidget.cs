using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game
{
    public class FloatingTextWidget : Widget
    {
        [SerializeField] private TextMeshProUGUI _textMesh;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _textContainer;

        private FloatingTextPresetSO _preset;
        private CancellationTokenSource _animationCts;

    private System.Action _onComplete;

        public void SetText(string text, FloatingTextPresetSO preset, System.Action onComplete = null)
        {
            _preset = preset;
            _textMesh.text = text;
            _textMesh.color = preset.TextColor;
            _onComplete = onComplete;

            // Reset initial state
            if (_textContainer != null)
            {
                _textContainer.anchoredPosition = Vector2.zero;
            }
            transform.localScale = Vector3.one * preset.StartScale;
            _canvasGroup.alpha = 1.0f;

            // Start the animation
            PlayAnimation().Forget();
        }

        private async UniTask PlayAnimation()
        {
            // Cancel any existing animation
            _animationCts?.Cancel();
            _animationCts = new CancellationTokenSource();

            try
            {
                // Create animation sequence
                Sequence sequence = DOTween.Sequence();

                // Position tween - animate the text container upward locally
                if (_textContainer != null)
                {
                    Vector2 startPosition = _textContainer.anchoredPosition;
                    Vector2 endPosition = startPosition + new Vector2(0, _preset.RiseAmount);
                    sequence.Join(_textContainer.DOAnchorPos(endPosition, _preset.Duration).SetEase(Ease.OutQuad));
                }

                // Scale tween
                sequence.Join(
                    transform.DOScale(1f, _preset.Duration)
                    .SetEase(_preset.ScaleCurve)
                    .ChangeValues(Vector3.zero, Vector3.one)
                );

                // Alpha tween
                sequence.Join(_canvasGroup.DOFade(1, _preset.Duration).SetEase(_preset.AlphaCurve));

                // Start the sequence
                await sequence.Play().AsyncWaitForCompletion();

                // Call completion callback instead of destroying directly
                _onComplete?.Invoke();
            }
            catch (OperationCanceledException)
            {
                // Animation was cancelled, cleanup
                DOTween.Kill(transform);
                if (_textContainer != null)
                {
                    DOTween.Kill(_textContainer);
                }
                _onComplete?.Invoke();
            }
        }

        private void OnDestroy()
        {
            _animationCts?.Cancel();
            _animationCts?.Dispose();
            DOTween.Kill(transform);
            if (_textContainer != null)
            {
                DOTween.Kill(_textContainer);
            }
        }
    }
}
