using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Entity
{
    public abstract class Unit : MonoBehaviour
    {
        [SerializeField] protected GameObject _view;
        [SerializeField] protected int _damage;
        [SerializeField] protected int _level;
        [SerializeField] protected int _price;
        [SerializeField] protected float _attackDelay;
        [SerializeField] protected float _rotationSpeed;
        [SerializeField] protected Animation _animation;

        [SerializeField] private Enemy _target;
        private Coroutine _attackRoutine;

        public int Damage => _damage;
        public int Level => _level;
        public int Price
        {
            get => _price;
            private set
            {
                _price = value;
                OnPriceChanged?.Invoke();
            }
        }
        public float RotationSpeed => _rotationSpeed;
        public float AttackDelay => _attackDelay;

        public event Action OnPriceChanged;

        private void Start()
        {
            _animation["Attack"].wrapMode = WrapMode.Once;
            _animation["Death"].wrapMode = WrapMode.Once;
        }

        public virtual void StartAttack(Enemy target)
        {
            _target = target;

            if(_attackRoutine != null)
                StopCoroutine(_attackRoutine);

            _attackRoutine = StartCoroutine(AttackWithDelay(target));
        }

        private void Update()
        {
            if (_target == null)
                return;

            RotateTowards(_target.transform.position);
        }

        public void IncreasePrice() => Price += 5;

        public void PlayerDefeat() => _animation.Play("Death");

        private void RotateTowards(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;

            if (direction == Vector3.zero)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                _rotationSpeed * Time.deltaTime
            );
        }

        private IEnumerator AttackWithDelay(Enemy target)
        {
            while(target.IsAlive && target != null)
            {
                yield return new WaitForSeconds(_attackDelay);
                _animation.Play("Attack");
                target.ApplyDamage(_damage);
            }
        }
    }
}