using Animations.Tutorial;
using Entity;
using Field;
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
        [SerializeField] private HireTutorialController _hireTutorialController;
        [SerializeField] private UnitController _unitController;
        [SerializeField] private UIController _uiController;

        private PlayerWallet _playerWallet;

        private void Awake()
        {
            if (_gridBuilder == null && _gridController == null)
                throw new MissingReferenceException("Grid controller or grid builder is null!");

            _playerWallet = new(0);

            _playerWallet.OnCoinsChanged += HandlePlayerCoinsChanged;
            _gridBuilder.OnGridInitialized += HandleInitializedGrid;
            _hireTutorialController.OnBuyButtonClicked += HandleHireButtonClicked;
            _unitController.OnCurrentPriceChanged += HandleChangedHeroPrice;
            _enemyController.OnEnemySpawned += HandleSpawnedEnemy;
            _enemyController.OnEnemyDied += HandleEnemyDeath;
        }

        

        private void Start() => _playerWallet.RequestCurrentCoinsCount();

        private void OnDestroy()
        {
            _gridBuilder.OnGridInitialized -= HandleInitializedGrid;
            _hireTutorialController.OnBuyButtonClicked -= HandleHireButtonClicked;
            _unitController.OnCurrentPriceChanged -= HandleChangedHeroPrice;
            _playerWallet.OnCoinsChanged -= HandlePlayerCoinsChanged;
            _enemyController.OnEnemySpawned -= HandleSpawnedEnemy;
            _enemyController.OnEnemyDied -= HandleEnemyDeath;
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
            _playerWallet.AddCoins(enemy.Reward);
        }

        private void HandlePlayerCoinsChanged(int coins)
        {
            _uiController.HandlePlayerCoinsChanged(coins);
        }

        private void HandleChangedHeroPrice(int price)
        {
            _uiController.HandleCurrentPriceChanged(price);
        }

        private void HandleHireButtonClicked()
        {
            _gridController.Spawn();
        }
    }
}