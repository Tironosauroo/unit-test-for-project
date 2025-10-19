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
        // --- ВИПРАВЛЕННЯ 2.1: Додаємо PickableItem, щоб OnTriggerEnter спрацював ---
        // (Припускаю, що ваш OnTriggerEnter шукає цей компонент)
        pickable.AddComponent<PickableItem>();
        var collider = pickable.GetComponent<Collider>();

        // HUD unactive by default
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

        // --- ВИПРАВЛЕННЯ 1: Texture2D.blackTexture - 4x4, а не 10x10 ---
        var sprite = Sprite.Create(Texture2D.blackTexture, new Rect(0, 0, 4, 4), Vector2.zero);

        var pickableItem = pickable.AddComponent<PickableItem>();
        pickableItem.itemSprite = sprite;

        var collider = pickable.GetComponent<Collider>();
        interaction.SendMessage("OnTriggerEnter", collider);
        yield return null;

        // activating HUD
        Assert.IsTrue(hud.activeSelf, "HUD should be active before interact.");

        // calling Interact hrough reflection
        var interactMethod = typeof(PlayerInteraction)
            .GetMethod("Interact", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        interactMethod.Invoke(interaction, null);
        yield return null;

        // HUD and item are inactive
        Assert.IsFalse(hud.activeSelf, "HUD should hide after interaction.");
        Assert.IsFalse(pickable.activeSelf, "Pickable should be disabled after picking.");
    }

    // Це буде вже інший тест, який перевіряє "безпечну" поведінку
    [UnityTest]
    public IEnumerator Interact_DoesNotThrow_AndHidesItem_When_NoInventory()
    {
        // arrange
        // hud залишаємо, inventory видаляємо
        typeof(PlayerInteraction)
            .GetField("inventory", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(interaction, null);

        var pickableItem = pickable.AddComponent<PickableItem>();
        var sprite = Sprite.Create(Texture2D.blackTexture, new Rect(0, 0, 4, 4), Vector2.zero);
        pickableItem.itemSprite = sprite;

        // Встановлюємо currentPickable через рефлексію
        var currentItemField = typeof(PlayerInteraction)
            .GetField("currentPickable", BindingFlags.NonPublic | BindingFlags.Instance);
        currentItemField.SetValue(interaction, pickable);

        // act
        var interactMethod = typeof(PlayerInteraction)
            .GetMethod("Interact", BindingFlags.NonPublic | BindingFlags.Instance);

        // assert
        // Перевіряємо, що винятку НЕ було
        Assert.DoesNotThrow(() => interactMethod.Invoke(interaction, null));
        yield return null;

        // Перевіряємо, що об'єкт все одно зник
        Assert.IsFalse(pickable.activeSelf, "Pickable should be disabled even with no inventory.");
    }
}