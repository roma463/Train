using System;
using _Train.Scripts.Root;
using _Train.Scripts.Character.MovementStateMachine;
using Mirror;
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

        private bool _isFixed;
        [SerializeField] private bool _cameraLocked;
        
        private Quaternion _targetCameraRotation;
        private Quaternion _targetBodyRotation;
        private Quaternion _targetBodySmoothRotation;
        
        [HideInInspector] public float camYRotation = 0f;
        [HideInInspector] public float bodyYRotation = 0f;
        
        private Vector2 LookDirection => INPUTE.instance.MouseDelta;
        public Transform LookPosition => lookPosition;
        public Transform CameraTransform => cameraTransform;
        
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
        
        public void FixateCamera()
        {
            _fixedRotationCenter = cameraTransform.rotation.eulerAngles.y;
            SetRotationLimits(-maxLyingLookAngle, maxLyingLookAngle);
            _isFixed = true;
        }

        public void FreeCamera()
        {
            SetRotationLimits(0f, 0f);
            _isFixed = false;
        }

        public void LockCamera()
        {
            _cameraLocked = true;
        }

        public void UnlockCamera()
        {
            _cameraLocked = false;
        }

        public void ResetCameraLimits()
        {
            if (_isFixed)
            {
                _fixedRotationCenter = rb.rotation.eulerAngles.y;
                SetRotationLimits(-maxLyingLookAngle, maxLyingLookAngle);
            }
            else
            {
                SetRotationLimits(0f, 0f);
            }
        }
        
        private void UpdateCameraRotation()
        {
            if (_cameraLocked)
                return;
            
            Vector2 look = (LookDirection * mouseSensitivity) * Time.deltaTime;

            // Обновление целевого вращения
            camYRotation += look.x;
            _xRotation -= look.y;
            _xRotation = Mathf.Clamp(_xRotation, -maxLookAngle, maxLookAngle);

            if (_isFixed)
            {
                _fixedRotationCenter = rb.rotation.eulerAngles.y;
                float minRotation = _fixedRotationCenter + _maxLeftAngle;
                float maxRotation = _fixedRotationCenter + _maxRightAngle;
                
                camYRotation = Mathf.Clamp(camYRotation, minRotation, maxRotation);
                
                _targetCameraRotation = Quaternion.Euler(_xRotation, camYRotation, 0f);
            }
            else
            {
                bodyYRotation += look.x;
                _targetCameraRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            }

            var yRotate = bodyYRotation;
            
            if (character.IsPassenger)
            {
                yRotate += transform.root.eulerAngles.y;
            }
            
            _targetBodyRotation = Quaternion.Euler(0f, yRotate, 0f);

            // Плавное вращение
            float smoothFactor = Time.deltaTime * rotationSmoothTime;

            if (_isFixed)
            {
                cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, _targetCameraRotation, smoothFactor);
            }
            else
            {
                cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, _targetCameraRotation, smoothFactor);
            }
            
            if (!_isFixed)
            {
                var rotation = Quaternion.Slerp(transform.rotation, _targetBodyRotation, smoothFactor);
                rb.MoveRotation(rotation);
            }
        }
        
        public void SetRotationLimits(float leftAngle, float rightAngle)
        {
            _maxLeftAngle = leftAngle;
            _maxRightAngle = rightAngle;
        
            // Если фиксация активна, применяем новые ограничения
            if (_isFixed)
            {
                float minRotation = _fixedRotationCenter + _maxLeftAngle;
                float maxRotation = _fixedRotationCenter + _maxRightAngle;
                camYRotation = Mathf.Clamp(camYRotation, minRotation, maxRotation);
            }
        }
    }
}
