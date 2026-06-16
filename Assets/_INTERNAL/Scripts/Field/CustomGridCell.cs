using Entity;
using UnityEngine;

namespace Field
{
    [System.Serializable]
    public class CustomGridCell
    {
        public Vector2Int Coordinates;
        public Vector3 WorldPosition;
        public bool IsOccupied;
        public Unit Unit;

        public GridCellData CellData;

        public CustomGridCell(Vector2Int coordinates, Vector3 worldPosition, GridCellData gridCellData)
        {
            Coordinates = coordinates;
            WorldPosition = worldPosition;
            IsOccupied = false;
            Unit = null;
            CellData = gridCellData;
        }
    }
}