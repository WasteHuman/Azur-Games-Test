using DG.Tweening;
using Playable;
using System;
using UnityEngine;

namespace Animations.Tutorial
{
    public class MergeTutorialController : MonoBehaviour
    {
        [Header("Tutorial Setup")]
        [SerializeField] private GameObject _tutorPanel;
        [SerializeField] private RectTransform _handTransform;
        [SerializeField] private RectTransform _canvasTransform;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private MergeController _mergeController;

        [Space(5), Header("Animation Settings")]
        [SerializeField] private float _animationDuration = 1.0f;

        private Tween _mergeTutorTween;

        public event Action OnTutorialCompleted;

        private void Start()
        {
            _mergeController.OnMergeCompleted += HandleCompletedMerge;
        }

        private void OnDestroy()
        {
            _mergeController.OnMergeCompleted -= HandleCompletedMerge;
        }

        public void AnimateMerge(Transform firstUnit, Transform secondUnit)
        {
            _mergeTutorTween?.Kill();

            var startScreenPos = WorldToCanvasPosition(firstUnit);
            var targetScreenPos = WorldToCanvasPosition(secondUnit);

            _handTransform.anchoredPosition = startScreenPos;

            _mergeTutorTween = _handTransform
                .DOAnchorPos(targetScreenPos, _animationDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }

        public void Open()
        {
            _tutorPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        public void Close()
        {
            _tutorPanel.SetActive(false);
            Time.timeScale = 1f;
            _mergeTutorTween.Kill();
        }

        private Vector2 WorldToCanvasPosition(Transform target)
        {
            var screenPoint = _mainCamera.WorldToScreenPoint(target.position);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasTransform,
                screenPoint,
                null,
                out Vector2 localPoint);

            return localPoint;
        }

        private void HandleCompletedMerge() => OnTutorialCompleted?.Invoke();
    }
}