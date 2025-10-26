using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using System.Collections;
using System.Reflection;

public class PlayerMovementTest
{
    private GameObject player;
    private PlayerMovement movement;
    private CharacterController controller;
    private GameObject cameraObj;

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


    [TearDown]
    public void Teardown()
    {
        if (player != null) Object.DestroyImmediate(player);
    }

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
