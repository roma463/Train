using UnityEngine;

namespace _Train.Scripts.Character.MovementStateMachine
{
    public class CharacterRunState : IState
    {
        private CharacterStateMachine _stateMachine;
        private CharacterStateType _currentType;
        private float _speedMove;
        private Transform _playerTransform;
        private Transform _cameraTransform;
        private bool _staminaIsLow;

        public CharacterRunState(CharacterStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _currentType = CharacterStateType.Run;
            _speedMove = _stateMachine.Context.MovementSettings.SpeedRun;
            _playerTransform = _stateMachine.Context.Character.transform;
            _cameraTransform = _stateMachine.Context.Camera;
        }

        public void OnEnterState()
        {
            _stateMachine.Context.CharacterAnimation.SetTriggerByTypeState(_currentType);

            _stateMachine.Context.InputHandler.OnSprintCancelled += CancelRun;
            _stateMachine.Context.InputHandler.OnMovementCancelled += OnMovementCancelled;
            _stateMachine.Context.InputHandler.OnJumpPerformed += OnJumpPerformed;
            _stateMachine.Context.Character.Collision.OnGroundedCanceled += OnGroundedCanceled;
            _stateMachine.Context.Character.SetRunIntent(true);

            _staminaIsLow = false;
        }

        public void OnExitState()
        {
            _stateMachine.Context.CharacterAnimation.ResetTriggerByTypeState(_currentType);
            
            _stateMachine.Context.InputHandler.OnSprintCancelled -= CancelRun;
            _stateMachine.Context.InputHandler.OnMovementCancelled -= OnMovementCancelled;
            _stateMachine.Context.InputHandler.OnJumpPerformed -= OnJumpPerformed;
            _stateMachine.Context.Character.Collision.OnGroundedCanceled -= OnGroundedCanceled;
            _stateMachine.Context.Character.SetRunIntent(false);
        }

        private void OnGroundedCanceled()
        {
            _stateMachine.ChangeStateByType(CharacterStateType.Fall);
        }

        private void CancelRun()
        {
            _stateMachine.ChangeStateByType(CharacterStateType.Walk);
        }

        private void OnJumpPerformed()
        {
            if (!_stateMachine.Context.Character.HasEnergyForJump)
                return;
            _stateMachine.Context.Character.TrySpendEnergyForJump();
            _stateMachine.ChangeStateByType(CharacterStateType.Jump);
        }

        private void OnMovementCancelled()
        {
            _stateMachine.ChangeStateByType(CharacterStateType.Idle);
        }

        public void Execute()
        {
        
        }

        public void FixedExecute()
        {
            ApplyMovement();
        }

        private void ApplyMovement()
        {
            var input = _stateMachine.Context.InputHandler.MovementDirection;

            var characterForward = _stateMachine.transform.forward;
            characterForward.y = 0f;
            characterForward.Normalize();

            var characterRight = _stateMachine.transform.right.normalized;
            
            var moveDirection = (characterForward * input.y + characterRight * input.x).normalized;
            moveDirection = Vector3.ProjectOnPlane(moveDirection, _stateMachine.Context.Character.Collision.GroundNormal).normalized;

            if (moveDirection.y > _stateMachine.Context.MovementSettings.VerticalNormalGround)
            {
                moveDirection.z = 0f;
                moveDirection.y = 0f;

                Debug.DrawLine(_stateMachine.transform.position, moveDirection + _stateMachine.transform.position, Color.red);
            }

            if (_stateMachine.Context.Character.transform.position.y > _stateMachine.Context.Character.Collision.GroundPoint.y)
                _stateMachine.Context.Character.Move(moveDirection * _speedMove, true);
            else
                _stateMachine.Context.Character.Move(moveDirection * _speedMove, false);
            
            Debug.DrawLine(_stateMachine.transform.position, moveDirection + _stateMachine.transform.position, Color.green);
        }
    }
}
