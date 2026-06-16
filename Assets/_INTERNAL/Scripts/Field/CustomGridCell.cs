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

        public CustomGridCell(Vector2Int coordinates, Vector3 worldPosition)
        {
            Coordinates = coordinates;
            WorldPosition = worldPosition;
            IsOccupied = false;
            Unit = null;
        }
    }
}