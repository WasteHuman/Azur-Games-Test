using Animations.Tutorial;
using Entity;
using Field;
using System;
using UnityEngine;

namespace Playable
{
    public class PlayableController : MonoBehaviour
    {
        // TODO: Оркестр всего плейебла
        [Header("Grid Setup")]
        [SerializeField] private GridBuilder _gridBuilder;
        [SerializeField] private GridController _gridController;

        [Space(5), Header("Other Controllers")]
        [SerializeField] private EnemyController _enemyController;
        [SerializeField] private UnitController _unitController;
        [SerializeField] private UIController _uiController;

        [Space(5), Header("Tutorials Setup")]
        [SerializeField] private HireTutorialController _hireTutorialController;
        [SerializeField] private bool _isHireTutorialCompleted = false;
        [SerializeField] private MergeTutorialController _mergeTutorialController;
        [SerializeField] private bool _isMergeTutorialCompleted = false;

        private PlayerWallet _playerWallet;

        public event Action OnPlayerWon;
        public event Action OnPlayerDefeat;

        private void Awake()
        {
            if (_gridBuilder == null && _gridController == null)
                throw new MissingReferenceException("Grid controller or grid builder is null!");

            _playerWallet = new(0);

            _playerWallet.OnCoinsChanged += HandlePlayerCoinsChanged;

            _gridBuilder.OnGridInitialized += HandleInitializedGrid;

            _hireTutorialController.OnBuyButtonClicked += HandleHireButtonClicked;
            _hireTutorialController.OnTutorialCompleted += HandleCompletedHireTutorial;

            _mergeTutorialController.OnTutorialCompleted += HandleComletedMergeTutorial;

            _unitController.OnCurrentPriceChanged += HandleChangedHeroPrice;
            _unitController.OnMergeTutorStart += HandleMergeTutorialPrepareCompleted;
            _unitController.OnAllTargetsDestroyed += HandlePlayerWin;

            _uiController.OnBuyHeroButtonClicked += HandleHireButtonClicked;

            _enemyController.OnEnemySpawned += HandleSpawnedEnemy;
            _enemyController.OnEnemyDied += HandleEnemyDeath;
            _enemyController.OnPathEnded += HandlePlayerDefeat;
        }

        private void Start() => _playerWallet.RequestCurrentCoinsCount();

        private void OnDestroy()
        {
            _playerWallet.OnCoinsChanged -= HandlePlayerCoinsChanged;

            _gridBuilder.OnGridInitialized -= HandleInitializedGrid;

            _hireTutorialController.OnBuyButtonClicked -= HandleHireButtonClicked;
            _hireTutorialController.OnTutorialCompleted -= HandleCompletedHireTutorial;

            _mergeTutorialController.OnTutorialCompleted -= HandleComletedMergeTutorial;

            _unitController.OnCurrentPriceChanged -= HandleChangedHeroPrice;
            _unitController.OnMergeTutorStart -= HandleMergeTutorialPrepareCompleted;
            _unitController.OnAllTargetsDestroyed -= HandlePlayerWin;

            _uiController.OnBuyHeroButtonClicked -= HandleHireButtonClicked;

            _enemyController.OnEnemySpawned -= HandleSpawnedEnemy;
            _enemyController.OnEnemyDied -= HandleEnemyDeath;
            _enemyController.OnPathEnded -= HandlePlayerDefeat;
        }

        private void HandleInitializedGrid(CustomGridCell[,] grid)
        {
            _gridController.InjectGrid(grid);
            _enemyController.StartEnemySpawn();
        }

        private void HandleSpawnedEnemy(Enemy enemy)
        {
            _unitController.AddNewActiveEnemies(enemy);
        }

        private void HandleEnemyDeath(Enemy enemy)
        {
            _unitController.OnEnemyDied(enemy);
            _uiController.HandleEnemyDied(enemy.transform, () =>
            {
                _playerWallet.AddCoins(enemy.Reward);
                _uiController.UpdateHireButtonState(_unitController.CurrentUnitPrice, _playerWallet.CurrentCoinsCount);

                if (!_isHireTutorialCompleted)
                {
                    _hireTutorialController.Initialize(_unitController.CurrentUnitPrice);
                    _hireTutorialController.Open();
                }
            });
        }

        private void HandlePlayerCoinsChanged(int coins)
        {
            _uiController.HandlePlayerCoinsChanged(coins);
            _uiController.UpdateHireButtonState(_unitController.CurrentUnitPrice, _playerWallet.CurrentCoinsCount);
        }

        private void HandleChangedHeroPrice(int price)
        {
            _uiController.HandleCurrentPriceChanged(price);
            _uiController.UpdateHireButtonState(_unitController.CurrentUnitPrice, _playerWallet.CurrentCoinsCount);
        }

        private void HandleHireButtonClicked()
        {
            _playerWallet.SpendCouns(_unitController.CurrentUnitPrice);
            _gridController.Spawn();
            _hireTutorialController.Close();
        }

        private void HandleMergeTutorialPrepareCompleted(Unit firstUnit, Unit secondUnit)
        {
            if (_isMergeTutorialCompleted)
                return;

            _hireTutorialController.Close();
            _uiController.Close();
            _mergeTutorialController.Open();
            _mergeTutorialController.AnimateMerge(firstUnit.transform, secondUnit.transform);
        }

        private void HandleCompletedHireTutorial()
        {
            _isHireTutorialCompleted = true;
            _uiController.Open();
            _uiController.UpdateHireButtonState(_unitController.CurrentUnitPrice, _playerWallet.CurrentCoinsCount);
        }

        private void HandleComletedMergeTutorial()
        {
            _isMergeTutorialCompleted = true;
            _mergeTutorialController.Close();
            _uiController.Open();
            _uiController.UpdateHireButtonState(_unitController.CurrentUnitPrice, _playerWallet.CurrentCoinsCount);
        }

        private void HandlePlayerDefeat()
        {
            _unitController.PlayerDefeat();
            _uiController.PlayerDefeat();
            _enemyController.StopGame();
        }

        private void HandlePlayerWin()
        {
            _unitController.PlayerWin();
            _uiController.PlayerWin();
            _enemyController.StopGame();
        }
    }
}