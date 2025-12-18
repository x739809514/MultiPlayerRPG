using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Custom/Player/Input Reader")]
public class InputReader: ScriptableObject, InputReader_Action.IPlayerActions
{
    public event UnityAction<Vector2> MoveEvent; 
    public event UnityAction AttackEvent;
    public event UnityAction AttackCancleEvent;
    public event UnityAction InteractEvent;
    public event UnityAction JumpEvent;
    
    // UI
    public event UnityAction<Vector2> NavigateEvent;
    public event UnityAction<Vector2> ScrollWheelEvent;
    public event UnityAction SubmitEvent;
    public event UnityAction CancleEvent;
    public event UnityAction ClickEvent;

    private InputReader_Action inputSystemActions;
    
    private void OnEnable()
    {
        if (inputSystemActions==null)
        {
            inputSystemActions = new InputReader_Action();
            inputSystemActions.Player.SetCallbacks(this);
        }
    }

    private void OnDisable()
    {
        inputSystemActions.Player.Disable();
    }
    
    // 切换到玩家控制
    public void EnablePlayerInput()
    {
        inputSystemActions.Player.Enable();
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            JumpEvent?.Invoke();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            AttackEvent?.Invoke();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            InteractEvent?.Invoke();
        }
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        NavigateEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            SubmitEvent?.Invoke();
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            CancleEvent?.Invoke();
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            ClickEvent?.Invoke();
        }
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        ScrollWheelEvent?.Invoke(context.ReadValue<Vector2>());
    }
}