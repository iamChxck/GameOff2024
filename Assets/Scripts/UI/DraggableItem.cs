using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform originalParent;
    public Image itemImage;
    public Vector2 originalTransform;
    private GameObject dragPlaceholder;
    private GameObject cloneItem;

    private void Start()
    {
        itemImage = GetComponent<Image>();
        originalTransform = itemImage.rectTransform.sizeDelta;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Create a clone for visual feedback while dragging
        cloneItem = Instantiate(gameObject, transform.position, transform.rotation, transform.parent);
        cloneItem.GetComponent<DraggableItem>().enabled = false;

        Color color = itemImage.color;
        color.a = 0.5f;
        cloneItem.GetComponent<Image>().color = color;

        // Create a placeholder to maintain layout
        dragPlaceholder = new GameObject("Placeholder");
        dragPlaceholder.transform.SetParent(transform.parent);
        LayoutElement le = dragPlaceholder.AddComponent<LayoutElement>();

        LayoutElement itemLayoutElement = GetComponent<LayoutElement>();
        if (itemLayoutElement != null)
        {
            le.preferredWidth = itemLayoutElement.preferredWidth;
            le.preferredHeight = itemLayoutElement.preferredHeight;
            le.flexibleWidth = 0;
            le.flexibleHeight = 0;
        }

        originalParent = transform.parent;
        transform.SetParent(dragPlaceholder.transform.parent);

        // Disable raycasting to allow dragging without interference
        itemImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;

        if (GetComponentInParent<ItemDisplay>() != null)
            FindObjectOfType<InventoryDisplay>().currentlyDraggedItem = GetComponentInParent<ItemDisplay>();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(cloneItem);

        // Reset color and raycasting
        Color color = itemImage.color;
        color.a = 1f;
        itemImage.color = color;
        itemImage.raycastTarget = true;

        ItemDisplay droppedOnItemDisplay = eventData.pointerEnter?.GetComponent<ItemDisplay>();

        if (eventData.pointerEnter != null && eventData.pointerEnter.gameObject.layer == LayerMask.NameToLayer("UI"))
        {
            // Return to original parent if dropped in UI
            transform.SetParent(originalParent);
        }
        else
        {
            if (FindObjectOfType<InventoryDisplay>().currentlyDraggedItem != null)
                FindObjectOfType<InventoryDisplay>().DropItem(FindObjectOfType<InventoryDisplay>().currentlyDraggedItem.itemIndex);
        }

        // Ensure item returns to original parent
        transform.SetParent(originalParent != null ? originalParent : FindObjectOfType<Canvas>().transform);
        transform.localPosition = Vector3.zero;

        itemImage.rectTransform.sizeDelta = originalTransform;
        FindObjectOfType<InventoryDisplay>().currentlyDraggedItem = null;
        Destroy(dragPlaceholder);
    }
}
