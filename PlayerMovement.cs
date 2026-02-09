using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Read movement input from Gamepad or Keyboard (WASD / Arrow keys)
        Vector2 moveInput = Vector2.zero;
        if (Gamepad.current != null)
        {
            moveInput = Gamepad.current.leftStick.ReadValue();
        }

        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            float v = 0f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) v += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) v -= 1f;
            float h = 0f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) h += 1f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) h -= 1f;
            // If gamepad already provided input, blend; otherwise use keyboard input
            if (moveInput == Vector2.zero) moveInput = new Vector2(h, v);
            else moveInput = Vector2.ClampMagnitude(moveInput + new Vector2(h, v), 1f);
        }

        bool isRunning = false;
        if (keyboard != null) isRunning = keyboard.leftShiftKey.isPressed;
        if (!isRunning && Gamepad.current != null) isRunning = Gamepad.current.leftShoulder.isPressed;

        float speed = canMove ? (isRunning ? runSpeed : walkSpeed) : 0f;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * (moveInput.y * speed)) + (right * (moveInput.x * speed));

        // Jump (space or gamepad South button)
        bool jumpPressed = false;
        if (keyboard != null) jumpPressed = keyboard.spaceKey.wasPressedThisFrame;
        if (!jumpPressed && Gamepad.current != null) jumpPressed = Gamepad.current.buttonSouth.wasPressedThisFrame;

        if (jumpPressed && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Crouch (R or gamepad right shoulder)
        bool crouch = false;
        if (keyboard != null) crouch = keyboard.rKey.isPressed;
        if (!crouch && Gamepad.current != null) crouch = Gamepad.current.rightShoulder.isPressed;

        if (crouch && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 6f;
            runSpeed = 12f;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        // Look (mouse delta or gamepad right stick)
        if (canMove)
        {
            Vector2 lookDelta = Vector2.zero;
            if (Mouse.current != null)
            {
                lookDelta = Mouse.current.delta.ReadValue();
            }
            if (Gamepad.current != null)
            {
                var rs = Gamepad.current.rightStick.ReadValue();
                // Right stick values are already small, blend with mouse
                lookDelta += rs * 100f; // scale so gamepad feels similar
            }

            float mx = lookDelta.x * lookSpeed * 0.02f;
            float my = lookDelta.y * lookSpeed * 0.02f;

            rotationX += -my;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            if (playerCamera != null) playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, mx, 0);
        }
    }
}
