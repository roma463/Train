using UnityEngine;

namespace _Train.Scripts.Character.MovementStateMachine.Airborne
{
    public abstract class CharacterAirborneState : IState
    {
        protected CharacterStateMachine _stateMachine;
        protected Transform CharacterTransform => _stateMachine.Context.transform;
        protected Transform CameraTransform => _stateMachine.Context.Camera;

        protected bool IsGrounded => _stateMachine.Context.Character.Collision.IsGrounded;
        protected virtual bool ApplyGravity => false;
        protected CharacterAirborneState(CharacterStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public virtual void OnEnterState()
        {
            _stateMachine.Context.Character.Collision.OnGroundedPerformed += OnGroundCollision;
        }

        public virtual void OnExitState()
        {
            _stateMachine.Context.Character.Collision.OnGroundedPerformed -= OnGroundCollision;
        }

        public virtual void Execute()
        {
        }

        public virtual void FixedExecute()
        {
            ApplyMovement();
        }

        protected virtual void OnGroundCollision()
        {
            ChangeStateToGrounded();
        }

        protected void ChangeStateToGrounded()
        {
            if (_stateMachine.Context.InputHandler.IsMovementPerformed)
            {
                if (_stateMachine.Context.InputHandler.IsRunning)
                    _stateMachine.ChangeStateByType(CharacterStateType.Run);
                else
                    _stateMachine.ChangeStateByType(CharacterStateType.Walk);
            }
            else
            {
                _stateMachine.ChangeStateByType(CharacterStateType.Idle);
            }
        }

        protected void ApplyMovement()
        {
            var moveDirection = GetMoveDirection();
            moveDirection.y = GetVerticalVelocity();
            _stateMachine.Context.Character.Move(moveDirection, ApplyGravity);
        }

        protected Vector3 GetMoveDirection()
        {
            var input = _stateMachine.Context.InputHandler.MovementDirection;

            var characterForward = CharacterTransform.forward;
            characterForward.y = 0f;
            characterForward.Normalize();

            var characterRight = CharacterTransform.right.normalized;
            var moveDirection = (characterForward * input.y + characterRight * input.x).normalized;

            return moveDirection * GetMovementSpeed();
        }

        protected float GetMovementSpeed()
        {
            if (!_stateMachine.Context.InputHandler.IsMovementPerformed)
                return 0f;

            return _stateMachine.Context.InputHandler.IsRunning
                ? _stateMachine.Context.MovementSettings.SpeedRun
                : _stateMachine.Context.MovementSettings.SpeedWalk;
        }

        protected abstract float GetVerticalVelocity();
    }
}