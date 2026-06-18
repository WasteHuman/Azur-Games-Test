using Entity;
using Field;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Playable
{
    public class MergeController : MonoBehaviour
    {
        // TODO: Контроллер мерджа юнитов
        [Header("References")]
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private UnitController _unitController;
        [SerializeField] private Unit _meridaArcherPrefab;

        [Space(5), Header("Raycast")]
        [SerializeField] private LayerMask _unitLayerMask;
        [SerializeField] private LayerMask _cellLayerMask;
        [SerializeField] private float _raycastMaxDistance = 100f;

        private GridCellData _originalCellData;
        private Vector3 _originalScale;
        private Unit _draggedUnit;
        private Transform _originalParent;
        private Vector3 _originalPosition;
        private float _dragPlaneY;

        public event Action OnMergeCompleted;

        private void Awake()
        {
            if(_mainCamera == null)
                _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (_draggedUnit == null)
                return;

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = _mainCamera.ScreenPointToRay(mousePosition);
            Plane plane = new(Vector3.up, new Vector3(0f, _dragPlaneY, 0f));
            if(plane.Raycast(ray, out float enter))
            {
                Vector3 worldPoint = ray.GetPoint(enter);
                _draggedUnit.transform.position = worldPoint;
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame)
                EndDrag();
        }

        public void StartDrag(Unit unit)
        {
            if (unit == null)
                return;

            _originalScale = unit.transform.localScale;
            _originalCellData = unit.GetComponentInParent<GridCellData>();

            _draggedUnit = unit;

            _draggedUnit.transform.localScale = _originalScale * 1.1f;

            _originalParent = unit.transform.parent;
            _originalPosition = unit.transform.position;
            _dragPlaneY = unit.transform.position.y;

            unit.transform.SetParent(null);
        }

        public void EndDrag()
        {
            if (_draggedUnit == null)
                return;

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = _mainCamera.ScreenPointToRay(mousePosition);

            RaycastHit[] cellHits = Physics.RaycastAll(ray, _raycastMaxDistance, _cellLayerMask);
            var orderedCells = cellHits.OrderBy(h => h.distance);
            GridCellData targetCell = null;
            foreach (var h in orderedCells)
            {
                var cell = h.collider.GetComponentInParent<GridCellData>();
                if (cell != null)
                {
                    targetCell = cell;
                    break;
                }
            }

            if (targetCell != null)
            {
                if (targetCell.IsOccupied)
                {
                    Unit targetUnit = targetCell.Unit ?? targetCell.GetComponentInChildren<Unit>();
                    if (targetUnit != null && targetUnit != _draggedUnit)
                    {
                        if (CanMerge(_draggedUnit, targetUnit))
                        {
                            PerformUnit(_draggedUnit, targetUnit);
                            _draggedUnit = null;
                            _originalParent = null;
                            return;
                        }
                        else
                        {
                            SwapUnits(_draggedUnit, targetUnit);
                            _draggedUnit = null;
                            _originalParent = null;
                            return;
                        }
                    }
                }
                else
                {
                    MoveUnitToCell(_draggedUnit, targetCell);
                    _draggedUnit = null;
                    _originalParent = null;
                    return;
                }
            }

            RaycastHit[] hits = Physics.RaycastAll(ray, _raycastMaxDistance, _unitLayerMask);
            Unit targetFromRay = null;

            var ordered = hits.OrderBy(h => h.distance);
            foreach (var h in ordered)
            {
                var u = h.collider.GetComponentInParent<Unit>();
                if (u != null && u != _draggedUnit)
                {
                    targetFromRay = u;
                    break;
                }
            }

            if (targetFromRay != null && CanMerge(_draggedUnit, targetFromRay))
                PerformUnit(_draggedUnit, targetFromRay);
            else
            {
                _draggedUnit.transform.SetParent(_originalParent);
                _draggedUnit.transform.position = _originalPosition;
                _draggedUnit.transform.localScale = _originalScale;
            }

            _draggedUnit = null;
            _originalParent = null;
        }

        private bool CanMerge(Unit a, Unit b)
        {
            if (a == null || b == null)
                return false;

            return a.GetType() == b.GetType() && a.Level == b.Level;
        }

        private void PerformUnit(Unit dragged, Unit target)
        {
            if (dragged == null || target == null)
                return;

            _unitController.RemoveUnit(dragged);
            _unitController.RemoveUnit(target);

            Transform targetParent = target.transform.parent;
            Vector3 targetPos = target.transform.position;
            Quaternion targetRot = target.transform.rotation;
            int targetPrice = target.Price;

            GridCellData targetCellData = target.GetComponentInParent<GridCellData>();

            _originalCellData.SetOccupiedFlag(false);
            _originalCellData.SetUnit(null);

            Destroy(dragged.gameObject);
            Destroy(target.gameObject);

            Unit meridaUnit = Instantiate(_meridaArcherPrefab, targetPos, targetRot, targetParent);
            meridaUnit.SetPrice(targetPrice);
            meridaUnit.IncreasePrice();
            targetCellData.SetUnit(meridaUnit);

            _unitController.AddNewUnit(meridaUnit, true);
            OnMergeCompleted?.Invoke();
        }

        private void MoveUnitToCell(Unit unit, GridCellData targetCell)
        {
            if (unit == null || targetCell == null)
                return;

            _originalCellData.SetOccupiedFlag(false);
            _originalCellData.SetUnit(null);

            unit.transform.SetParent(targetCell.transform);
            unit.transform.position = targetCell.transform.position;
            unit.transform.localScale = _originalScale;

            targetCell.SetUnit(unit);
            targetCell.SetOccupiedFlag(true);
        }

        private void SwapUnits(Unit a, Unit b)
        {
            if (a == null || b == null)
                return;

            GridCellData aCell = a.GetComponentInParent<GridCellData>();
            GridCellData bCell = b.GetComponentInParent<GridCellData>();

            Transform aParent = a.transform.parent;
            Transform bParent = b.transform.parent;
            Vector3 aPos = a.transform.position;
            Vector3 bPos = b.transform.position;

            a.transform.SetParent(bParent);
            a.transform.position = bPos;
            a.transform.localScale = _originalScale;

            b.transform.SetParent(aParent);
            b.transform.position = aPos;
            b.transform.localScale = _originalScale;

            aCell.SetUnit(b);
            bCell.SetUnit(a);

            aCell.SetOccupiedFlag(true);
            bCell.SetOccupiedFlag(true);
        }
    }
}