/// @file PlayerMovementTest_fixed.cs
/// @brief This file contains unit tests for the PlayerMovement class in a Unity game environment.
///        It uses NUnit and Unity Test Tools to test player movement and crouching functionalities.
///        The tests ensure proper handling of input-based movement and camera adjustments during crouching.
/// @details The PlayerMovement class is assumed to manage player locomotion and actions like crouching in the game.
///          Tests cover forward movement based on input, crouching mechanics, and camera position resets.
///          All tests use reflection to access private fields and methods, and coroutines for frame-based simulations.

using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using System.Collections;
using System.Reflection;

/// @brief Test class for PlayerMovement functionality.
/// @details This class provides unit tests to validate the PlayerMovement's methods for handling movement and crouching.
///          It includes setup and teardown to create and destroy test objects, ensuring clean test environments.
///          Tests focus on input-driven movement, camera positioning, and method invocations via reflection.
public class PlayerMovementTest
{
    private GameObject player;
    private PlayerMovement movement;
    private CharacterController controller;
    private GameObject cameraObj;

    /// @brief Sets up the test environment before each test.
    /// @details Creates a PlayerMovement component, CharacterController, camera, and initializes their states.
    ///          Sends Awake and Start messages, and invokes OnEnable if available to simulate component lifecycle.
    /// @throws None (setup failures would be handled by NUnit; reflection errors caught by Assert.Fail).
    [SetUp]
    public void Setup()
    {
        player = new GameObject("Player");
        controller = player.AddComponent<CharacterController>();

        cameraObj = new GameObject("Camera");
        cameraObj.transform.SetParent(player.transform);

        movement = player.AddComponent<PlayerMovement>();
        movement.cameraTransform = cameraObj.transform;

        controller.enabled = true;

        player.SendMessage("Awake");
        player.SendMessage("Start");

        var onEnable = movement.GetType().GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (onEnable != null) onEnable.Invoke(movement, null);
        else movement.enabled = true;
    }

    /// @brief Tears down the test environment after each test.
    /// @details Destroys the player GameObject to prevent memory leaks and ensure test isolation.
    /// @throws None (teardown failures would be handled by NUnit).
    [TearDown]
    public void Teardown()
    {
        if (player != null) Object.DestroyImmediate(player);
    }

    /// @brief Tests that the player moves forward when input is given.
    /// @details Simulates input by setting the moveInput field and invokes Update over several frames,
    ///          verifying that the player's position changes in the forward direction.
    /// @throws None (test assertions handle failures; reflection errors caught by Assert.Fail).
    [UnityTest]
    public IEnumerator Player_Moves_Forward_When_Input_Given()
    {
        Assert.IsNotNull(movement, "Movement component should be present.");
        Assert.IsTrue(movement.enabled, "Movement component should be enabled.");

        Vector3 startPos = player.transform.position;

        var moveInputField = movement.GetType()
            .GetField("moveInput", BindingFlags.NonPublic | BindingFlags.Instance);
        if (moveInputField == null) Assert.Fail("PlayerMovement: private field 'moveInput' not found.");
        moveInputField.SetValue(movement, new Vector2(0, 1));

        int framesToSimulate = 5;
        for (int i = 0; i < framesToSimulate; i++)
        {
            var updateMethod = movement.GetType().GetMethod("Update", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (updateMethod != null)
            {
                updateMethod.Invoke(movement, null);
            }
            else
            {
                movement.SendMessage("Update", SendMessageOptions.DontRequireReceiver);
            }
            yield return null;
        }

        float epsilon = 0.01f;
        Assert.Greater(player.transform.position.z, startPos.z + epsilon, "Player should move forward when input is positive on Y axis.");
    }

    /// @brief Tests that the player crouches and returns to normal height.
    /// @details Invokes StartCrouch to lower the camera, then StopCrouch to reset it,
    ///          verifying camera position changes and resets correctly.
    /// @throws None (test assertions handle failures; reflection errors caught by Assert.Fail).
    [UnityTest]
    public IEnumerator Player_Crouches_And_Returns_To_Normal()
    {
        float normalY = movement.cameraTransform.localPosition.y;

        var startCrouch = movement.GetType().GetMethod("StartCrouch", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        if (startCrouch == null) Assert.Fail("PlayerMovement: StartCrouch method not found.");
        startCrouch.Invoke(movement, null);

        yield return null;
        float crouchedY = movement.cameraTransform.localPosition.y;

        Assert.Less(crouchedY, normalY, "camera should be a bit down");

        var stopCrouch = movement.GetType().GetMethod("StopCrouch", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        if (stopCrouch == null) Assert.Fail("PlayerMovement: StopCrouch method not found.");
        stopCrouch.Invoke(movement, null);

        yield return null;
        float resetY = movement.cameraTransform.localPosition.y;

        Assert.AreEqual(normalY, resetY, 0.01f, "camera should be on normal position");
    }
}
