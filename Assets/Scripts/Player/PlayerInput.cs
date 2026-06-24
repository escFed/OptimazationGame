using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput 
{
    private InputSystem_Actions inputActions;

    public PlayerInput()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
    }

    public Vector2 MoveInput => inputActions.Player.Move.ReadValue<Vector2>();
    public Vector2 PointerScreenPosition => Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
}
