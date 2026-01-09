using _Train.Scripts.Character.MovementStateMachine;
using UnityEngine;

namespace _Train.Scripts.Character.CameraCharacter
{
    public class CameraShake : MonoBehaviour
    {
        [Header("Head Bob Settings")]
        [SerializeField] private bool enableHeadBob = true;
        [SerializeField] private CameraShakeSettings shakeSettings;
        [Space]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Character character;
        [SerializeField] private CharacterStateMachine characterStateMachine;
        
        
        private Vector3 _originalRotation;
        private float _bobTimer = 0;
        private float _currentBobFrequency;
        private float _currentBobHorizontalAmount;
        private float _currentBobVerticalAmount;
        
        private Vector3 _targetRotation;
        private Vector3 _currentRotation;
        
        // Jump/Land variables
        private float _jumpBobTimer = 0f;
        private float _landBobTimer = 0f;
        private bool _isJumping = false;
        private bool _wasGrounded = true;
        private Vector3 _jumpBobRotation = Vector3.zero;
        private Vector3 _landBobRotation = Vector3.zero;
        private float _currentSmoothSped;

        void Start()
        {
            _currentSmoothSped = shakeSettings.BobSmoothSpeed;
            _originalRotation = playerCamera.transform.localEulerAngles;
        }

        void Update()
        {
            _originalRotation = playerCamera.transform.localEulerAngles;
            
            if (!enableHeadBob || Cursor.visible) 
                return;
            
            var shakeData = shakeSettings.GetShakeDataByType(characterStateMachine.CurrentStateType);

            if (shakeData != null)
            {
                _currentBobFrequency = shakeData.bobFrequency;
                _currentBobHorizontalAmount = shakeData.bobHorizontalAmount;
                _currentBobVerticalAmount = shakeData.bobVerticalAmount;
            }
            
            HandleGroundedCheck();
            HandleHeadBob();
            HandleJumpBob();
            HandleLandBob();
            CombineRotations();
        }
        
        private void HandleGroundedCheck()
        {
            var isGrounded = character.Collision.IsGrounded;
        
            if (!isGrounded && _wasGrounded && !_isJumping)
            {
                Debug.Log("Jump");
                StartJumpBob();
            }
        
            if (isGrounded && !_wasGrounded && _isJumping)
            {
                Debug.Log("Land");
                StartLandBob();
            }
        
            _wasGrounded = isGrounded;
        }
        
        private void StartJumpBob()
        {
            _isJumping = true;
            _jumpBobTimer = shakeSettings.JumpBobDuration;
        }
        
        private void StartLandBob()
        {
            _isJumping = false;
            _landBobTimer = shakeSettings.LandBobDuration;
        }

        private void HandleHeadBob()
        {
            var shakeData = shakeSettings.GetShakeDataByType(characterStateMachine.CurrentStateType);

            if (shakeData != null)
            {
                _currentBobFrequency = shakeData.bobFrequency;
                _currentBobHorizontalAmount = shakeData.bobHorizontalAmount;
                _currentBobVerticalAmount = shakeData.bobVerticalAmount;
            }

            if (shakeData != null)
            {
                _bobTimer += Time.deltaTime * _currentBobFrequency;
                
                var horizontalRot = Mathf.Sin(_bobTimer) * _currentBobHorizontalAmount;
                var verticalRot = (Mathf.Sin(_bobTimer * 2) * _currentBobVerticalAmount) * 0.5f;
                
                _targetRotation = new Vector3(verticalRot, 0f, -horizontalRot);
            }
            else
            {
                _targetRotation = Vector3.zero;
                _bobTimer = 0;
            }

            _currentRotation = Vector3.Lerp(_currentRotation, _targetRotation, Time.deltaTime * 
                (shakeData != null ? _currentSmoothSped : shakeSettings.ReturnToCenterSpeed));
            
            playerCamera.transform.localEulerAngles = _originalRotation + _currentRotation;
        }
        
        private void HandleJumpBob()
        {
            if (_jumpBobTimer > 0)
            {
                _jumpBobTimer -= Time.deltaTime;
                var normalizedTime = 1f - (_jumpBobTimer / shakeSettings.JumpBobDuration);
                
                var jumpCurve = Mathf.Sin(normalizedTime * Mathf.PI * 0.5f);
                var bobIntensity = jumpCurve * shakeSettings.JumpBobAmount;
                
                _jumpBobRotation = new Vector3(-bobIntensity * 0.7f, 0f, 0f);
            }
            else
            {
                _jumpBobRotation = Vector3.zero;
                if (!character.Collision.IsGrounded)
                    _isJumping = false;
            }
        }

        private void HandleLandBob()
        {
            if (_landBobTimer > 0)
            {
                _currentSmoothSped = shakeSettings.JumpBobSmoothSpeed;
                _landBobTimer -= Time.deltaTime;
                var normalizedTime = 1f - (_landBobTimer / shakeSettings.LandBobDuration);
                
                var landCurve = Mathf.Sin(normalizedTime * Mathf.PI);
                var bobIntensity = landCurve * shakeSettings.LandBobAmount;
                
                _landBobRotation = new Vector3(bobIntensity * 0.8f, 0f, Mathf.Sin(normalizedTime * Mathf.PI * 2f) * shakeSettings.LandBobAmount * 0.3f);
            }
            else
            {
                _currentSmoothSped = shakeSettings.BobSmoothSpeed;
                _landBobRotation = Vector3.zero;
            }
        }

        private void CombineRotations()
        {
            var finalRotation = _targetRotation + _jumpBobRotation + _landBobRotation;
            
            _currentRotation = Vector3.Lerp(_currentRotation, finalRotation, Time.deltaTime * 
                (character.Collision.IsGrounded ? _currentSmoothSped : shakeSettings.ReturnToCenterSpeed));
            
            playerCamera.transform.localEulerAngles = playerCamera.transform.localEulerAngles = _originalRotation + _currentRotation;
        }
    }
}