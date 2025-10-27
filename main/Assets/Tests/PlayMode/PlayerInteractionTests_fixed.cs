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
        player = new GameObject("Player");
        interaction = player.AddComponent<PlayerInteraction>();

        hud = new GameObject("HUD");
        hud.SetActive(false);

        var hudField = typeof(PlayerInteraction)
            .GetField("hud", BindingFlags.NonPublic | BindingFlags.Instance);
        if (hudField == null)
            Assert.Fail("PlayerInteraction: expected private field 'hud' not found.");
        hudField.SetValue(interaction, hud);

        pickable = new GameObject("Pickable");
        pickable.tag = "Pickable";
        var box = pickable.AddComponent<BoxCollider>();
        box.isTrigger = true;

        var rb = pickable.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        var onEnableMethod = typeof(PlayerInteraction).GetMethod("OnEnable", BindingFlags.NonPublic | BindingFlags.Instance);
        if (onEnableMethod != null)
        {
            onEnableMethod.Invoke(interaction, null);
        }
        else
        {
            interaction.enabled = true;
        }
    }

    [TearDown]
    public void Teardown()
    {
        if (player != null) Object.DestroyImmediate(player);
        if (hud != null) Object.DestroyImmediate(hud);
        if (pickable != null) Object.DestroyImmediate(pickable);
    }

    [UnityTest]
    public IEnumerator Hud_Activates_OnTriggerEnter_And_Deactivates_OnTriggerExit()
    {
        pickable.AddComponent<PickableItem>();
        var collider = pickable.GetComponent<Collider>();

        Assert.IsFalse(hud.activeInHierarchy, "HUD should be inactive at start.");

        var onTriggerEnter = typeof(PlayerInteraction).GetMethod("OnTriggerEnter", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        if (onTriggerEnter == null) Assert.Fail("PlayerInteraction: OnTriggerEnter method not found.");
        onTriggerEnter.Invoke(interaction, new object[] { collider });
        yield return null;

        Assert.IsTrue(hud.activeInHierarchy, "HUD should activate when entering pickable trigger.");

        var onTriggerExit = typeof(PlayerInteraction).GetMethod("OnTriggerExit", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        if (onTriggerExit == null) Assert.Fail("PlayerInteraction: OnTriggerExit method not found.");
        onTriggerExit.Invoke(interaction, new object[] { collider });
        yield return null;

        Assert.IsFalse(hud.activeInHierarchy, "HUD should deactivate when exiting pickable trigger.");
    }

    [UnityTest]
    public IEnumerator Interact_HidesHud_And_DisablesPickable()
    {
        var tex = new Texture2D(4, 4);
        var sprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), Vector2.zero);

        var pickableItem = pickable.AddComponent<PickableItem>();
        pickableItem.itemSprite = sprite;

        var collider = pickable.GetComponent<Collider>();

        var onTriggerEnter = typeof(PlayerInteraction).GetMethod("OnTriggerEnter", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        if (onTriggerEnter == null) Assert.Fail("PlayerInteraction: OnTriggerEnter method not found.");
        onTriggerEnter.Invoke(interaction, new object[] { collider });
        yield return null;

        Assert.IsTrue(hud.activeInHierarchy, "HUD should be active before interact.");

        var interactMethod = typeof(PlayerInteraction)
            .GetMethod("Interact", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        if (interactMethod == null) Assert.Fail("PlayerInteraction: Interact method not found.");

        Assert.DoesNotThrow(() => interactMethod.Invoke(interaction, null), "Interact invocation should not throw.");
        yield return null;

        Assert.IsFalse(hud.activeInHierarchy, "HUD should hide after interaction.");
        Assert.IsFalse(pickable.activeSelf, "Pickable should be disabled after picking.");
    }

    [UnityTest]
    public IEnumerator Interact_DoesNotThrow_AndHidesItem_When_NoInventory()
    {
        var inventoryField = typeof(PlayerInteraction).GetField("inventory", BindingFlags.NonPublic | BindingFlags.Instance);
        if (inventoryField == null) Assert.Fail("PlayerInteraction: expected private field 'inventory' not found.");
        inventoryField.SetValue(interaction, null);

        var pickableItem = pickable.AddComponent<PickableItem>();
        var tex = new Texture2D(4, 4);
        var sprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), Vector2.zero);
        pickableItem.itemSprite = sprite;

        var currentItemField = typeof(PlayerInteraction)
            .GetField("currentPickable", BindingFlags.NonPublic | BindingFlags.Instance);
        if (currentItemField == null) Assert.Fail("PlayerInteraction: expected private field 'currentPickable' not found.");
        currentItemField.SetValue(interaction, pickable);

        var interactMethod = typeof(PlayerInteraction)
            .GetMethod("Interact", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        if (interactMethod == null) Assert.Fail("PlayerInteraction: Interact method not found.");

        Assert.DoesNotThrow(() => interactMethod.Invoke(interaction, null), "Interact should not throw even when inventory is null.");
        yield return null;

        Assert.IsFalse(pickable.activeSelf, "Pickable should be disabled even with no inventory.");
    }
}
