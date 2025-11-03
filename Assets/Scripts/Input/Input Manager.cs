using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.MovementActions movement;
    private PlayerController playerController;
    private PlayerLook playerLook;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerLook = GetComponent<PlayerLook>();
        playerInput = new PlayerInput();
        movement = playerInput.Movement;
        movement.Jump.performed += ctx => playerController.Jump();
        movement.Sprint.performed += ctx => playerController.Sprint();
        movement.Crouch.performed += ctx => playerController.Crouch();
    }

    // Update is called once per frame

    private void FixedUpdate()
    {
        playerController.ProcessMove(movement.Walk.ReadValue<Vector2>());
    }

    private void LateUpdate()
    {
        playerLook.ProcessLook(movement.Look.ReadValue<Vector2>());
    }
    void OnEnable()
    {
        movement.Enable();
    }

    void OnDisable()
    {
        movement.Disable();
    }
}
