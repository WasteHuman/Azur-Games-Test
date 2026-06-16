using UnityEngine;

namespace Entity
{
    public abstract class Unit : MonoBehaviour
    {
        protected int _damage;
        protected GameObject _view;
        protected int _level;

        public int Damage => _damage;
        public int Level => _level;
    }
}