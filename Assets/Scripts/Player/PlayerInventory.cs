using UnityEngine;


public class PlayerInventory : MonoBehaviour
{
    public DynamicInventory inventory;
    public InventoryDisplay inventoryDisplay;

    public bool isInventoryOpen;


    private void Awake()
    {
        inventoryDisplay = FindObjectOfType<InventoryDisplay>();
    }
    private void Start()
    {
        inventoryDisplay.gameObject.SetActive(false);
    }

    private void Update()
    {
        ToggleInventory();

        if (isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void ToggleInventory()
    {
        if (PlayerInputActions.Instance.UI.Inventory.triggered)
        {
            Debug.Log("Toggling Inventory");
            if (inventoryDisplay.gameObject.activeInHierarchy)
            {
                inventoryDisplay.gameObject.SetActive(false);
                isInventoryOpen = false;
            }
            else
            {
                inventoryDisplay.gameObject.SetActive(true);
                isInventoryOpen = true;
            }
        }
    }




}
