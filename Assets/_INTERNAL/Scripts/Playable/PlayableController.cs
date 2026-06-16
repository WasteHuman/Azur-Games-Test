using Field;
using UnityEngine;

namespace Playable
{
    public class PlayableController : MonoBehaviour
    {
        [Header("Grid Setup")]
        [SerializeField] private GridBuilder _gridBuilder;
        [SerializeField] private GridController _gridController;

        private void Start()
        {
            if (_gridBuilder == null && _gridController == null)
                throw new MissingReferenceException("Grid controller or grid builder is null!");

            _gridController.InjectGrid(_gridBuilder.Cells);
        }
    }
}