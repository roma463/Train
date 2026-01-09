using UnityEngine;

namespace _Train.Scripts.Character.Data
{
    [CreateAssetMenu(fileName = "CharacterBaseData", menuName ="CharacterBaseData")]
    public class CharacterBaseData : ScriptableObject
    {
        [field: SerializeField] public float BagOpenTime { get; private set; }
        [field: SerializeField] public float MinTimeOpen { get; private set; }
        [field: SerializeField] public float MaxVerticalVelocity { get; private set; }
        [field: SerializeField] public float TimeForStartRestoreStamina { get; private set; } = 1f;
    }
}
