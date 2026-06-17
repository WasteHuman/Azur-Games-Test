using DG.Tweening;
using UnityEngine;

namespace Animations.Tutorial
{
    public class ArcherCardAnimations : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float _moveAnimDuration = 0.15f;
        [SerializeField] private Vector2 _moveTarget;

        [Space(5), Header("Light Card Animation Settings")]
        [SerializeField] private RectTransform _lightCardTransform;
        [SerializeField] private float _lightCardMoveAnimDuration = 0.15f;
        [SerializeField] private Vector2 _lightCardMoveTarget;

        private Tween _lightCardMoveTween;
        private Tween _moveTween;

        private void OnEnable()
        {
            var rectTransform = GetComponent<RectTransform>();
            _moveTarget.x = rectTransform.position.x;

            _moveTween?.Kill();
            _lightCardMoveTween?.Kill();

            _moveTween = rectTransform
                .DOAnchorPos(_moveTarget, _moveAnimDuration)
                .SetLoops(-1, LoopType.Yoyo);

            _lightCardMoveTween = _lightCardTransform
                .DOAnchorPos(_lightCardMoveTarget, _lightCardMoveAnimDuration)
                .SetLoops(-1, LoopType.Restart);
        }

        private void OnDisable()
        {
            _moveTween.Kill();
            _lightCardMoveTween.Kill();
        }
    }
}