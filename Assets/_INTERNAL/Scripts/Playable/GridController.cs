using Entity;
using Field;
using UnityEngine;

namespace Playable
{
    public class GridController : MonoBehaviour
    {
        // TODO: Оркестр юнитов внутри сетки
        [SerializeField] private Unit _elfPrefab;

        private CustomGridCell[,] _grid;

        public void InjectGrid(CustomGridCell[,] grid)
        {
            _grid = grid ?? throw new MissingReferenceException("Grid is missing!");
            Debug.Log($"Grid injected: {_grid.GetType().Name}");
        }
    }
}