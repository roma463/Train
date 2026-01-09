using System;
using System.Collections.Generic;
using _Train.Scripts.Character.MovementStateMachine.Airborne;
using _Train.Scripts.Character.MovementStateMachine.Sitting;
using _Train.Scripts.Character.MovementStateMachine.Standing;
using UnityEngine;

namespace _Train.Scripts.Character.MovementStateMachine
{
    public class CharacterStateMachine : StateMachine<CharacterContext>
    {
        public event Action<CharacterStateType> OnStateChanged;
        
        [SerializeField] private CharacterStateType _currentTypeState;

        private Dictionary<CharacterStateType, IState> _stateHandlers;
        
        public CharacterStateType CurrentStateType => _currentTypeState;

        public override void Initialize()
        {
            base.Initialize();
            Context.Init();

            _stateHandlers = new Dictionary<CharacterStateType, IState>()
            {
                { CharacterStateType.Idle, new CharacterIdleState(this) },
                { CharacterStateType.Walk, new CharacterWalkState(this) },
                { CharacterStateType.Run, new CharacterRunState(this) },
                { CharacterStateType.SitMove, new CharacterStateSitMove(this) },
                { CharacterStateType.Jump, new CharacterStateJump(this) },
                { CharacterStateType.Fall, new CharacterStateFall(this) },
                { CharacterStateType.SitIdle, new CharacterStateSitIdle(this) },
                { CharacterStateType.Death, new CharacterDeathState(this) },
            };

            ChangeStateByType(CharacterStateType.Idle);
        }

        public void ChangeStateByType(CharacterStateType characterStateType)
        {
            _currentTypeState = characterStateType;
            OnStateChanged?.Invoke(_currentTypeState);
            ChangeState(_stateHandlers[characterStateType]);
        }
    }

    public enum CharacterStateType
    {
        None,
        Idle,
        Walk,
        Run,
        Jump,
        Fall,
        SitMove,
        SitIdle,
        Death,
    }
}