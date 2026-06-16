using UnityEngine;

namespace Entity
{
    public abstract class Enemy : MonoBehaviour
    {
        [SerializeField] protected GameObject _view;

        protected int _health;
        protected Vector3 _position;
        protected float _velocity;
        protected float _rotationSpeed = 5f;

        public int Health => _health;
        public Vector3 Position => _position;
        public float Velocity => _velocity;
        public float RotationSpeed => _rotationSpeed;

        public Enemy(int health, float velocity)
        {
            _health = health;
            _velocity = velocity;
        }
    }
}