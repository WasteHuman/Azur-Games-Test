using UnityEngine;

namespace Playable.UI
{
    public class HpAnchor : MonoBehaviour
    {
        [SerializeField] private Transform _point;

        public Transform Point => _point;
    }
}