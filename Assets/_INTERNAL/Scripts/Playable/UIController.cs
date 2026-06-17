using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Playable
{
    public class UIController : MonoBehaviour
    {
        [Header("Action buttons")]
        [SerializeField] private Button _buyHeroButton;

        [Space(5), Header("Other UI")]
        [SerializeField] private TextMeshProUGUI _heroCostText;

        public event Action OnBuyHeroButtonClicked;

        private void Start()
        {
            if (_buyHeroButton == null)
                return;

            _buyHeroButton.onClick.AddListener(HandleBuyHeroButtonClick);
        }

        private void OnDestroy()
        {
            if (_buyHeroButton == null)
                return;

            _buyHeroButton.onClick.RemoveListener(HandleBuyHeroButtonClick);
        }

        private void HandleBuyHeroButtonClick() => OnBuyHeroButtonClicked?.Invoke();
    }
}