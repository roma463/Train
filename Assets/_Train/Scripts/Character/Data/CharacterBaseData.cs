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
        
        [field: SerializeField] public int MaxEnergy { get; private set; } = 100;
        [field: SerializeField] public int IdleEnergyPerSecond { get; private set; } = 1;
        [field: SerializeField] public int WalkEnergyPerSecond { get; private set; } = 12;
        [field: SerializeField] public int RunEnergyPerSecond { get; private set; } = 12;
        [field: SerializeField] public int JumpEnergyPerSecond { get; private set; } = 12;
    }
}
