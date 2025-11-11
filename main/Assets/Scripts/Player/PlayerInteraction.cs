/// @file PlayerInteraction.cs
/// @brief This file contains the PlayerInteraction class, which handles player interactions with pickable items in the Unity game.
///        It manages HUD activation, input controls, and item collection into the inventory.
/// @details The PlayerInteraction component uses Unity's Input System to detect interaction inputs.
///          It activates HUD when near pickable items, allows picking them up, and integrates with the Inventory system.
///          Ensures proper state management for current pickable items and HUD visibility.

using UnityEngine;
using UnityEngine.InputSystem;

/// @brief Component for managing player interactions with pickable items.
/// @details This MonoBehaviour class handles trigger events for pickable objects, input for interactions,
///          and coordinates with the Inventory to add collected items. It controls HUD visibility and item instantiation.
public class PlayerInteraction : MonoBehaviour
{
    /// @brief The HUD GameObject displayed when near pickable items.
    [Header("HUD for Pickable Items")]
    [SerializeField] private GameObject hud;  // HUD inside Canvas

    private PlayerControls controls;
    private GameObject currentPickable;
    private Inventory inventory;

    /// @brief Initializes the input controls and subscribes to interaction events.
    /// @details Creates a new PlayerControls instance, sets up event handlers for Interact and NextItemQueue actions,
    ///          and retrieves the Inventory component.
    private void Awake()
    {
        controls = new PlayerControls();

        // subscribe on events
        controls.Player.Interact.started += ctx => Interact();

        inventory = GetComponent<Inventory>();
        controls.Player.NextItemQueue.started += ctx => inventory?.NextItem();
    }

    /// @brief Enables input controls and disables HUD by default.
    /// @details Activates the PlayerControls and ensures the HUD is hidden initially.
    private void OnEnable()
    {
        controls.Enable();
        if (hud != null)
            hud.SetActive(false); // default disabled HUD cursor
    }

    /// @brief Disables input controls.
    /// @details Deactivates the PlayerControls when the component is disabled.
    private void OnDisable()
    {
        controls.Disable();
    }

    /// @brief Handles entering a trigger collider with a pickable item.
    /// @param other The Collider of the object entered.
    /// @details Checks if the collider is tagged "Pickable", sets it as current, and activates the HUD.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickable"))
        {
            currentPickable = other.gameObject;
            if (hud != null)
                hud.SetActive(true);
        }
    }

    /// @brief Handles exiting a trigger collider with a pickable item.
    /// @param other The Collider of the object exited.
    /// @details If exiting the current pickable, clears it and deactivates the HUD.
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickable") && other.gameObject == currentPickable)
        {
            currentPickable = null;
            if (hud != null)
                hud.SetActive(false);
        }
    }

    /// @brief Performs the interaction action to pick up the current item.
    /// @details If a pickable item is current, creates a clone, adds it to inventory, disables the original,
    ///          clears current pickable, and hides HUD. Handles cases with or without inventory.
    private void Interact()
    {
        if (currentPickable != null)
        {
            Sprite itemSprite = currentPickable.GetComponent<PickableItem>()?.itemSprite;

            if (inventory != null && itemSprite != null)
            {
                // clone of active item
                GameObject clone = Instantiate(currentPickable);
                clone.SetActive(true); // must be true for OK copy
                clone.transform.SetParent(null); // removing from scene
                clone.transform.position = Vector3.zero;
                clone.transform.rotation = Quaternion.identity;
                clone.tag = "Untagged"; // no infinite picking

                // adding copy to queue
                inventory.AddItem(new InventoryItem(currentPickable.name, itemSprite, clone));
            }

            currentPickable.SetActive(false);
            currentPickable = null;

            if (hud != null)
                hud.SetActive(false);
        }
    }
}
