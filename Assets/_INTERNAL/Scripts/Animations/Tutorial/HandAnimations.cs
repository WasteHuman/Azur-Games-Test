using DG.Tweening;
using UnityEngine;

namespace Animations.Tutorial
{
    public class HandAnimations : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float _animationDuration = 1.0f;
        [SerializeField] private Quaternion _rotationTarget;
        [SerializeField] private Vector2 _moveTarget;

        private Tween _moveTween;
        private Tween _rotationTween;

        private void OnEnable()
        {
            var rectTransform = GetComponent<RectTransform>();

            _moveTween?.Kill();
            _rotationTween?.Kill();

            _moveTween = rectTransform
                .DOAnchorPos(_moveTarget, _animationDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
            _rotationTween = rectTransform
                .DORotateQuaternion(_rotationTarget, _animationDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }

        private void OnDisable()
        {
            _moveTween.Kill();
            _rotationTween.Kill();
        }
    }
}