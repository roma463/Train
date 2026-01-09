using _Train.Scripts.Character.CameraCharacter;
using _Train.Scripts.Character.Data;
using UnityEngine;

namespace _Train.Scripts.Character
{
    public class CharacterContext : Context
    {
        [field: SerializeField] public Transform Camera { get; private set; }

        [field: SerializeField] public Character Character { get; private set; }
        [field: SerializeField] public CharacterMovementSettings MovementSettings { get; private set; }
        [field: SerializeField] public CharacterAnimation CharacterAnimation { get; private set; }
        [field: SerializeField] public CharacterCameraController CameraController { get; private set; }
        [field: SerializeField] public Material DefaultMaterial { get; private set; }
        [field: SerializeField] public Material DeathMaterial { get; private set; }
        [field: SerializeField] public GrabSystem GrabSystem { get; private set; }

        public INPUTE InputHandler => INPUTE.instance;
        public float StartHeightCharacter { get; private set; }
        public float StartVectivalOffsetCharacterCollider { get; private set; }
        public bool SkipFallDelayAnimation { get; set; }
        public bool WallJump { get; set; }
        public float LyingRotationDirection { get; set; }
    
        public void Init()
        {
            StartHeightCharacter = Character.CharacterCollider.height;
            StartVectivalOffsetCharacterCollider = Character.CharacterCollider.center.y;
        }
    }
}
