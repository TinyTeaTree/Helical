using Core;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Game/FloatingTextPreset", fileName = "FloatingTextPreset")]
    public class FloatingTextPresetSO : BaseSO
    {
        [Header("Animation Settings")]
        [SerializeField] private float _duration = 1.0f;
        [SerializeField] private float _riseAmount = 50.0f;

        [Header("Scale Animation")]
        [SerializeField] private AnimationCurve _scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 0.8f);

        [Header("Alpha Animation")]
        [SerializeField] private AnimationCurve _alphaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        [Header("Text Settings")]
        [SerializeField] private Color _textColor = Color.white;
        [SerializeField] private float _startScale = 1.0f;
        [SerializeField] private float _endScale = 0.8f;

        public float Duration => _duration;
        public float RiseAmount => _riseAmount;
        public AnimationCurve ScaleCurve => _scaleCurve;
        public AnimationCurve AlphaCurve => _alphaCurve;
        public Color TextColor => _textColor;
        public float StartScale => _startScale;
        public float EndScale => _endScale;
    }
}
