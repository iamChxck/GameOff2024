using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerDevice : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ItemInstance itemInstanceInEquipmentSlot;
    public string itemCategory;
    public Image equipmentImage;
    public bool isHovered;

    private PlayerInteract playerInteract;

    void Awake()
    {
        // Ensure the child object exists before getting the Image component
        equipmentImage = transform.GetChild(0)?.GetComponent<Image>();

        // Get reference to PlayerInteract
        playerInteract = FindObjectOfType<PlayerInteract>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ItemDisplay itemDisplay = eventData.pointerDrag?.GetComponentInParent<ItemDisplay>();

        if (itemDisplay != null)
        {
            if (itemDisplay.itemCategory == "Device")
            {
                equipmentImage.color = Color.green;
                isHovered = true;
            }
            else
            {
                equipmentImage.color = Color.red;
                isHovered = true;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isHovered)
        {
            equipmentImage.color = Color.white;
            isHovered = false;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggableItem = eventData.pointerDrag?.GetComponent<DraggableItem>();
        if (draggableItem != null && draggableItem.GetComponentInParent<ItemDisplay>() != null)
        {
            if (gameObject.tag == "SlotDevice")
            {
                equipmentImage.sprite = draggableItem.itemImage.sprite;
                itemInstanceInEquipmentSlot = FindObjectOfType<InventoryDisplay>().inventory.items[draggableItem.GetComponentInParent<ItemDisplay>().itemIndex];

                // Update currentlySelectedItem in PlayerInteract
                if (playerInteract != null)
                {
                    playerInteract.currentlySelectedItem = itemInstanceInEquipmentSlot != null ? gameObject : null;
                }

                // Remove the item from the DynamicInventory
                FindObjectOfType<PlayerInventory>().inventory.RemoveItem(draggableItem.gameObject.GetComponentInParent<ItemDisplay>().itemIndex);
            }
        }
    }


    public void ResetSlot()
    {
        itemInstanceInEquipmentSlot = null;
        itemCategory = null;
        equipmentImage.sprite = null;

        // Clear currentlySelectedItem in PlayerInteract
        if (playerInteract != null)
        {
            playerInteract.currentlySelectedItem = null;
        }
    }
}
