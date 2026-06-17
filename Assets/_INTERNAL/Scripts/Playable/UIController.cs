using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Playable
{
    public class UIController : MonoBehaviour
    {
        [Header("Action buttons")]
        [SerializeField] private Button _buyHeroButton;

        [Space(5), Header("Other UI")]
        [SerializeField] private TextMeshProUGUI _heroPriceText;
        [SerializeField] private TextMeshProUGUI _playerCoinsText;
        [SerializeField] private Canvas _canvas;

        [Space(5), Header("Rewards from Enemy Settings")]
        [SerializeField] private GameObject _coinPrefab;
        [SerializeField] private int _coinsCount;

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

        public void HandleEnemyDied(Transform enemyPosition)
        {
            GameObject[] coins = new GameObject[_coinsCount];
            Vector3 offset;

            for(int i = 0; i < _coinsCount; i++)
            {
                offset = new(Random.Range(0, 30), Random.Range(0, 30));
                coins[i] = Instantiate(_coinPrefab, _canvas.transform);
                coins[i].transform.position = enemyPosition.position + offset;
            }
        }

        public void HandleCurrentPriceChanged(int price) => _heroPriceText.text = price.ToString();
        public void HandlePlayerCoinsChanged(int coins) => _playerCoinsText.text = coins.ToString();

        private void HandleBuyHeroButtonClick() => OnBuyHeroButtonClicked?.Invoke();
    }
}