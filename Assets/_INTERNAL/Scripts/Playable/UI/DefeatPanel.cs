using UnityEngine;
using UnityEngine.UI;

namespace Playable.UI
{
    public class DefeatPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] Button _button;

        private void Start()
        {
            if (_button == null || _panel == null)
                return;

            _panel.SetActive(false);
            _button.onClick.AddListener(HandleButtonClick);
        }

        private void OnDestroy()
        {
            if (_button == null)
                return;

            _button.onClick.RemoveListener(HandleButtonClick);
        }

        public void Open() => _panel.SetActive(true);
        public void Close() => _panel.SetActive(false);

        private void HandleButtonClick() => Debug.Log("[Defeat Panel] Buton Clicked");
    }
}