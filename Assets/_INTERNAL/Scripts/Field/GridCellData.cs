using Entity;
using UnityEngine;

namespace Field
{
    public class GridCellData : MonoBehaviour
    {
        [SerializeField] private Vector2Int _coordinates;
        [SerializeField] private bool _isOccupied;
        [SerializeField] private Unit _unit;

        public Vector2Int Coordinates => _coordinates;
        public bool IsOccupied => _isOccupied;
        public Unit Unit => _unit;

        public void SetCoordinates(Vector2Int coordinates) => _coordinates = coordinates;
        public void SetOccupiedFlag(bool flag) => _isOccupied = flag;
        public void SetUnit(Unit unit) => _unit = unit;
    }
}