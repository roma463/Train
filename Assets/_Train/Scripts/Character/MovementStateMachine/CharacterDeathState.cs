using UnityEngine;

namespace _Train.Scripts.Character.MovementStateMachine
{
    public class CharacterDeathState : IState
    {
        private CharacterStateMachine _stateMachine;

        public CharacterDeathState(CharacterStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public void OnEnterState()
        {
            _stateMachine.Context.Character.LockPlayer();
            _stateMachine.Context.CameraController.LockCamera();
            _stateMachine.Context.Character.ChangeToDeathMaterial();
            _stateMachine.Context.CharacterAnimation.SetTriggerByTypeState(CharacterStateType.Death);
            
            _stateMachine.Context.Character.CharacterRigidbody.linearVelocity = Vector3.zero;
        }

        public void OnExitState()
        {
            
        }
        
        public void Execute()
        {
            
        }

        public void FixedExecute()
        {
            if (!_stateMachine.Context.Character.Collision.IsGrounded)
            {
                _stateMachine.Context.Character.Move(Vector3.down * Mathf.Abs(_stateMachine.Context.Character.VerticalVelocity), true);
            }
        }
    }
}