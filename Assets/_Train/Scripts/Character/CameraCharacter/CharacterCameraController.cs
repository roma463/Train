using System;
using _Train.Scripts.Root;
using _Train.Scripts.Character.MovementStateMachine;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

namespace _Train.Scripts.Character.CameraCharacter
{
    public class CharacterCameraController : NetworkBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float mouseSensitivity = 100f;
        [SerializeField] private float maxLookAngle = 90f;
        [SerializeField] private float maxLyingLookAngle = 30f;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform lookPosition;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Character character;
        [SerializeField] private CharacterStateMachine stateMachine;

        [Header("Camera Settings")]
        public float rotationSmoothTime = 0.1f;

        [Header("Camera Positions")] 
        [SerializeField] private float cameraStandPosition = -0.13f;
        [SerializeField] private float cameraSitPosition = -0.19f;
        [SerializeField] private float cameraLiePosition = -0.5f;
    
        private float _xRotation = 0f;
    
        private float _xRotationVelocity;
        private float _yRotationVelocity;

        private float _maxLeftAngle;
        private float _maxRightAngle;
        private float _fixedRotationCenter;
        
        [SerializeField] private bool _cameraLocked;
        
        private Quaternion _targetCameraRotation;
        private Quaternion _targetBodyRotation;
        private Quaternion _targetBodySmoothRotation;
        
        private float bodyYRotation = 0f;
        
        private Vector2 LookDirection => INPUTE.instance.MouseDelta;
        
        
        private void Start()
        {
            _targetCameraRotation = cameraTransform.localRotation;
            _targetBodyRotation = transform.localRotation;
            stateMachine.OnStateChanged += ChangeCameraPosition;
        }

        private void OnDestroy()
        {
            stateMachine.OnStateChanged -= ChangeCameraPosition;
        }

        private void ChangeCameraPosition(CharacterStateType stateType)
        {
            switch (stateType)
            {
                case CharacterStateType.SitIdle:
                case CharacterStateType.SitMove:
                    cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, cameraSitPosition, cameraTransform.localPosition.z);
                    break;
                default:
                    cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, cameraStandPosition, cameraTransform.localPosition.z);
                    break;
            }
        }

        private void Update()
        {
            if (isLocalPlayer)
            {
                UpdateCameraRotation();
            }
        }

        public void LockCamera()
        {
            _cameraLocked = true;
        }

        public void UnlockCamera()
        {
            _cameraLocked = false;
        }
        
        private void UpdateCameraRotation()
        {
            if (_cameraLocked)
                return;
            
            Vector2 look = (LookDirection) * Time.deltaTime;
            
            _xRotation -= look.y;
            _xRotation = Mathf.Clamp(_xRotation, -maxLookAngle, maxLookAngle);
            
            bodyYRotation += look.x;
            _targetCameraRotation = Quaternion.Euler(_xRotation, 0f, 0f);

            var yRotate = look.x;
            
            if (character.IsPassenger)
            {
                Quaternion parentWorldRotation = transform.root.rotation;

                yRotate += bodyYRotation;
                Quaternion relativeRotation = Quaternion.Euler(0f, yRotate, 0f);
                _targetBodyRotation = parentWorldRotation * relativeRotation;
            }
            else
            {
                _targetBodyRotation = Quaternion.Euler(0f, _targetBodyRotation.eulerAngles.y + yRotate, 0f);
            }

            float smoothFactor = Time.deltaTime * rotationSmoothTime;
            
            cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, _targetCameraRotation, smoothFactor);
            var rotation = Quaternion.Slerp(transform.rotation, _targetBodyRotation, smoothFactor);
            rb.MoveRotation(rotation);
        }
    }
}
