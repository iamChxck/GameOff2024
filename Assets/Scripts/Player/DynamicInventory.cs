using UnityEngine;

[CreateAssetMenu]
public class DynamicInventory : ScriptableObject
{
    public int maxItems = 10;
    public ItemInstance[] items; // Use an array instead of a list

    private void OnEnable()
    {
        // Initialize the array with default items
        items = new ItemInstance[maxItems];
        for (int i = 0; i < maxItems; i++)
        {
            items[i] = ItemInstance.CreateDefault();
        }
    }

    public bool AddItem(ItemInstance itemToAdd)
    {
        // Finds an empty slot if there is one
        for (int i = 0; i < maxItems; i++)
        {
            // Check if the slot is a default item indicating an empty slot
            if (items[i].itemType == null)
            {
                items[i] = itemToAdd;
                return true;
            }
        }

        Debug.Log("No space in the inventory");
        return false;
    }

    public bool MoveItemFromEquipment(ItemInstance itemToMove, int targetIndex)
    {
        // Check if the target index is within bounds
        if (targetIndex >= 0 && targetIndex < maxItems)
        {
            // Check if the target slot is empty
            if (items[targetIndex].itemType == null)
            {
                // Move the item to the target slot
                items[targetIndex] = itemToMove;
                return true;
            }
            else
            {
                Debug.Log("Target slot is not empty");
                return false;
            }
        }

        Debug.Log("Invalid target index");
        return false;
    }

    public bool SwapItem(int sourceIndex, int targetIndex)
    {
        // Check if the source and target indices are within bounds
        if (sourceIndex >= 0 && sourceIndex < maxItems && targetIndex >= 0 && targetIndex < maxItems)
        {
            // Swap the items in the source and target slots
            ItemInstance temp = items[sourceIndex];
            items[sourceIndex] = items[targetIndex];
            items[targetIndex] = temp;
            return true;
        }

        Debug.Log("Invalid source or target index");
        return false;
    }

    public bool RemoveItem(int itemIndex)
    {
        // Check if the index is within bounds and the slot is not already a default item
        if (itemIndex >= 0 && itemIndex < maxItems && items[itemIndex].itemType != null)
        {
            // Replace the item with a default item to indicate an empty slot
            items[itemIndex] = ItemInstance.CreateDefault();
            return true;
        }

        Debug.Log("Invalid item index or slot is already empty");
        return false;
    }

    public void DropItem(int itemIndex)
    {
        // Check if the itemIndex is within bounds and the slot is not already a default item
        if (itemIndex >= 0 && itemIndex < maxItems && items[itemIndex].itemType != null)
        {
            // Store the item to be dropped
            ItemInstance itemToDrop = items[itemIndex];
            // Instantiate the item model at the player's position
            Vector3 dropPosition = FindObjectOfType<PlayerController>().transform.position;
            GameObject itemModel = Instantiate(itemToDrop.itemType.modelPrefab, dropPosition, Quaternion.identity);


            //Place a default slot to indicate its empty
            items[itemIndex] = ItemInstance.CreateDefault();

        }
    }
}