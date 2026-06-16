using Entity;
using UnityEngine;

namespace Playable
{
    public class EnemyController : MonoBehaviour
    {
        // TODO: Контроллер врагов (перемещение/передача в PlayableController событий, связанных с Enemy)
        [Header("Enemy Prefabs Setup")]
        [SerializeField] private Enemy _baseEnemyPrefab;
        [SerializeField] private Enemy _bossEnemyPrefab;

        [Space(5), Header("Enemy Count Settings")]
        [SerializeField] private int _orcEnemyCount = 6;
        [SerializeField] private int _bossEnemyCount = 1;
    }
}