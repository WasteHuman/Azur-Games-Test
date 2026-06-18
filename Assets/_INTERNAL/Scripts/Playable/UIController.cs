using Animations.Tutorial;
using DG.Tweening;
using Playable.UI;
using System;
using System.Collections.Generic;
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
        [SerializeField] private ButtonAnimations _buyHeroButtonAnimation;

        [Space(5), Header("Other UI")]
        [SerializeField] private TextMeshProUGUI _heroPriceText;
        [SerializeField] private TextMeshProUGUI _playerCoinsText;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _canvasTransform;
        [SerializeField] private RectTransform _coinsTargetTransform;
        [SerializeField] private GameObject _uiPanel;
        [SerializeField] private DefeatPanel _defeatPanel;
        [SerializeField] private WinPanel _winPanel;

        [Space(5), Header("Rewards from Enemy Settings")]
        [SerializeField] private GameObject _coinPrefab;
        [SerializeField] private int _coinsCount;
        [SerializeField] private float _spawnRadius = 30f;
        [SerializeField] private float _animationDuration = 1f;

        [Space(5), Header("Camera Setup")]
        [SerializeField] private Camera _mainCamera;

        [Space(5), Header("Controllers")]
        [SerializeField] private UnitController _unitController;

        private readonly Queue<GameObject> _coinsPool = new();

        public event Action OnBuyHeroButtonClicked;

        private void Start()
        {
            InitializeCoinPool();

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

        private void InitializeCoinPool()
        {
            for (int i = 0; i < _coinsCount; i++)
            {
                GameObject coin = Instantiate(_coinPrefab, _canvas.transform);
                coin.SetActive(false);
                _coinsPool.Enqueue(coin);
            }
        }

        public void PlayerWin()
        {
            _winPanel.Open();
            StopGame();
        }

        public void PlayerDefeat()
        {
            _defeatPanel.Open();
            StopGame();
        }

        private void StopGame()
        {
            if (_buyHeroButton != null)
                _buyHeroButton.interactable = false;

            if (_unitController != null)
                _unitController.enabled = false;

            if (_uiPanel != null)
                _uiPanel.SetActive(false);
        }

        public void Open() => _uiPanel.SetActive(true);
        public void Close() => _uiPanel.SetActive(false);

        public void UpdateHireButtonState(int currentPrice, int currentCoins)
        {
            if(currentCoins < currentPrice)
            {
                _buyHeroButton.interactable = false;
                _buyHeroButtonAnimation.StopAnimations();
                return;
            }

            _buyHeroButton.interactable = true;
            _buyHeroButtonAnimation.StartAnimations();
        }

        public void HandleEnemyDied(Transform enemyPosition, Action onComplete)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasTransform,
                _mainCamera.WorldToScreenPoint(enemyPosition.position),
                null,
                out Vector2 spawnPos);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasTransform,
                RectTransformUtility.WorldToScreenPoint(
                    null,
                    _coinsTargetTransform.position),
                    null,
                    out Vector2 targetUIPos);

            int completedCoins = 0;
            int spawnedCoins = 0;

            for (int i = 0; i < _coinsCount; i++)
            {
                if (_coinsPool.Count == 0)
                    break;

                GameObject coin = _coinsPool.Dequeue();
                RectTransform coinRect = coin.GetComponent<RectTransform>();

                coin.SetActive(true);

                spawnedCoins++;

                coinRect.anchoredPosition =
                    spawnPos + Random.insideUnitCircle * _spawnRadius;

                coinRect
                    .DOAnchorPos(targetUIPos, _animationDuration)
                    .SetEase(Ease.InQuad)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        coin.SetActive(false);
                        _coinsPool.Enqueue(coin);

                        completedCoins++;

                        if (completedCoins == spawnedCoins)
                        {
                            onComplete?.Invoke();
                        }
                    });
            }
        }

        public void HandleCurrentPriceChanged(int price) => _heroPriceText.text = price.ToString();

        public void HandlePlayerCoinsChanged(int coins) => _playerCoinsText.text = coins.ToString();

        private void HandleBuyHeroButtonClick() => OnBuyHeroButtonClicked?.Invoke();
    }
}