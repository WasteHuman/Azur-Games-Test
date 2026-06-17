using Entity;
using System.Collections.Generic;
using UnityEngine;

namespace Playable
{
    public class UnitController : MonoBehaviour
    {
        // TODO: Оркестр всех Юнитов, ищет доступных для атаки врагов, помечает их как цель конкретного Юнита и передаёт наведение Юниту
        [SerializeField] private List<Unit> _availableUnits = new();

        public void AddNewUnit(Unit newUnit)
        {
            _availableUnits.Add(newUnit);
        }
    }
}