/// @file PickableItem.cs
/// @brief This file contains the PickableItem class, which defines properties for items that can be picked up in the Unity game.
///        It includes sprite and rotation settings for HUD display and hand placement.
/// @details The PickableItem component is attached to GameObjects that represent collectible items.
///          It provides data for inventory integration, such as the sprite for UI slots and custom rotation when held in the player's hand.

using UnityEngine;

/// @brief Component for pickable items in the game.
/// @details This MonoBehaviour class holds data for items that can be interacted with and added to the inventory.
///          It specifies the sprite for HUD display and the rotation vector for proper orientation when equipped in the hand.
public class PickableItem : MonoBehaviour
{
    /// @brief The sprite used for displaying the item in inventory HUD slots.
    [Header("HUD Sprite")] // for inventory slots
    public Sprite itemSprite;

    /// @brief The rotation vector applied to the item when held in the player's hand.
    /// @details Defaults to Vector3.zero if not specified, allowing for custom orientations.
    [Header("InHand Rotation")] // for custom rotation in hand
    public Vector3 handRotation = Vector3.zero;
}
