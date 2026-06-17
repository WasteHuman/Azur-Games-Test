using UnityEngine;

namespace Entity
{
    public class ElfArcher : Unit
    {
        public ElfArcher(int damage, int level, float rotationSpeed, int cost, float attackDelay)
            : base(damage, level, rotationSpeed, cost, attackDelay) { }

        public void IncreaseCost() => _cost += 5;
    }
}