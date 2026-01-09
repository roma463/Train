using UnityEngine;

namespace _Train.Scripts.Character.MovementStateMachine.Airborne
{
    public class CharacterStateFall : CharacterAirborneState
    {
        private bool _playAnimation;
        protected override bool ApplyGravity => false;

        public CharacterStateFall(CharacterStateMachine stateMachine) : base(stateMachine) { }

        public override void OnEnterState()
        {
            base.OnEnterState();
            _stateMachine.Context.CharacterAnimation.SetTriggerByTypeState(CharacterStateType.Fall);
        }

        public override void OnExitState()
        {
            base.OnExitState();
           
            _playAnimation = false;
        }

        public override void Execute()
        {
            if (_stateMachine.Context.Character.Collision.WillDamage)
            {
                _stateMachine.Context.CharacterAnimation.ResetTriggerByTypeState(CharacterStateType.Fall);
                return;
            }
            
            if (IsGrounded)
                OnGroundCollision();
        }

        protected override float GetVerticalVelocity()
        {
            var gravityEffect = _stateMachine.Context.Character.Gravity * Time.deltaTime;
            var verticalVel = _stateMachine.Context.Character.VerticalVelocity + gravityEffect;

            _stateMachine.Context.Character.VerticalVelocity = verticalVel;

            _playAnimation = true;
            _stateMachine.Context.CharacterAnimation.SetTriggerByTypeState(CharacterStateType.Fall);

            return verticalVel;
        }
    }
}
