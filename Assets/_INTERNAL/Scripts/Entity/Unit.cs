using System.Collections;
using UnityEngine;

namespace Entity
{
    public abstract class Unit : MonoBehaviour
    {
        [SerializeField] protected GameObject _view;
        [SerializeField] protected int _damage;
        [SerializeField] protected int _level;
        [SerializeField] protected int _cost;
        [SerializeField] protected float _attackDelay;
        [SerializeField] protected float _rotationSpeed;

        private Coroutine _attackRoutine;

        public int Damage => _damage;
        public int Level => _level;
        public int Cost => _cost;
        public float RotationSpeed => _rotationSpeed;
        public float AttackDelay => _attackDelay;

        public Unit(int damage, int level, float rotationSpeed, int cost, float attackDelay)
        {
            _damage = damage;
            _level = level;
            _rotationSpeed = rotationSpeed;
            _cost = cost;
            _attackDelay = attackDelay;
        }

        public virtual void StartAttack(Enemy target)
        {
            if(_attackRoutine != null)
                StopCoroutine(_attackRoutine);

            _attackRoutine = StartCoroutine(AttackWithDelay(target));
        }

        private IEnumerator AttackWithDelay(Enemy target)
        {
            while(target.IsAlive && target != null)
            {
                yield return new WaitForSeconds(_attackDelay);
                target.ApplyDamage(_damage);
            }
        }
    }
}