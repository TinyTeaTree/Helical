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
                // Store initial position
                Vector3 startPosition = transform.localPosition;
                Vector3 endPosition = startPosition + new Vector3(0, _preset.RiseAmount, 0);

                // Create animation sequence
                Sequence sequence = DOTween.Sequence();

                // Position tween
                sequence.Join(transform.DOLocalMove(endPosition, _preset.Duration).SetEase(Ease.OutQuad));

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
                _onComplete?.Invoke();
            }
        }

        private void OnDestroy()
        {
            _animationCts?.Cancel();
            _animationCts?.Dispose();
            DOTween.Kill(transform);
        }
    }
}
