using System;
using _Train.Scripts.Character.MovementStateMachine;
using UnityEngine;

namespace _Train.Scripts.Character.CameraCharacter
{
    [Serializable]
    public class CameraShakeData
    {
        public CharacterStateType stateType;
        [Space]
        public float bobFrequency = 1.5f;
        public float bobHorizontalAmount = 0.05f;
        public float bobVerticalAmount = 0.05f;
    }
}