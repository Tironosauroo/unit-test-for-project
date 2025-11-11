/// @file PlayerLook.cs
/// @brief This file contains the PlayerLook class, which handles camera rotation based on mouse input in the Unity game.
///        It uses Unity's Input System to control player look direction and clamps vertical rotation.
/// @details The PlayerLook component manages mouse sensitivity, input reading, and applies rotations to the camera and player transform.
///          It ensures smooth look controls with clamped pitch to prevent over-rotation.


using UnityEngine;
using UnityEngine.InputSystem;

/// @brief Component for controlling player camera look direction.
/// @details This MonoBehaviour class reads mouse input via PlayerControls, applies sensitivity and time scaling,
///          and rotates the camera vertically and the player horizontally. It clamps vertical rotation to prevent flipping.
public class PlayerLook : MonoBehaviour
{
    /// @brief The sensitivity multiplier for mouse movement.
    public float mouseSensitivity = 100f;

    /// @brief The transform of the camera to rotate for vertical look.
    public Transform cameraTransform;

    private float xRotation = 0f;

    private PlayerControls controls;
    private Vector2 lookInput;

    /// @brief Initializes the input controls and subscribes to look input events.
    /// @details Creates a PlayerControls instance and sets up callbacks for look performed and canceled.
    void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    /// @brief Enables the player controls.
    void OnEnable() => controls.Player.Enable();

    /// @brief Disables the player controls.
    void OnDisable() => controls.Player.Disable();

    /// @brief Locks the cursor at the start of the game.
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// @brief Updates the camera and player rotation based on look input.
    /// @details Calculates mouse movement deltas, clamps vertical rotation, and applies rotations in LateUpdate for smooth rendering.
    // update is called once per frame
    void LateUpdate()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
