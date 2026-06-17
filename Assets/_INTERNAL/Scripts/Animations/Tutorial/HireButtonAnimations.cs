using DG.Tweening;
using UnityEngine;

namespace Animations.Tutorial
{
    public class HireButtonAnimations : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float _breathAnimDuration = 1.0f;
        [SerializeField] private Vector2 _breathTargetScale = new(0.85f, 0.85f);

        private Tween _breathTween;

        private void OnEnable()
        {
            var rectTransform = GetComponent<RectTransform>();

            _breathTween?.Kill();

            _breathTween = rectTransform
                .DOScale(_breathTargetScale, _breathAnimDuration)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void OnDisable()
        {
            _breathTween.Kill();
        }
    }
}