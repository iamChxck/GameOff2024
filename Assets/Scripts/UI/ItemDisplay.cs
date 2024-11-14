using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ItemDisplay : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int itemIndex;
    public string itemCategory;
    public GameObject inventoryInfoWindow;
    public Image image;
    public TMP_Text itemNameText;
    public GameObject itemInSlot;

    private void Start()
    {
        if (itemNameText != null)
            itemNameText.raycastTarget = false;
  
        image = gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
    }

    private void OnDisable()
    {
        if (inventoryInfoWindow != null)
            inventoryInfoWindow.gameObject.SetActive(false);
    }

    public void UpdateItemDisplay(Sprite newSprite, string itemType)
    {
        itemCategory = itemType;
        image.gameObject.SetActive(true);
        image.transform.GetComponent<Image>().sprite = newSprite;
    }

    public void DeactivateItemDisplay()
    {
        image.gameObject.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggableItem = eventData.pointerDrag?.GetComponent<DraggableItem>();
        PlayerDevice playerDevice = eventData.pointerDrag?.GetComponentInParent<PlayerDevice>();
        //When the player moves an item from the inventory to another inventory slot
        if (draggableItem != null && playerDevice == null)
        {
            // Swap the item in the DynamicInventory
            FindObjectOfType<PlayerInventory>().inventory.SwapItem(draggableItem.gameObject.GetComponentInParent<ItemDisplay>().itemIndex, itemIndex);
        }
        else if(draggableItem != null && playerDevice != null)
        {
            FindObjectOfType<PlayerInventory>().inventory.MoveItemFromEquipment(playerDevice.itemInstanceInEquipmentSlot, itemIndex);
            playerDevice.itemInstanceInEquipmentSlot = null;
            playerDevice.equipmentImage.sprite = null;
        }
        else
        {
            Debug.LogWarning($"You can only drop items in the inventory!");
        }
    }


    public void UpdateInfoWindow(ItemInstance item)
    {
        if (item.itemType == null)
            return;
        if (itemNameText != null)
        {
            // Set item text
            itemNameText.text = item.name;
        }
           
        if (inventoryInfoWindow != null)
            inventoryInfoWindow.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer entered: " + name);
        // Set the hovered item in the InventoryDisplay script when the pointer enters this item
        FindObjectOfType<InventoryDisplay>().hoveredItem = this;
        UpdateInfoWindow(FindObjectOfType<InventoryDisplay>().inventory.items[itemIndex]);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer exited: " + name);
        // Clear the hovered item in the InventoryDisplay script when the pointer exits this item
        if (FindObjectOfType<InventoryDisplay>().hoveredItem == this)
        {
            FindObjectOfType<InventoryDisplay>().hoveredItem = null;
            if (inventoryInfoWindow != null)
                inventoryInfoWindow.SetActive(false);
        }
    }
}
