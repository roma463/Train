using UnityEngine;

namespace _Train.Scripts.Character.Data
{
    [CreateAssetMenu(fileName = "CharacterMovementSettings", menuName ="CharacterMovementSettings")]
    public class CharacterMovementSettings : ScriptableObject
    {
        [field: SerializeField] public float Gravity { get; private set; } = -1f;

        [field: SerializeField, Space] public float SpeedWalk { get; private set; }
        [field: SerializeField] public float SpeedRun { get; private set; }
        [field: SerializeField] public float SpeedRotate { get; private set; }
        [field: SerializeField, Range(0f, 1f)] public float VerticalNormalGround { get; private set; }

        [field: SerializeField, Space] public float JumpForce { get; private set; }
        [field: SerializeField] public float TimeJumpAnimation { get; private set; }
        [field: SerializeField] public AnimationCurve AddForceCurve { get; private set; }
        [field: SerializeField, Range(0, 100)] public int RemoveStaminaOnJumpInPercent { get; private set; }

        [field: SerializeField, Space] public float VelocityForPlayFall { get; private set; } = -0.2f;

        [field: SerializeField, Space] public float SpeedRunByWall { get; private set; }
        [field: SerializeField] public float OffsetByYForRunWall { get; private set; }
        [field: SerializeField] public float DurationRunByWall { get; private set; }

        [field: SerializeField, Space] public float SpeedRunByWallVertical { get; private set; }
        [field: SerializeField] public float DurationRunByWallVertical { get; private set; }
    
        [field: SerializeField, Space] public float HeightCharacterBySlide { get; private set; }

        [field: SerializeField, Space] public float SpeedMoveSit { get; private set; }
        [field: SerializeField] public float HeightCharacterBySit { get; private set; }
    
        [field: SerializeField, Space] public float MaxDropTime { get; private set; } = 1;
        [field: SerializeField, Space] public float MinTimeForDropWithForce { get; private set; } = 0.1f;
        [field: SerializeField] public float MaxTimeForDropWithForce { get; private set; } = 1f;
        [field: SerializeField] public float MaxDropForce { get; private set; } = 1;

        private void OnValidate()
        {
            if (Gravity > VelocityForPlayFall)
                VelocityForPlayFall = Gravity;
        }
    }
}
