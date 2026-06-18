using UnityEngine;
using UnityEngine.UI;

namespace Playable.UI
{
    public class HealthBarView : MonoBehaviour
    {
        [SerializeField] private Vector3 _offset = new(0f, 10f, 0f);
        [SerializeField] private Transform _target;
        [SerializeField] private Camera _camera;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _fill;

        private void Start()
        {
            if (_camera == null)
                _camera = Camera.main;
        }

        public void Init(Transform target, Transform parent)
        {
            _target = target;
            transform.SetParent(parent);
            transform.SetAsFirstSibling();
        }

        public void SetHealth(float value) => _fill.fillAmount = value;

        private void LateUpdate()
        {
            if (_target == null || _camera == null)
                return;

            Vector3 screenPos = _camera.WorldToScreenPoint(_target.position + _offset);
            _rectTransform.position = screenPos;
        }
    }
}