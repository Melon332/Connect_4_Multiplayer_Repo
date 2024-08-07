using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager
{
    private const string MouseActionName = "MousePosition";
    private const string MouseLeftClickActionName = "MouseLeftClick";
    private const string PauseButtonActionName = "PauseButton";
    private const string ToggleChatBoxActionName = "ToggleChatBox";
    
    public Vector2 mousePos { private set; get; }
    
    public Action OnLeftMouseClickAction;
    public Action OnPauseButtonPressedAction;
    public Action OnToggleChatBoxButtonPressedAction;
    public Action<Vector2> OnMouseMoveAction;
    
    private PlayerInput playerInput;

    public InputManager(PlayerInput _playerInput)
    {
        playerInput = _playerInput;
    }

    public void EnableInput()
    {
        playerInput.actions.Enable();
        playerInput.actions.FindAction(MouseActionName).performed += OnMouseMove;
        playerInput.actions.FindAction(MouseLeftClickActionName).performed += OnLeftMouseClick;
        playerInput.actions.FindAction(PauseButtonActionName).performed += OnPauseButtonPressed;
        playerInput.actions.FindAction(ToggleChatBoxActionName).performed += OnToggleChatBoxButtonPressed;
    }
    


    public void DisableInput()
    {
        playerInput.actions.FindAction(MouseActionName).performed -= OnMouseMove;
        playerInput.actions.FindAction(MouseLeftClickActionName).performed -= OnLeftMouseClick;
        playerInput.actions.FindAction(PauseButtonActionName).performed -= OnPauseButtonPressed;
        playerInput.actions.Disable();
    }
    
    private void OnToggleChatBoxButtonPressed(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValueAsButton())
        {
            OnToggleChatBoxButtonPressedAction?.Invoke();
        }
    }
    
    private void OnLeftMouseClick(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValueAsButton())
        {
            OnLeftMouseClickAction?.Invoke();
        }
    }
    
    private void OnPauseButtonPressed(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValueAsButton())
        {
            OnPauseButtonPressedAction?.Invoke();
        }
    }

    private void OnMouseMove(InputAction.CallbackContext ctx)
    {
        mousePos = ctx.ReadValue<Vector2>();
        OnMouseMoveAction?.Invoke(mousePos);
    }
}
