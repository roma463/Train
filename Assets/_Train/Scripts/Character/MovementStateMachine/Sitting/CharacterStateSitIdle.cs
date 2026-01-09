using UnityEngine;

namespace _Train.Scripts.Character.MovementStateMachine.Sitting
{
    public class CharacterStateSitIdle : IState
    {
        private CharacterStateMachine _stateMachine;

        public CharacterStateSitIdle(CharacterStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void OnEnterState()
        {
            _stateMachine.Context.CharacterAnimation.SetTriggerByTypeState(CharacterStateType.SitIdle);

            _stateMachine.Context.InputHandler.OnMovementPerformed += OnMovementPerformed;
            _stateMachine.Context.InputHandler.OnJumpPerformed += OnJumpPerformed;
            _stateMachine.Context.InputHandler.OnCrouchPerformed += OnControlPerformed;
            
            _stateMachine.Context.Character.Collision.OnGroundedCanceled += OnGroundedCanceled;

            ShrinkCollider();
        }

        public void OnExitState()
        {
            _stateMachine.Context.InputHandler.OnMovementPerformed -= OnMovementPerformed;
            _stateMachine.Context.InputHandler.OnJumpPerformed -= OnJumpPerformed;
            _stateMachine.Context.InputHandler.OnCrouchPerformed -= OnControlPerformed;
            
            _stateMachine.Context.Character.Collision.OnGroundedCanceled -= OnGroundedCanceled;

            ExpandCollider();
        }
        
        private void ShrinkCollider()
        {
            _stateMachine.Context.Character.CharacterCollider.height = _stateMachine.Context.MovementSettings.HeightCharacterBySit;
            var changeHeight = _stateMachine.Context.MovementSettings.HeightCharacterBySit / _stateMachine.Context.StartHeightCharacter;
            _stateMachine.Context.Character.CharacterCollider.center = Vector3.up * _stateMachine.Context.StartVectivalOffsetCharacterCollider * changeHeight;
        }

        private void ExpandCollider()
        {
            _stateMachine.Context.Character.CharacterCollider.height = _stateMachine.Context.StartHeightCharacter;
            _stateMachine.Context.Character.CharacterCollider.center = Vector3.up * _stateMachine.Context.StartVectivalOffsetCharacterCollider;
        }

        private void OnControlPerformed()
        {
            _stateMachine.ChangeStateByType(CharacterStateType.Idle);
        }

        private void OnGroundedCanceled()
        {
            _stateMachine.ChangeStateByType(CharacterStateType.Fall);
        }

        private void OnJumpPerformed()
        {
            _stateMachine.ChangeStateByType(CharacterStateType.Jump);
        }

        private void OnMovementPerformed()
        {
            _stateMachine.ChangeStateByType(CharacterStateType.SitMove);
        }

        public void Execute()
        {
            _stateMachine.Context.Character.Move(Vector3.zero, true);
        }

        public void FixedExecute()
        {
        }
    }
}
