using DG.Tweening;
using UnityEngine;

namespace Animations.Tutorial
{
    public class ButtonAnimations : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float _breathAnimDuration = 1.0f;
        [SerializeField] private Vector2 _breathTargetScale = new(0.85f, 0.85f);

        private RectTransform _rectTransform;

        private Tween _breathTween;

        private void OnEnable()
        {
            if(_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();

            _breathTween?.Kill();

            _breathTween = _rectTransform
                .DOScale(_breathTargetScale, _breathAnimDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }

        private void OnDisable()
        {
            _breathTween.Kill();
        }

        public void StartAnimations()
        {
            if (_rectTransform == null)
                return;

            _breathTween?.Kill();

            _breathTween = _rectTransform
                .DOScale(_breathTargetScale, _breathAnimDuration)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void StopAnimations()
        {
            _breathTween.Kill();
        }
    }
}