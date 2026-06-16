using Field;
using UnityEngine;

namespace Playable
{
    public class GridController : MonoBehaviour
    {
        private CustomGridCell[,] _grid;

        public void InjectGrid(CustomGridCell[,] grid)
        {
            _grid = grid ?? throw new MissingReferenceException("Grid is missing!");
            Debug.Log($"Grid injected: {_grid.GetType().Name}");
        }
    }
}