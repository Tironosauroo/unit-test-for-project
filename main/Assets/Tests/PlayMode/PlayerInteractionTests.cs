using NUnit.Framework;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerInteractionTests
{
    private GameObject player;
    private PlayerInteraction interaction;
    private GameObject hud;
    private GameObject pickable;

    [SetUp]
    public void Setup()
    {
        // creating player
        player = new GameObject("Player");
        interaction = player.AddComponent<PlayerInteraction>();

        // creating HUD
        hud = new GameObject("HUD");
        typeof(PlayerInteraction)
            .GetField("hud", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(interaction, hud);

        // creating item
        pickable = new GameObject("Pickable");
        pickable.tag = "Pickable";
        pickable.AddComponent<BoxCollider>();

        // emulation OnEnable
        player.SendMessage("OnEnable");
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(player);
        Object.DestroyImmediate(hud);
        Object.DestroyImmediate(pickable);
    }

    [UnityTest]
    public IEnumerator Hud_Activates_OnTriggerEnter_And_Deactivates_OnTriggerExit()
    {
        // adding PickableItem
        pickable.AddComponent<PickableItem>();
        var collider = pickable.GetComponent<Collider>();

        // HUD inactive by default
        Assert.IsFalse(hud.activeSelf, "HUD should be inactive at start.");

        // player enters a trigger
        interaction.SendMessage("OnTriggerEnter", collider);
        yield return null;

        Assert.IsTrue(hud.activeSelf, "HUD should activate when entering pickable trigger.");

        // player exits a trigger
        interaction.SendMessage("OnTriggerExit", collider);
        yield return null;

        Assert.IsFalse(hud.activeSelf, "HUD should deactivate when exiting pickable trigger.");
    }

    [UnityTest]
    public IEnumerator Interact_HidesHud_And_DisablesPickable()
    {
        // add test PickableItem from Sprite

        // 4x4 sprite
        var sprite = Sprite.Create(Texture2D.blackTexture, new Rect(0, 0, 4, 4), Vector2.zero);

        var pickableItem = pickable.AddComponent<PickableItem>();
        pickableItem.itemSprite = sprite;

        var collider = pickable.GetComponent<Collider>();
        interaction.SendMessage("OnTriggerEnter", collider);
        yield return null;

        // activating HUD
        Assert.IsTrue(hud.activeSelf, "HUD should be active before interact.");

        // calling Interact through reflection
        var interactMethod = typeof(PlayerInteraction)
            .GetMethod("Interact", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        interactMethod.Invoke(interaction, null);
        yield return null;

        // HUD and item are inactive
        Assert.IsFalse(hud.activeSelf, "HUD should hide after interaction.");
        Assert.IsFalse(pickable.activeSelf, "Pickable should be disabled after picking.");
    }

    [UnityTest]
    public IEnumerator Interact_DoesNotThrow_AndHidesItem_When_NoInventory()
    {
        // arrange
        typeof(PlayerInteraction)
            .GetField("inventory", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(interaction, null);

        var pickableItem = pickable.AddComponent<PickableItem>();
        var sprite = Sprite.Create(Texture2D.blackTexture, new Rect(0, 0, 4, 4), Vector2.zero);
        pickableItem.itemSprite = sprite;

        // setting currentPickable through reflection
        var currentItemField = typeof(PlayerInteraction)
            .GetField("currentPickable", BindingFlags.NonPublic | BindingFlags.Instance);
        currentItemField.SetValue(interaction, pickable);

        // act
        var interactMethod = typeof(PlayerInteraction)
            .GetMethod("Interact", BindingFlags.NonPublic | BindingFlags.Instance);

        // assert
        // no exceptions
        Assert.DoesNotThrow(() => interactMethod.Invoke(interaction, null));
        yield return null;

        // disabled test
        Assert.IsFalse(pickable.activeSelf, "Pickable should be disabled even with no inventory.");
    }
}