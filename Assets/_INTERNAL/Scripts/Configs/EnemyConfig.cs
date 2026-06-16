using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(menuName = "Configs/Enemy/Enemy Config", fileName = "EnemyConfig")]
    public class EnemyConfig : ScriptableObject
    {
        [field: SerializeField] public EnemyConfigData Enemy { get; private set; }
    }
}