using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Field
{
    [ExecuteInEditMode]
    public class GridBuilder : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private int _columns = 5;
        [SerializeField] private int _rows = 4;
        [SerializeField] private GameObject _cellPrefab;
        [SerializeField] private Vector3 _startPosition;
        [SerializeField] private Vector3 _cellOffset = Vector3.one;

        [Space(5), Header("Visual Settings")]
        [SerializeField] private Sprite _idleSprite;
        [SerializeField] private Sprite _occupiedSprite;

        [Space(5), Header("Generated Grid")]
        [SerializeField] private CustomGridCell[,] _cells;
        [SerializeField] private Transform _cellsParent;

        private bool _isRebuilding = false;
        private bool _isGridInitialized = false;

        public event Action<CustomGridCell[,]> OnGridInitialized;

        public int Columns => _columns;
        public int Rows => _rows;
        public CustomGridCell[,] Cells
        {
            get => _cells;
            private set
            {
                _cells = value;
            }
        }
        public Transform CellsParent => _cellsParent;

        private void Start()
        {
            InitializeGridFromScene();
        }

        private void InitializeGridFromScene()
        {
            LoadGridFromScene();
            _isGridInitialized = true;

            OnGridInitialized?.Invoke(Cells);
            Debug.Log($"Grid initialized: {Cells != null}");
        }

        private void LoadGridFromScene()
        {
            if (_cellsParent == null)
            {
                Debug.Log($"Grid parent is null: {_cellsParent == null}");
                return;
            }

            Cells = new CustomGridCell[_columns, _rows];

            GridCellData[] allCells = _cellsParent.GetComponentsInChildren<GridCellData>();

            foreach (GridCellData cellData in allCells)
            {
                Vector2Int coords = cellData.Coordinates;

                if (coords.x >= 0 && coords.x < _columns &&
                    coords.y >= 0 && coords.y < _rows)
                {
                    Cells[coords.x, coords.y] = new CustomGridCell(
                        coords,
                        cellData.transform.position,
                        cellData);
                }
            }

            _isGridInitialized = true;
        }

#if UNITY_EDITOR
        private int _lastColumns = -1;
        private int _lastRows = -1;
        private GameObject _lastCellPrefab = null;

        private void OnValidate()
        {
            if (!_isRebuilding)
                return;

            if (Application.isPlaying)
                return;

            if (_lastColumns != _columns || _lastRows != _rows || _lastCellPrefab != _cellPrefab)
            {
                _lastColumns = _columns;
                _lastRows = _rows;
                _lastCellPrefab = _cellPrefab;

                if (IsGridValid())
                {
                    ClearGrid();
                    BuildGrid();
                    EditorUtility.SetDirty(this);
                }
            }
        }

        private void OnDestroy()
        {
            if (!Application.isPlaying && _cellsParent != null)
                DestroyImmediate(_cellsParent.gameObject);
        }

        private bool IsGridValid() => _cellPrefab != null && _columns > 0 && _rows > 0;

        private void ClearGrid()
        {
            if (_cellsParent != null)
                DestroyImmediate(_cellsParent.gameObject);

            Cells = null;
        }

        private void BuildGrid()
        {
            Cells = new CustomGridCell[_columns, _rows];

            GameObject parentObj = new("GridCells");
            parentObj.transform.SetPositionAndRotation(_startPosition, Quaternion.identity);
            _cellsParent = parentObj.transform;

            EditorUtility.SetDirty(parentObj);

            for (int x = 0; x < _columns; x++)
            {
                for (int y = 0; y < _rows; y++)
                    CreateCell(x, y);
            }

            _isRebuilding = false;
            Debug.Log($"Grid built: {Cells != null}");
        }

        private void CreateCell(int x, int y)
        {
            Vector3 position = _startPosition + new Vector3(
                x: x * _cellOffset.x,
                y: 0f,
                z: y * _cellOffset.z);

            Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);
            GameObject cellObj = Instantiate(_cellPrefab, position, rotation, _cellsParent);
            cellObj.name = $"Cell_{x}_{y}";

            if (!cellObj.TryGetComponent<GridCellData>(out var cellData))
                cellData = cellObj.AddComponent<GridCellData>();

            Vector2Int coordinates = new(x, y);
            cellData.SetCoordinates(coordinates);
            cellData.SetOccupiedFlag(false);

            CustomGridCell cell = new(coordinates, position, cellData);
            Cells[x, y] = cell;

            UpdateCellVisual(cellObj, false);

            EditorUtility.SetDirty(cellObj);
        }

        private void UpdateCellVisual(GameObject cellObj, bool isOccupied)
        {
            if (cellObj.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            {
                Sprite newSprite = isOccupied ? _occupiedSprite : _idleSprite;
                spriteRenderer.sprite = newSprite;
            }
        }

        [ContextMenu("Rebuild Grid")]
        private void RebuildGrid()
        {
            _isRebuilding = true;
            ClearGrid();
            BuildGrid();
            EditorUtility.SetDirty(this);
        }

        [ContextMenu("Clear Grid")]
        private void ClearGridMenu()
        {
            ClearGrid();
            EditorUtility.SetDirty(this);
        }
#endif

        public CustomGridCell GetCell(Vector2Int coordinates)
        {
            if (coordinates.x < 0 || coordinates.x >= _columns ||
                coordinates.y < 0 || coordinates.y >= _rows)
                return null;

            return Cells[coordinates.x, coordinates.y];
        }

        public CustomGridCell GetCell(int x, int y) => GetCell(new Vector2Int(x, y));

        public bool TryGetCell(int x, int y, out CustomGridCell cell)
        {
            cell = GetCell(x, y);
            return cell != null;
        }

        public Vector3 GetCellWorldPosition(Vector2Int coordinates)
        {
            var cell = GetCell(coordinates);
            return cell?.WorldPosition ?? Vector3.zero;
        }

        public Vector2Int WorldToGridPosition(Vector3 worldPosition)
        {
            Vector3 localPosition = worldPosition - _startPosition;

            int x = Mathf.RoundToInt(localPosition.x / _cellOffset.x);
            int y = Mathf.RoundToInt(localPosition.z / _cellOffset.z);

            return new Vector2Int(
                Mathf.Clamp(x, 0, _columns - 1),
                Mathf.Clamp(y, 0, _rows - 1));
        }

        public bool IsPositionInGrid(Vector3 worldPosition)
        {
            Vector3 localPosition = worldPosition - _startPosition;

            int x = Mathf.RoundToInt(localPosition.x / _cellOffset.x);
            int y = Mathf.RoundToInt(localPosition.z / _cellOffset.z);

            return x >= 0 && x < _columns && y >= 0 && y < _rows;
        }
    }
}