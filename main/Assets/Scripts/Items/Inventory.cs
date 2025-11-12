/// @file Inventory.cs
/// @brief This file contains the implementation of the Inventory system in a Unity game.
///        It includes the InventoryItem data structure and the Inventory class that manages item queuing, HUD updates, and hand object instantiation.
///        The Inventory class implements a custom IInventoryQueue interface for queue operations.
/// @details The Inventory system uses a queue to manage items, updating UI slots and the player's hand object.
///          It supports adding items, cycling through them, and displaying sprites in HUD slots.
///          The system ensures proper instantiation and rotation of items in the player's hand.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

/// @brief Serializable class representing an item in the inventory.
/// @details Contains the name, sprite, and prefab for an inventory item.
///          Used to store and display items in the queue.
[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public Sprite itemSprite;
    public GameObject prefab;

    /// @brief Constructor for InventoryItem.
    /// @param name The name of the item.
    /// @param sprite The sprite used for UI display.
    /// @param prefabObject The GameObject prefab associated with the item.
    public InventoryItem(string name, Sprite sprite, GameObject prefabObject)
    {
        itemName = name;
        itemSprite = sprite;
        prefab = prefabObject;
    }
}

/// @brief Main inventory management class that implements IInventoryQueue for InventoryItem.
/// @details Manages a queue of inventory items, updates HUD slots, and handles item instantiation in the player's hand.
///          Provides methods to add items, cycle through them, and delegate queue operations.
///          Inherits from MonoBehaviour for Unity integration.
public class Inventory : MonoBehaviour, IInventoryQueue<InventoryItem> //double inheritance hierarchy
{
    [Header("HUD Slots")]
    [SerializeField] private UnityEngine.UI.Image mainSlotImage;
    [SerializeField] private UnityEngine.UI.Image subSlotImage;

    [Header("Hand Object")]
    [SerializeField] private Transform hand;
    private GameObject currentItemInHand;

    private InventoryQueue<InventoryItem> itemQueue = new InventoryQueue<InventoryItem>();

    /// @brief Gets the count of items in the inventory queue.
    /// @return The number of items in the queue.
    public int Count => itemQueue.Count;

    /// @brief Adds an item to the end of the inventory queue.
    /// @param item The InventoryItem to enqueue.
    /// @details Updates HUD and hand after enqueuing.
    public void Enqueue(InventoryItem item)
    {
        itemQueue.Enqueue(item);
        UpdateHUD();
        UpdateHand();
    }

    /// @brief Removes and returns the first item from the inventory queue.
    /// @return The dequeued InventoryItem.
    /// @throws InvalidOperationException If the queue is empty.
    /// @details Updates HUD and hand after dequeuing.
    public InventoryItem Dequeue()
    {
        InventoryItem item = itemQueue.Dequeue();
        UpdateHUD();
        UpdateHand();
        return item;
    }

    /// @brief Converts the inventory queue to an array.
    /// @return An array of InventoryItem representing the queue contents.
    public InventoryItem[] IQtoArray()
    {
        return itemQueue.IQtoArray();
    }

    /// @brief Initializes the inventory on start.
    /// @details Calls UpdateHUD and UpdateHand to set initial states.
    private void Start()
    {
        UpdateHUD();
        UpdateHand();
    }

    /// @brief Adds a new item to the inventory.
    /// @param newItem The InventoryItem to add.
    /// @details Delegates to Enqueue for queue management.
    public void AddItem(InventoryItem newItem)
    {
        Enqueue(newItem);
    }

    /// @brief Cycles to the next item in the inventory.
    /// @details If more than one item exists, dequeues the first and enqueues it to the end.
    public void NextItem()
    {
        if (Count > 1)
        {
            InventoryItem first = Dequeue();
            Enqueue(first);
        }
    }

    /// @brief Updates the HUD slots based on current queue contents.
    /// @details Sets sprites and visibility for main and sub slots.
    private void UpdateHUD()
    {
        InventoryItem[] items = IQtoArray();

        // MainSlot
        if (items.Length > 0)
        {
            mainSlotImage.sprite = items[0].itemSprite;
            mainSlotImage.enabled = true;
        }
        else
        {
            mainSlotImage.sprite = null;
            mainSlotImage.enabled = false;
        }

        // SubSlot
        if (items.Length > 1)
        {
            subSlotImage.sprite = items[1].itemSprite;
            subSlotImage.enabled = true;
        }
        else
        {
            subSlotImage.sprite = null;
            subSlotImage.enabled = false;
        }
    }

    /// @brief Updates the item in the player's hand based on the current queue.
    /// @details Destroys the previous item and instantiates the new one with proper rotation.
    private void UpdateHand()
    {
        // removing last item
        if (currentItemInHand != null)
            Destroy(currentItemInHand);

        InventoryItem[] items = IQtoArray();

        // if there is/are items in queue - adding it/them
        if (items.Length > 0 && items[0].prefab != null)
        {
            currentItemInHand = Instantiate(items[0].prefab, hand);
            currentItemInHand.transform.localPosition = Vector3.zero;
            PickableItem pickable = items[0].prefab.GetComponent<PickableItem>();
            if (pickable != null)
                currentItemInHand.transform.localRotation = Quaternion.Euler(pickable.handRotation);
            else
                currentItemInHand.transform.localRotation = Quaternion.identity;
        }
    }
}
