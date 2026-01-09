
using _Train.Scripts.Root;
using UnityEngine;

namespace _Train.Scripts.Character.MovementStateMachine.Standing
{
    public class CharacterIdleState : IState
    {
        private CharacterStateMachine _stateMachine;

        public CharacterIdleState(CharacterStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void OnEnterState()
        {
            _stateMachine.Context.CharacterAnimation.SetTriggerByTypeState(CharacterStateType.Idle);

            _stateMachine.Context.InputHandler.OnMovementPerformed += OnMovementPerformed;
            _stateMachine.Context.InputHandler.OnJumpPerformed += OnJumpPerformed;
            _stateMachine.Context.InputHandler.OnCrouchPerformed += OnControlPerformed;
            _stateMachine.Context.Character.Collision.OnGroundedCanceled += OnGroundedCanceled;
        }

        public void OnExitState()
        {
            _stateMachine.Context.CharacterAnimation.ResetTriggerByTypeState(CharacterStateType.Idle);
            
            _stateMachine.Context.InputHandler.OnMovementPerformed -= OnMovementPerformed;
            _stateMachine.Context.InputHandler.OnJumpPerformed -= OnJumpPerformed;
            _stateMachine.Context.InputHandler.OnCrouchPerformed -= OnControlPerformed;
            _stateMachine.Context.Character.Collision.OnGroundedCanceled -= OnGroundedCanceled;
        }

        private void OnGroundedCanceled()
        {
            _stateMachine.ChangeStateByType(CharacterStateType.Fall);
        }

        private void OnControlPerformed()
        {
            _stateMachine.ChangeStateByType(CharacterStateType.SitIdle);
        }

        public void Execute() 
        {
        }

        public void FixedExecute()
        {
            if (_stateMachine.Context.Character.transform.position.y > _stateMachine.Context.Character.Collision.GroundPoint.y)
                _stateMachine.Context.Character.Move(Vector3.down * Mathf.Abs(_stateMachine.Context.Character.VerticalVelocity), true);
            else
                _stateMachine.Context.Character.Move(Vector3.zero, false);
        }

        private void OnJumpPerformed()
        {
            _stateMachine.ChangeStateByType(CharacterStateType.Jump);
        }

        private void OnMovementPerformed()
        {
            _stateMachine.ChangeStateByType(CharacterStateType.Walk);
        }
    }
}
