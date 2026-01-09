using UnityEngine;

namespace _Train.Scripts.Character.MovementStateMachine.Sitting
{
    public class CharacterStateSitMove : IState
    {
        private CharacterStateMachine _stateMachine;
        private Transform _cameraTransform;
        private float _speedMove;

        public CharacterStateSitMove(CharacterStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _cameraTransform = _stateMachine.Context.Camera;
            _speedMove = _stateMachine.Context.MovementSettings.SpeedMoveSit;
        }

        public void OnEnterState()
        {
            _stateMachine.Context.CharacterAnimation.SetTriggerByTypeState(CharacterStateType.SitMove);

            _stateMachine.Context.InputHandler.OnMovementCancelled += OnMovementCancelled;
            _stateMachine.Context.InputHandler.OnJumpPerformed += OnJumpPerformed;
            _stateMachine.Context.InputHandler.OnCrouchPerformed += OnControlPerformed;
            _stateMachine.Context.Character.Collision.OnGroundedCanceled += OnGroundedCanceled;

            _stateMachine.Context.Character.CharacterCollider.height = _stateMachine.Context.MovementSettings.HeightCharacterBySit;
            var changeHieght = _stateMachine.Context.MovementSettings.HeightCharacterBySit / _stateMachine.Context.StartHeightCharacter;
            _stateMachine.Context.Character.CharacterCollider.center = Vector3.up * _stateMachine.Context.StartVectivalOffsetCharacterCollider * changeHieght;
        }

        public void OnExitState()
        {
            _stateMachine.Context.InputHandler.OnMovementCancelled -= OnMovementCancelled;
            _stateMachine.Context.InputHandler.OnJumpPerformed -= OnJumpPerformed;
            _stateMachine.Context.InputHandler.OnCrouchPerformed -= OnControlPerformed;
            _stateMachine.Context.Character.Collision.OnGroundedCanceled -= OnGroundedCanceled;

            _stateMachine.Context.Character.CharacterCollider.height = _stateMachine.Context.StartHeightCharacter;
            _stateMachine.Context.Character.CharacterCollider.center = Vector3.up * _stateMachine.Context.StartVectivalOffsetCharacterCollider;
        }

        private void OnControlPerformed()
        {
            if (_stateMachine.Context.InputHandler.IsRunning)
                _stateMachine.ChangeStateByType(CharacterStateType.Run);
            else
                _stateMachine.ChangeStateByType(CharacterStateType.Walk);
        }

        private void OnGroundedCanceled()
        {
            _stateMachine.ChangeStateByType(CharacterStateType.Fall);
        }

        private void OnJumpPerformed()
        {
            _stateMachine.ChangeStateByType(CharacterStateType.Jump);
        }

        private void OnMovementCancelled()
        {
            _stateMachine.ChangeStateByType(CharacterStateType.SitIdle);
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
            }
            
            _stateMachine.Context.Character.Move(moveDirection * _speedMove);
        }
    }
}
