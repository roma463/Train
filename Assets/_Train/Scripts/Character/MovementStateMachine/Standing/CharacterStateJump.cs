using System.Collections;
using _Train.Scripts.Character.MovementStateMachine.Airborne;
using UnityEngine;

namespace _Train.Scripts.Character.MovementStateMachine.Standing
{
    public class CharacterStateJump : CharacterAirborneState
    {
        private float _verticalVelocity;
        private Coroutine _jumpRoutine;

        public CharacterStateJump(CharacterStateMachine stateMachine) : base(stateMachine) { }

        public override void OnEnterState()
        {
            Jump();
            _stateMachine.Context.Energy.SpendForJump();
            base.OnEnterState();
        }

        public override void OnExitState()
        {
            base.OnExitState();
            _verticalVelocity = 0;

            if (_stateMachine.Context.WallJump)
                _stateMachine.Context.WallJump = false;

            if (_jumpRoutine != null)
                StopJumpRoutine();
        }

        protected override void OnGroundCollision()
        {
            if (_jumpRoutine != null)
                StopJumpRoutine();

            base.OnGroundCollision();
        }

        protected override float GetVerticalVelocity() => _verticalVelocity;

        private void Jump()
        {
            _stateMachine.Context.CharacterAnimation.SetTriggerByTypeState(CharacterStateType.Jump);
            _jumpRoutine = _stateMachine.StartCoroutine(AddForceByCurve());
        }

        private IEnumerator AddForceByCurve()
        {
            var force = _stateMachine.Context.MovementSettings.JumpForce;
            var curve = _stateMachine.Context.MovementSettings.AddForceCurve;

            var timeJumpAnimation = _stateMachine.Context.MovementSettings.TimeJumpAnimation;
            float currentTimeAnimation = 0;

            while (currentTimeAnimation < timeJumpAnimation)
            {
                var positionOnCurve = currentTimeAnimation != 0 
                    ? currentTimeAnimation / timeJumpAnimation * curve.keys[curve.length - 1].time
                    : 0;
                
                var value = curve.Evaluate(positionOnCurve);

                _verticalVelocity = value * force;
                currentTimeAnimation += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _jumpRoutine = null;

           ChangeState();
        }

        private void ChangeState()
        {
            if (!IsGrounded)
            {
                _stateMachine.ChangeStateByType(CharacterStateType.Fall);
            }
            else
            {
                if (_stateMachine.Context.InputHandler.MovementDirection != Vector2.zero)
                {
                    if (_stateMachine.Context.InputHandler.IsSprintPerformed)
                        _stateMachine.ChangeStateByType(CharacterStateType.Run);
                    else
                        _stateMachine.ChangeStateByType(CharacterStateType.Walk);
                }
                else
                {
                    _stateMachine.ChangeStateByType(CharacterStateType.Idle);
                }
            }
        }
        
        private void StopJumpRoutine()
        {
            _stateMachine.StopCoroutine(_jumpRoutine);
            _jumpRoutine = null;
        }
    }
}
