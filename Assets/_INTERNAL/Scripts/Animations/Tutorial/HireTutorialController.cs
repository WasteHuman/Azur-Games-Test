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

        public void Initialize(int heroPrice) => _heroPriceText.text = heroPrice.ToString();

        private void HandleBuyHeroButtonClick() => OnBuyButtonClicked?.Invoke();
    }
}