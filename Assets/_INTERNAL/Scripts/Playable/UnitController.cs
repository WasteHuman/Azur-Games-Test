using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Playable
{
    public class UnitController : MonoBehaviour
    {
        // TODO: Оркестр всех Юнитов, ищет доступных для атаки врагов, помечает их как цель конкретного Юнита и передаёт наведение Юниту
        [SerializeField] private List<Unit> _availableUnits = new();
        [SerializeField] private MergeController _mergeController;
        [SerializeField] private PlayableController _playableController;

        private bool _isPlaying = true;

        private int _currentUnitPrice;

        private readonly List<Enemy> _activeEnemies = new();
        private readonly Dictionary<Unit, Enemy> _unitTargets = new();

        public int CurrentUnitPrice
        {
            get => _currentUnitPrice;
            private set
            {
                _currentUnitPrice = value;
                OnCurrentPriceChanged?.Invoke(_currentUnitPrice);
            }
        }

        public event Action<Unit, Unit> OnMergeTutorStart;
        public event Action<int> OnCurrentPriceChanged;
        public event Action OnAllTargetsDestroyed;

        private void Update()
        {
            if(_isPlaying)
                AssignTargetsToUnits();
        }

        public void AddNewUnit(Unit newUnit, bool isFirstArcher = false, bool isMerge = false)
        {
            newUnit.OnDragStarted += HandleUnitDragStarted;
            newUnit.OnDragCompleted += HandleUnitDragCompleted;

            _availableUnits.Add(newUnit);

            if (!isFirstArcher && !isMerge)
            {
                newUnit.SetPrice(CurrentUnitPrice);
                newUnit.IncreasePrice();
            }

            if(_availableUnits.Count == 2)
            {
                Unit firstUnit = _availableUnits.First();
                OnMergeTutorStart?.Invoke(firstUnit, newUnit);
            }

            CurrentUnitPrice = newUnit.Price;
        }

        public void PlayerWin()
        {
            ClearTargets();

            for (int i = 0; i < _availableUnits.Count; i++)
                _availableUnits[i].PlayerWin();
        }

        public void PlayerDefeat()
        {
            _isPlaying = false;
            ClearTargets();

            for (int i = 0; i < _availableUnits.Count; i++)
                _availableUnits[i].PlayerDefeat();
        }

        public void RemoveUnit(Unit unit)
        {
            if (unit == null)
                return;

            unit.OnDragStarted -= HandleUnitDragStarted;
            unit.OnDragCompleted -= HandleUnitDragCompleted;

            if (_availableUnits.Contains(unit))
                _availableUnits.Remove(unit);

            var targetsForRemoved = _unitTargets.Where(kvp => kvp.Key == unit).ToList();
            foreach (var kpv in targetsForRemoved)
                _unitTargets.Remove(kpv.Key);
        }

        public void AddNewActiveEnemies(Enemy enemy) => _activeEnemies.Add(enemy);

        public void OnEnemyDied(Enemy enemy)
        {
            _activeEnemies.Remove(enemy);

            var unitsTargetingDeadEnemy = _unitTargets.Where(kvp => kvp.Value == enemy).ToList();
            foreach (var kvp in unitsTargetingDeadEnemy)
                _unitTargets.Remove(kvp.Key);
        }

        private void AssignTargetsToUnits()
        {
            if (_activeEnemies == null || _activeEnemies.Count == 0)
                return;

            var aliveEnemies = _activeEnemies.Where(e => e.IsAlive).ToList();

            if (aliveEnemies.Count == 0)
                return;

            Enemy boss = aliveEnemies.FirstOrDefault(e => e.IsBossMob);
            var normalEnemies = aliveEnemies.Where(e => !e.IsBossMob).ToList();

            if (normalEnemies.Count > 0)
                DistributeUnitsToEnemies(normalEnemies);
            else if (boss != null && boss.IsAlive)
                AssignAllUnitsToTarget(boss);
        }

        private void AssignAllUnitsToTarget(Enemy target)
        {
            foreach (var unit in _availableUnits)
            {
                if (_unitTargets.ContainsKey(unit) && _unitTargets[unit] == target)
                    continue;

                _unitTargets[unit] = target;
                unit.StartAttack(target);
            }
        }

        private void DistributeUnitsToEnemies(List<Enemy> aliveEnemies)
        {
            if (aliveEnemies.Count == 0)
            {
                OnAllTargetsDestroyed?.Invoke();
                return;
            }

            var deadTargets = _unitTargets.Where(kvp => kvp.Value == null || !kvp.Value.IsAlive).ToList();
            foreach (var kvp in deadTargets)
                _unitTargets.Remove(kvp.Key);

            var enemyUnitCount = new Dictionary<Enemy, int>();
            foreach (var enemy in aliveEnemies)
                enemyUnitCount[enemy] = _unitTargets.Values.Count(e => e == enemy);

            var unitsWithoutTarget = _availableUnits
                .Where(u =>!_unitTargets.ContainsKey(u) || _unitTargets[u] == null || !_unitTargets[u].IsAlive)
                .ToList();

            foreach (var unit in unitsWithoutTarget)
            {
                var targetEnemy = aliveEnemies.OrderBy(e => enemyUnitCount[e]).FirstOrDefault();

                if (targetEnemy == null)
                    return;

                _unitTargets[unit] = targetEnemy;
                unit.StartAttack(targetEnemy);
                targetEnemy.MarkAsTarget();

                enemyUnitCount[targetEnemy]++;
                aliveEnemies.Remove(targetEnemy);
            }
        }

        private void HandleUnitDragCompleted() => _mergeController.EndDrag();

        private void HandleUnitDragStarted(Unit unit) => _mergeController.StartDrag(unit);

        public void ClearTargets() => _unitTargets.Clear();
    }
}