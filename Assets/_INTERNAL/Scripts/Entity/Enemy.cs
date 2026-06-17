using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity
{
    public abstract class Enemy : MonoBehaviour
    {
        [SerializeField] protected GameObject _view;
        [SerializeField] protected int _health;
        [SerializeField] protected float _velocity;
        [SerializeField] protected bool _isBossMob = false;
        [SerializeField] protected Animation _animation;
        [SerializeField] protected int _reward;

        protected Vector3 _position;
        protected float _rotationSpeed = 5f;

        private bool _isDie = false;
        [SerializeField] private bool _isTarget = false;
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
        public int Reward => _reward;
        public Vector3 Position => _position;
        public float Velocity => _velocity;
        public float RotationSpeed => _rotationSpeed;
        public bool IsAlive => _health > 0;
        public bool IsBossMob => _isBossMob;

        public event Action<int> OnHealthChange;
        public event Action OnPathEnded;
        public event Action<Enemy> OnEnemyDied;

        private void Start()
        {
            _animation["Attack"].wrapMode = WrapMode.Once;
            _animation["Death"].wrapMode = WrapMode.Once;
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
            Debug.Log($"Enemy health: {Health}");

            if (Health <= 0)
            {
                _isDie = true;
                StartCoroutine(WaitForAnimationEndStrict("Death", OnEnemyDied));
            }
        }

        protected virtual void Update()
        {
            if (_path == null || _path.Count == 0 || _isPathEnded || _isDie) 
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
                    OnPathEnd();
            }
        }

        private IEnumerator WaitForAnimationEndStrict(string clip, Action<Enemy> onComplete)
        {
            _animation[clip].wrapMode = WrapMode.Once;
            _animation.Play(clip);

            AnimationState state = _animation[clip];
            yield return new WaitUntil(() => state.time >= state.length);

            onComplete?.Invoke(this);
            StopCoroutine(WaitForAnimationEndStrict(clip, onComplete));
        }

        protected virtual void OnPathEnd()
        {
            _isPathEnded = true;
            _animation.Play("Attack");
            OnPathEnded?.Invoke();
            Debug.Log($"{gameObject.name} reached the end of the path!");
        }
    }
}