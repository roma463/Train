using UnityEngine;

namespace _Train.Scripts.Character.MovementStateMachine.Standing
{
    public class CharacterWalkState : IState
    {
        private CharacterStateMachine _stateMachine;
        private CharacterStateType _currentType;
        private float _speedMove;
        private Transform _playerTransform;
        private Transform _cameraTransform;

        public CharacterWalkState(CharacterStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _currentType = CharacterStateType.Walk;
            _speedMove = _stateMachine.Context.MovementSettings.SpeedWalk;
            _playerTransform = _stateMachine.Context.Character.transform;
            _cameraTransform = _stateMachine.Context.Camera;
        }

        public void OnEnterState()
        {
            _stateMachine.Context.CharacterAnimation.SetTriggerByTypeState(_currentType);

            _stateMachine.Context.InputHandler.OnSprintPerformed += OnRunPerformed;
            _stateMachine.Context.InputHandler.OnMovementCancelled += OnMovementCancelled;
            _stateMachine.Context.InputHandler.OnJumpPerformed += OnJumpPerformed;
            _stateMachine.Context.InputHandler.OnCrouchPerformed += OnControlPerformed;
            _stateMachine.Context.Character.Collision.OnGroundedCanceled += OnGroundedCanceled;
        }

        public void OnExitState()
        {
            _stateMachine.Context.CharacterAnimation.ResetTriggerByTypeState(_currentType);
            
            _stateMachine.Context.InputHandler.OnSprintPerformed -= OnRunPerformed;
            _stateMachine.Context.InputHandler.OnMovementCancelled -= OnMovementCancelled;
            _stateMachine.Context.InputHandler.OnJumpPerformed -= OnJumpPerformed;
            _stateMachine.Context.InputHandler.OnCrouchPerformed -= OnControlPerformed;
            _stateMachine.Context.Character.Collision.OnGroundedCanceled -= OnGroundedCanceled;
        }

        private void OnControlPerformed()
        {
            _stateMachine.ChangeStateByType(CharacterStateType.SitMove);
        }

        private void OnGroundedCanceled()
        {
            _stateMachine.ChangeStateByType(CharacterStateType.Fall);
        }

        private void OnRunPerformed()
        {
            if (!_stateMachine.Context.Character.IsLockRestoreStamina)
                _stateMachine.ChangeStateByType(CharacterStateType.Run);
        }

        private void OnJumpPerformed()
        {
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


            var applyGravity = _stateMachine.Context.Character.transform.position.y >
                               _stateMachine.Context.Character.Collision.GroundPoint.y;
            
            _stateMachine.Context.Character.Move(moveDirection * _speedMove, applyGravity);

            Debug.DrawLine(_stateMachine.transform.position, moveDirection + _stateMachine.transform.position, Color.green);
        }
    }
}
