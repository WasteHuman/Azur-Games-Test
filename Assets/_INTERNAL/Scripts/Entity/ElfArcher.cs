using UnityEngine;

namespace Entity
{
    public class ElfArcher : Unit
    {
        public ElfArcher(int damage, int level, float rotationSpeed, int cost) : base(damage, level, rotationSpeed, cost) { }

        public void IncreaseCost() => _cost += 5;
    }
}