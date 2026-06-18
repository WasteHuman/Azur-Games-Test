using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Animations.Tutorial
{
    public class HireTutorialController : MonoBehaviour
    {
        [SerializeField] private GameObject _tutorPanel;
        [SerializeField] private TextMeshProUGUI _heroPriceText;
        [SerializeField] private Button _buyHeroButton;

        public event Action OnBuyButtonClicked;
        public event Action OnTutorialCompleted;

        private void OnEnable()
        {
            if (_buyHeroButton == null)
                return;

            _buyHeroButton.onClick.AddListener(HandleBuyHeroButtonClick);
        }

        private void OnDisable()
        {
            if (_buyHeroButton == null)
                return;

            _buyHeroButton.onClick.RemoveListener(HandleBuyHeroButtonClick);
        }

        public void Open()
        {
            _tutorPanel.SetActive(true);
            Time.timeScale = 0.0f;
        }

        public void Close()
        {
            _tutorPanel.SetActive(false);
            Time.timeScale = 1.0f;
            OnTutorialCompleted?.Invoke();
        }

        public void Initialize(int heroPrice) => _heroPriceText.text = heroPrice.ToString();

        private void HandleBuyHeroButtonClick() => OnBuyButtonClicked?.Invoke();
    }
}