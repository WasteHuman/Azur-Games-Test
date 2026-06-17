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

        private PlayerWallet _playerWallet;

        private void Awake()
        {
            if (_gridBuilder == null && _gridController == null)
                throw new MissingReferenceException("Grid controller or grid builder is null!");

            _gridBuilder.OnGridInitialized += HandleInitializedGrid;
        }

        private void Start()
        {
            _playerWallet = new(0);
        }

        private void OnDestroy()
        {
            _gridBuilder.OnGridInitialized -= HandleInitializedGrid;
        }

        private void HandleInitializedGrid(CustomGridCell[,] grid)
        {
            _gridController.InjectGrid(grid);
            _enemyController.StartEnemySpawn();
        }
    }
}