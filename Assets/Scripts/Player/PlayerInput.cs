using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput
{
    private readonly InputSystem_Actions inputActions;

    private bool isEnabled;
    private bool isDisposed;

    public PlayerInput()
    {
        inputActions = new InputSystem_Actions();
        Enable();
    }

    public Vector2 MoveInput =>
        inputActions.Player.Move.ReadValue<Vector2>();

    public Vector2 PointerScreenPosition =>
        Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : Vector2.zero;

    public bool PausePressed =>
        inputActions.Player.Pause.WasPressedThisFrame();

    public void Enable()
    {
        if (isDisposed || isEnabled)
        {
            return;
        }

        inputActions.Player.Enable();
        isEnabled = true;
    }

    public void Disable()
    {
        if (isDisposed || !isEnabled)
        {
            return;
        }

        inputActions.Player.Disable();
        isEnabled = false;
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        Disable();
        inputActions.Dispose();

        isDisposed = true;
    }
}