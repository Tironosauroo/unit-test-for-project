/// @file PlayerMovement.cs
/// @brief This file contains the PlayerMovement class, which handles player locomotion, jumping, and crouching in the Unity game.
///        It uses Unity's Input System for controls and integrates with CharacterController for physics-based movement.
/// @details The PlayerMovement component manages movement speed, gravity, jumping mechanics, and camera adjustments during crouching.
///          It subscribes to input actions for move, jump, and crouch, and applies transformations accordingly.
///          Ensures grounded checks for jumping and smooth transitions for crouching.

using UnityEngine;
using UnityEngine.InputSystem; // новий інпут

/// @brief Component for controlling player movement, jumping, and crouching.
/// @details This MonoBehaviour class requires a CharacterController and uses PlayerControls for input.
///          It handles horizontal movement, vertical velocity with gravity, jumping, and camera height adjustments for crouching.
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    /// @brief The normal movement speed of the player.
    [Header("Movement")]
    public float speed = 6f;

    /// @brief The movement speed while crouching.
    public float crouchSpeed = 3f;

    /// @brief The gravity acceleration applied to the player.
    public float gravity = -9.81f;

    /// @brief The height of the jump.
    public float jumpHeight = 1.5f;

    /// @brief The transform of the camera for crouching adjustments.
    [Header("Camera")]
    public Transform cameraTransform;

    /// @brief The amount to lower the camera during crouching.
    public float crouchHeight = 0.8f; // downing

    private float normalCameraY;
    private bool isCrouching = false;

    private CharacterController controller;
    private Vector3 velocity;

    private PlayerControls controls; // class form new input system .inputactions
    private Vector2 moveInput;

    /// @brief Initializes the input controls and subscribes to movement, jump, and crouch events.
    /// @details Creates a PlayerControls instance and sets up callbacks for performed and canceled actions.
    void Awake()
    {
        controls = new PlayerControls();

        // subscribing on input
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => Jump();
        controls.Player.Crouch.performed += ctx => StartCrouch();
        controls.Player.Crouch.canceled += ctx => StopCrouch();
    }

    /// @brief Enables the player controls.
    void OnEnable() => controls.Player.Enable();

    /// @brief Disables the player controls.
    void OnDisable() => controls.Player.Disable();

    /// @brief Initializes the CharacterController and stores the normal camera height.
    void Start()
    {
        controller = GetComponent<CharacterController>();
        normalCameraY = cameraTransform.localPosition.y;
    }

    /// @brief Updates movement and applies gravity each frame.
    /// @details Calculates movement direction based on input, applies speed (considering crouching), and handles vertical velocity with gravity.
    void Update()
    {
        // movement
        float currentSpeed = isCrouching ? crouchSpeed : speed;
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // gravity (no ragdolls for new input sys)
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    /// @brief Performs a jump if the player is grounded.
    /// @details Calculates the initial upward velocity based on jump height and gravity.
    void Jump()
    {
        if (controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    /// @brief Starts crouching by lowering the camera and setting the crouching flag.
    /// @details Adjusts the camera's local position downward by crouchHeight and sets isCrouching to true.
    void StartCrouch()
    {
        isCrouching = true;
        Vector3 pos = cameraTransform.localPosition;
        pos.y = normalCameraY - crouchHeight;
        cameraTransform.localPosition = pos;
    }

    /// @brief Stops crouching by resetting the camera height and clearing the crouching flag.
    /// @details Resets the camera's local position to normalCameraY and sets isCrouching to false.
    void StopCrouch()
    {
        isCrouching = false;
        Vector3 pos = cameraTransform.localPosition;
        pos.y = normalCameraY;
        cameraTransform.localPosition = pos;
    }
}
