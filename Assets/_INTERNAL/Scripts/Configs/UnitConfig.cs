using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(menuName = "Configs/Units/Unit Config", fileName = "UnitConfig")]
    public class UnitConfig : ScriptableObject
    {
        [field: SerializeField] public UnitConfigData Unit { get; private set; }
    }
}