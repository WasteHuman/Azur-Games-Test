using Entity;
using Field;
using UnityEngine;

namespace Playable
{
    public class GridController : MonoBehaviour
    {
        // TODO: Оркестр юнитов внутри сетки
        [Header("Unit Settings")]
        [SerializeField] private Unit _elfPrefab;
        [SerializeField] private Vector2Int _firstElfStartCoordinates = new(2, 2);

        [Space(5), Header("Other Controllers")]
        [SerializeField] private UnitController _unitController;

        private CustomGridCell[,] _grid;

        public void InjectGrid(CustomGridCell[,] grid)
        {
            _grid = grid ?? throw new MissingReferenceException("Grid is missing!");
            SpawnFirstArcher();
        }

        private void SpawnFirstArcher()
        {
            var cell = _grid[_firstElfStartCoordinates.x, _firstElfStartCoordinates.y];
            var elfUnit = Instantiate(_elfPrefab, cell.CellData.transform, worldPositionStays: true);
            elfUnit.transform.position = cell.CellData.transform.position;

            _unitController.AddNewUnit(elfUnit, true);

            cell.SetUnit(elfUnit);
            cell.SetOccupiedFlag(true);
        }

        public void Spawn()
        {
            CustomGridCell freeCell = null;
            int maxX = _grid.GetLength(0);
            int maxY = _grid.GetLength(1);
            int maxAttempts = maxX * maxY;

            int randomX;
            int randomY;
            bool foundFreeCell = false;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                randomX = Random.Range(0, maxX);
                randomY = Random.Range(0, maxY);
                var cell = _grid[randomX, randomY];

                if (!cell.IsOccupied)
                {
                    foundFreeCell = true;
                    freeCell = cell;
                    break;
                }
            }

            if (foundFreeCell)
            {
                var elfUnit = Instantiate(_elfPrefab, freeCell.CellData.transform, worldPositionStays: true);
                elfUnit.transform.position = freeCell.CellData.transform.position;

                _unitController.AddNewUnit(elfUnit);

                freeCell.SetUnit(elfUnit);
                freeCell.SetOccupiedFlag(true);
            }
            else
                Debug.LogError("Couldn't find an empty cell! The field is fully occupied.");
        }
    }
}