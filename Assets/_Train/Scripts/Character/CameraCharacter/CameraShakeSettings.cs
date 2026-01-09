using System.Collections.Generic;
using System.Linq;
using _Train.Scripts.Character.MovementStateMachine;
using UnityEngine;

namespace _Train.Scripts.Character.CameraCharacter
{
    [CreateAssetMenu(fileName = "CameraShakeSettings", menuName = "Camera shake settings")]
    public class CameraShakeSettings : ScriptableObject
    {
        [SerializeField] private float bobSmoothSpeed = 2f;
        [SerializeField] private float returnToCenterSpeed = 1f;
        [SerializeField] private List<CameraShakeData> cameraShakeTypes;
        [Header("Jump Bob Settings")]
        [SerializeField] private float jumpBobSmoothSpeed = 5f;
        [SerializeField] private float jumpBobAmount = 0.15f;
        [SerializeField] private float jumpBobDuration = 0.2f;
        [SerializeField] private float landBobAmount = 0.2f;
        [SerializeField] private float landBobDuration = 0.3f;
        
        public float BobSmoothSpeed => bobSmoothSpeed;
        public float ReturnToCenterSpeed => returnToCenterSpeed;
        public float JumpBobSmoothSpeed => jumpBobSmoothSpeed;
        public float JumpBobAmount => jumpBobAmount;
        public float JumpBobDuration => jumpBobDuration;
        public float LandBobAmount => landBobAmount;
        public float LandBobDuration => landBobDuration;

        public CameraShakeData GetShakeDataByType(CharacterStateType stateType)
        {
            var cameraShakeData = cameraShakeTypes.FirstOrDefault(x => x.stateType == stateType);
            
            return cameraShakeData;
        }
    }
}