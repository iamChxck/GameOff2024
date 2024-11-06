using System.Linq;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    public DynamicInventory inventory;
    public ItemDisplay[] slots;
    public ItemDisplay hoveredItem;
    public ItemDisplay currentlyDraggedItem;

    private void Start()
    {
        InitInventory();
    }

    private void Update()
    {
        UpdateInventory();
    }

    private void InitInventory()
    {
        // Find all ItemDisplay objects in the scene
        ItemDisplay[] unsortedSlots = FindObjectsOfType<ItemDisplay>();

        // Sort them based on their hierarchy order
        slots = unsortedSlots.OrderBy(go => go.transform.GetSiblingIndex()).ToArray();

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].itemIndex = i;
        }
    }

    void UpdateInventory()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            // Check if there is a valid item in the inventory slot
            if (inventory.items[i].itemType != null)
            {
                // Update the item display with the existing item
                slots[i].UpdateItemDisplay(inventory.items[i].itemType.icon,inventory.items[i].itemType.itemCategory);
            }
            else
            {
                // Check if the item is already a default item instance
                // This avoids redundant assignments and preserves performance
                ItemInstance defaultItem = inventory.items[i];
                if (defaultItem.itemType == null)
                {
                    // If already default, just update the display to reflect an empty slot
                    slots[i].UpdateItemDisplay(null, null); // Null indicates an empty display
                }
                else
                {
                    // If not already default, create a default item instance
                    inventory.items[i] = ItemInstance.CreateDefault();

                    // Update the item display to reflect the default (empty) slot
                    slots[i].UpdateItemDisplay(null, null); // Assuming null values indicate an empty display
                }
            }
        }
    }

    public void DropItem(int itemIndex)
    {
        // Check if the itemIndex is within bounds and the slot is not already a default item
        if (itemIndex >= 0 && itemIndex < inventory.maxItems && inventory.items[itemIndex].itemType != null)
        {
            // Drop the item from the inventory
            inventory.DropItem(itemIndex);

            // Update the inventory again
            UpdateInventory();
        }
        else
        {
            Debug.Log("Invalid item index or slot is already empty");
        }
    }



}