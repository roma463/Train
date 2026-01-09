using System;
using _Train.Scripts.Character.CameraCharacter;
using _Train.Scripts.Character.MovementStateMachine;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Train.Scripts.Character
{
    public class CharacterAnimation : NetworkBehaviour
    {
        public event Action OnDropItem; 
        
        [SerializeField] private Animator _animator;
        // [SerializeField] private Animator armAnimator;
        [SerializeField] private CharacterCameraController _cameraController;
        [SerializeField] private Transform _camera;
        [SerializeField] private NetworkAnimator _networkAnimator;
        [SerializeField] private Transform _headDirectionPoint;

        private CharacterStateType _currentState = CharacterStateType.None;
        private INPUTE _input;
        private string _lastAnimationName;
        private bool _dontReset;

        private void Start()
        {
            _input = INPUTE.instance;
        }
        
        private void Update()
        {
            if (isOwned)
            {
                // _animator.SetFloat("VelosityX", _input.MovementDirection.x);
                // _animator.SetFloat("VelosityY", _input.MovementDirection.y);
                // armAnimator.SetFloat("VelosityY", _input.MovementDirection.y);
                
                _headDirectionPoint.position =  _camera.transform.position + _camera.transform.forward * 2;
            }
        }
        
        public void SetTriggerByTypeState(CharacterStateType type)
        {
            if (_currentState == type)
                return;
            
            _currentState = type;
            SetTriggerByName(type.ToString());
        }

        public void ResetTriggerByTypeState(CharacterStateType type)
        {
            if (_currentState == type)
                return;
            
            _currentState = type;
            ResetTriggerByName(type.ToString());
        }

        public void OnDropItemEvent()
        {
            OnDropItem?.Invoke();
        }

        public void SetBagBlendShapes()
        {
            // bagMeshRenderer.SetBlendShapeWeight(0, bagController.CountFilledSlots);
        }

        public void SetTriggerByName(string triggerName)
        {
            SetTriggerByName(triggerName, true, false);
        }

        public void SetTriggerByName(string triggerName, bool dontReset = false)
        {
            SetTriggerByName(triggerName, true, dontReset);
        }

        public void SetTriggerByName(string triggerName, bool isArmAnimationChange = true, bool dontReset = false)
        {
            _networkAnimator.SetTrigger(triggerName);
            _animator.SetTrigger(triggerName);

            if (isArmAnimationChange)
            {
                if (!string.IsNullOrEmpty(_lastAnimationName) && !_dontReset)
                {
                    _networkAnimator.ResetTrigger(_lastAnimationName);
                    _animator.ResetTrigger(_lastAnimationName);
                    // armAnimator.ResetTrigger(_lastAnimationName);
                }
                // armAnimator.SetTrigger(triggerName);
            }
            
            _dontReset = dontReset;
            _lastAnimationName = triggerName;
        }

        public void ResetTriggerByName(string triggerName, bool isArmAnimationChange = true)
        {
            _networkAnimator.ResetTrigger(triggerName);
            _animator.ResetTrigger(triggerName);
            
            // if(isArmAnimationChange)
            //     armAnimator.ResetTrigger(triggerName);
        }
    }
}
