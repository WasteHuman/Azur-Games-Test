using UnityEngine;

namespace Entity
{
    public abstract class Unit : MonoBehaviour
    {
        [SerializeField] protected GameObject _view;

        protected int _damage;
        protected int _level;
        protected int _cost;
        protected float _rotationSpeed;

        public int Damage => _damage;
        public int Level => _level;
        public int Cost => _cost;
        public float RotationSpeed => _rotationSpeed;

        public Unit(int damage, int level, float rotationSpeed, int cost)
        {
            _damage = damage;
            _level = level;
            _rotationSpeed = rotationSpeed;
            _cost = cost;
        }
    }
}