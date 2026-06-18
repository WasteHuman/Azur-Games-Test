using Playable.UI;
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
        [SerializeField] private HpAnchor _hpAnchor;

        protected Vector3 _position;
        protected float _rotationSpeed = 5f;

        private int _maxHealth;
        private HealthBarView _healthBarView;

        private bool _isDie = false;
        private bool _isTarget = false;
        private bool _isPathEnded = false;

        private List<Transform> _path;
        private int _currentWaypointIndex = 0;

        public int Health
        {
            get => _health;
            private set
            {
                _health = value;
            }
        }
        public int Reward => _reward;
        public bool IsAlive => _health > 0;
        public bool IsBossMob => _isBossMob;
        public Transform HpAnchorPoint => _hpAnchor.Point;

        public event Action OnPathEnded;
        public event Action<Enemy> OnEnemyDied;

        private void Start()
        {
            _maxHealth = _health;
            _animation["Attack"].wrapMode = WrapMode.Once;
        }

        public void SetHealthBar(HealthBarView healthBarView) => _healthBarView = healthBarView;

        public void InitializePath(List<Transform> waypoints)
        {
            if (waypoints == null)
                return;

            _path = waypoints;

            _currentWaypointIndex = 1;
        }

        public void MarkAsTarget() => _isTarget = true;

        public void Stop() => _isPathEnded = true;

        public virtual void ApplyDamage(int damage)
        {
            Health -= damage;
            _healthBarView.SetHealth(Mathf.Clamp01((float)Health / (float)_maxHealth));

            if (Health <= 0)
            {
                _isDie = true;
                Destroy(_healthBarView.gameObject);
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

            yield return new WaitUntil(() => !_animation.IsPlaying(clip));

            onComplete?.Invoke(this);
        }

        protected virtual void OnPathEnd()
        {
            _isPathEnded = true;
            _animation.Play("Attack");
            OnPathEnded?.Invoke();
        }
    }
}