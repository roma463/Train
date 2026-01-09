using System;
using System.Collections;
using _Train.Scripts.Root;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class INPUTE : MonoBehaviour
{
    public static INPUTE instance;

    public event Action OnPerformedR;
    public event Action OnPerformedEscape;
    public event Action OnPerformedGrab;
    public event Action OnCanceledGrab;
    public event Action OnPerformedDrop;
    public event Action OnCanceledDrop;
    public event Action OnLeftMouseButtonPerformed;
    public event Action OnLeftMouseButtonCanceled;
    public event Action OnSprintPerformed;
    public event Action OnSprintCancelled;
    public event Action OnMovementPerformed;
    public event Action OnMovementCancelled;
    public event Action OnJumpPerformed;
    public event Action OnJumpCancelled;
    public event Action OnCrouchPerformed;
    public event Action OnLaydownPerformed;
    public event Action OnLaydownRotateLPerformed;
    public event Action OnLaydownRotateRPerformed;
    public event Action OnLaydownCancelled;
    public event Action OnClickAnyKey;

    private InputSystem_Actions _inputActions;

    private bool _isLaydownPerformed;
    private Coroutine _waitForCrouchCoroutine;

    public bool IsSprintPerformed => _inputActions.Player.Sprint.IsPressed();
    public Vector2 MovementDirection => GameIsPaused() ? Vector2.zero : _inputActions.Player.Move.ReadValue<Vector2>();
    public Vector2 MouseDelta => GameIsPaused() ? Vector2.zero : _inputActions.Player.Look.ReadValue<Vector2>();
    public bool IsMovementPerformed { get; private set; }
    public bool IsRunning { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        instance = this;
        _inputActions = new InputSystem_Actions();
        _inputActions.Enable();
        
        _inputActions.Player.Move.performed += PerformMove;
        _inputActions.Player.Move.canceled += CancelMove;
        
        _inputActions.Player.Sprint.performed += PerformSprint;
        _inputActions.Player.Sprint.canceled += CancelSprint;
        
        _inputActions.Player.Jump.performed += PerformJump;
        _inputActions.Player.Jump.canceled += CancelJump;
        
        _inputActions.Player.Crouch.performed += PerformCrouch;
        _inputActions.Player.Crouch.canceled += CancelCrouch; 
        
        // _inputActions.Player.R.performed += PerformedR;
        
        _inputActions.Player.Interact.performed += PerformedE;
        _inputActions.Player.Interact.canceled += CanceledE;
        
        // _inputActions.Player.Action.performed += LeftMouseButtonPerformed;
        // _inputActions.Player.Action.canceled += LeftMouseButtonCanceled;
        
        // _inputActions.UI.Back.performed += PerformedEscape;
        // _inputActions.UI.AnyKey.performed += PerformedAnyKey;
        
        _inputActions.Player.Drop.performed += PerformedDrop;
        _inputActions.Player.Drop.canceled += CanceledDrop;
        
        // _inputActions.Player.LaydownRotateL.performed += PerformLaydownRotateL;
        // _inputActions.Player.LaydownRotateR.performed += PerformLaydownRotateR;
        
        // GameStateController.Instance.OnPauseChanged += OnChangePauseState;
    }

    private void OnDestroy()
    {
        _inputActions.Player.Move.performed -= PerformMove;
        _inputActions.Player.Move.canceled -= CancelMove;
        
        _inputActions.Player.Sprint.performed -= PerformSprint;
        _inputActions.Player.Sprint.canceled -= CancelSprint;
        
        _inputActions.Player.Jump.performed -= PerformJump;
        _inputActions.Player.Jump.canceled -= CancelJump;
        
        _inputActions.Player.Crouch.performed -= PerformCrouch;
        _inputActions.Player.Crouch.canceled -= CancelCrouch;
        
        // _inputActions.Player.R.performed -= PerformedR;
        
        _inputActions.Player.Interact.performed -= PerformedE;
        _inputActions.Player.Interact.canceled -= CanceledE;
        
        // _inputActions.Player.Action.performed -= LeftMouseButtonPerformed;
        // _inputActions.Player.Action.canceled -= LeftMouseButtonCanceled;
        //
        // _inputActions.UI.Back.performed -= PerformedEscape;
        // _inputActions.UI.AnyKey.performed -= PerformedAnyKey;
        
        _inputActions.Player.Drop.performed -= PerformedDrop;
        _inputActions.Player.Drop.canceled -= CanceledDrop;
        
        // _inputActions.Player.LaydownRotateL.performed -= PerformLaydownRotateL;
        // _inputActions.Player.LaydownRotateR.performed -= PerformLaydownRotateR;
        
        // GameStateController.Instance.OnPauseChanged -= OnChangePauseState;
        
        _inputActions.Disable();
    }

    private void OnChangePauseState(bool state)
    {
        if (IsMovementPerformed && state)
        {
            IsMovementPerformed = false;
            OnMovementCancelled?.Invoke();
        }
    }
    
    private void PerformMove(CallbackContext context)
    {
        if (GameIsPaused())
            return;
        
        IsMovementPerformed = true;
        OnMovementPerformed?.Invoke();
    }
    
    private void CancelMove(CallbackContext context)
    {
        if (GameIsPaused())
            return;
        
        IsMovementPerformed = false;
        OnMovementCancelled?.Invoke();
    }
    
    private void PerformSprint(CallbackContext context)
    {
        if (GameIsPaused())
            return;
        
        IsRunning = true;
        OnSprintPerformed?.Invoke();
    }
    
    private void CancelSprint(CallbackContext context)
    {
        if (GameIsPaused())
            return;
        
        IsRunning = false;
        OnSprintCancelled?.Invoke();
    }
    
    private void PerformJump(CallbackContext context)
    {
        if (GameIsPaused())
            return;
        
        OnJumpPerformed?.Invoke();
    }
    
    private void CancelJump(CallbackContext context)
    {
        if (GameIsPaused())
            return;
        
        OnJumpCancelled?.Invoke();
    }
    
    private void PerformCrouch(CallbackContext context)
    {
        if (GameIsPaused())
            return;
        
        _waitForCrouchCoroutine = StartCoroutine(WaitForHoldingCrouch());
    }
    
    private void CancelCrouch(CallbackContext context)
    {
        if (GameIsPaused())
            return;
        
        if (_waitForCrouchCoroutine != null)
            StopCoroutine(_waitForCrouchCoroutine);

        if (_isLaydownPerformed)
        {
            _isLaydownPerformed = false;
            return;
        }
        
        OnCrouchPerformed?.Invoke();
    }

    private IEnumerator WaitForHoldingCrouch()
    {
        yield return new WaitForSeconds(0.5f);

        // if (_inputActions.Player.Crouch.IsPressed())
        // {
        //     PerformLaydown();
        // }
    }
    
    private void PerformLaydown()
    {
        if (GameIsPaused())
            return;
        
        _isLaydownPerformed = true;
        OnLaydownPerformed?.Invoke();
    }
    
    private void CancelLaydown()
    {
        OnLaydownCancelled?.Invoke();
    }
    
    private void PerformedR(CallbackContext callbackContext)
    {
        if (GameIsPaused())
            return;
        
        OnPerformedR?.Invoke();
    }

    private void LeftMouseButtonPerformed(CallbackContext callbackContext)
    {
        if (GameIsPaused())
            return;
        
        OnLeftMouseButtonPerformed?.Invoke();
    }
    
    private void PerformedEscape(CallbackContext callbackContext)
    {
        OnPerformedEscape?.Invoke();
    }

    private void LeftMouseButtonCanceled(CallbackContext callbackContext)
    {
        if (GameIsPaused())
            return;
        
        OnLeftMouseButtonCanceled?.Invoke();
    }

    private void PerformedE(CallbackContext callbackContext)
    {
        if (GameIsPaused())
            return;
        
        OnPerformedGrab?.Invoke();
    }
    
    private void CanceledE(CallbackContext callbackContext)
    {
        if (GameIsPaused())
            return;
        
        OnCanceledGrab?.Invoke();
    }

    private void PerformedDrop(CallbackContext callbackContext)
    {
        if (GameIsPaused())
            return;
        
        OnPerformedDrop?.Invoke();
    }

    private void CanceledDrop(CallbackContext callbackContext)
    {
        if (GameIsPaused())
            return;
        
        OnCanceledDrop?.Invoke();
    }
    
    private void PerformLaydownRotateL(CallbackContext context)
    {
        if (GameIsPaused())
            return;
        
        OnLaydownRotateLPerformed?.Invoke();
    }
    
    private void PerformLaydownRotateR(CallbackContext context)
    {
        if (GameIsPaused())
            return;
        
        OnLaydownRotateRPerformed?.Invoke();
    }
    
    private void PerformedAnyKey(CallbackContext callbackContext)
    {
        OnClickAnyKey?.Invoke();
    }

    private bool GameIsPaused()
    {
        return false;
    }
}
