using UnityEngine;

[System.Serializable]
public class ItemInstance
{
    public ItemData itemType;
    public string name;
    public string description;

    public GameObject itemGameObject;

    // Constructor for creating a specific item instance
    public ItemInstance(ItemData itemData)
    {
        itemType = itemData;
        name = itemData.itemName;
        description = itemData.itemDescription;
    }

    // Private constructor for the default item instance
    private ItemInstance()
    {
        itemType = null; // No item type for the default instance
        name = null;
        description = null;

    }

    // Static method for creating a default item instance
    public static ItemInstance CreateDefault()
    {
        return new ItemInstance();
    }
}
