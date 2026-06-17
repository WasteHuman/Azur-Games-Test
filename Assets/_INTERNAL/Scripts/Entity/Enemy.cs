using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entity
{
    public abstract class Enemy : MonoBehaviour
    {
        [SerializeField] protected GameObject _view;
        [SerializeField] protected int _health;
        [SerializeField] protected float _velocity;

        protected Vector3 _position;
        protected float _rotationSpeed = 5f;

        private bool _isTarget = false;
        private bool _isPathEnded = false;
        private List<Transform> _path;
        private int _currentWaypointIndex = 0;

        public int Health
        {
            get => _health;
            private set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Damage cannot be a negative");

                _health = value;
                OnHealthChange?.Invoke(_health);
            }
        }
        public Vector3 Position => _position;
        public float Velocity => _velocity;
        public float RotationSpeed => _rotationSpeed;
        public bool IsAlive => _health > 0;

        public event Action<int> OnHealthChange;
        public event Action OnPathEnded;
        public event Action OnEnemyDied;

        public Enemy(int health, float velocity)
        {
            _health = health;
            _velocity = velocity;
        }

        public void RequestCurrentHealth() => OnHealthChange?.Invoke(Health);

        public void InitializePath(List<Transform> waypoints)
        {
            if (waypoints == null)
                return;

            _path = waypoints;

            _currentWaypointIndex = 1;
        }

        public void MarkAsTarget() => _isTarget = true;

        public virtual void ApplyDamage(int damage)
        {
            Health -= damage;

            if (Health <= 0)
                OnEnemyDied?.Invoke();
        }

        protected virtual void Update()
        {
            if (_path == null || _path.Count == 0 || _isPathEnded) 
                return;

            if (_currentWaypointIndex >= _path.Count)
            {
                OnPathEnd();
                return;
            }

            MoveAlongPath();
        }

        private void MoveAlongPath()
        {
            Transform targetWaypoint = _path[_currentWaypointIndex];

            transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetWaypoint.position,
                    _velocity * Time.deltaTime);

            transform.rotation = Quaternion.LookRotation(targetWaypoint.position - transform.position);

            if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.01f)
            {
                _currentWaypointIndex++;

                if (_currentWaypointIndex >= _path.Count)
                {
                    OnPathEnd();
                }
            }
        }

        protected virtual void OnPathEnd()
        {
            _isPathEnded = true;
            OnPathEnded?.Invoke();
            Debug.Log($"{gameObject.name} reached the end of the path!");
        }
    }
}