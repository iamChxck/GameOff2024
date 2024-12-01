using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public DynamicInventory inventory;
    public InventoryDisplay inventoryDisplay;

    public GameObject pauseMenu;

    public bool isInventoryOpen;
    public bool hasKey; // Boolean to track if the inventory contains a key

    private void Awake()
    {
        inventoryDisplay = FindObjectOfType<InventoryDisplay>();
    }

    private void Start()
    {
        inventoryDisplay.gameObject.SetActive(false);
        CheckForKey(); // Initial check for the key
    }

    private void Update()
    {
        ToggleInventory();

        // Update the hasKey status every frame (optional, if needed dynamically)
        CheckForKey();
    }

    private void ToggleInventory()
    {
        if (InputActionSingleton.Instance.UI.Inventory.triggered)
        {
            if (inventoryDisplay.gameObject.activeInHierarchy)
            {
                inventoryDisplay.gameObject.SetActive(false);
                isInventoryOpen = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                pauseMenu.SetActive(false);
                GetComponent<PlayerController>().paused = false;
                Time.timeScale = 1;
                inventoryDisplay.gameObject.SetActive(true);
                isInventoryOpen = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }


    /// <summary>
    /// Check if the inventory contains an item named "Key"
    /// </summary>
    private void CheckForKey()
    {
        hasKey = false; // Reset the key status

        foreach (ItemInstance item in inventory.items)
        {
            if (item != null && item.itemType != null && item.itemType.name == "Key")
            {
                hasKey = true;
                break; // Exit the loop early as we found a key
            }
        }
    }
}
