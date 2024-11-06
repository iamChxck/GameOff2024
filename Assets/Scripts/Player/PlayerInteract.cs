using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public PlayerInventory playerInventory; // Reference to the player's inventory
    public float interactRange = 3f; // Range of interaction
    private PlayerInputActions inputActions; // Reference to the generated input actions
    private Camera playerCamera; // Reference to the player's camera for raycasting direction
    private OutlineEffect currentOutline; // Store the currently highlighted outline
    public bool hasLens; // Track whether the player has picked up the Lens
    public bool hasDayNightCycler; // Track whether the player has picked up the DayNightCycler
    public bool isLensEquipped; // Track whether the Lens is currently equipped
    public GameObject currentlySelectedItem; // Store the currently selected item

    private void Awake()
    {
        playerInventory = GetComponent<PlayerInventory>();
        inputActions = PlayerInputActions.Instance; // Use the singleton instance
        inputActions.Player.Interact.performed += OnInteract; // Bind the Interact action
        inputActions.Player.UseItem.performed += UseSelectedItem; // Bind the UseItem action
        inputActions.Enable();

        playerCamera = Camera.main; // Assumes the main camera is used for the player's view
    }

    private void OnDestroy()
    {
        inputActions.Player.Interact.performed -= OnInteract; // Unbind the Interact action
        inputActions.Player.UseItem.performed -= UseSelectedItem; // Unbind the UseItem action
    }

    private void Update()
    {
        if(currentlySelectedItem != null)
        {
            if(currentlySelectedItem.GetComponent<PlayerDevice>() != null)
            {
                if(currentlySelectedItem.GetComponent<PlayerDevice>().itemInstanceInEquipmentSlot == null)
                {
                    isLensEquipped = false;
                    UpdateGrayscaleEffect();

                }
            }
        }
        // Directly check if F is pressed
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            Debug.Log("F key pressed directly.");
            OnInteract(new InputAction.CallbackContext()); // Simulate calling the method
        }
        #region Raycast outline
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        // Check if the raycast hits something within the interaction range
        if (Physics.Raycast(ray, out hit, interactRange))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name); // Log the name of the hit object

            // Check if the hit object is interactable
            if (hit.collider.CompareTag("Interactable"))
            {
                OutlineEffect outline = hit.collider.GetComponent<OutlineEffect>();

                // Check if the outline component is found
                if (outline != null)
                {
                    // Enable outline on the new object, and disable it on the previous one
                    if (currentOutline != outline)
                    {
                        DisableCurrentOutline();
                        currentOutline = outline;
                        currentOutline.EnableOutline();
                    }
                }
            }
            else
            {
                // If the object is not interactable, disable the outline
                DisableCurrentOutline();
            }
        }
        else
        {
            // No hit, disable the outline if it exists
            DisableCurrentOutline();
        }
        #endregion

    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Interact action triggered"); // Log when the action is triggered

        if (currentOutline != null)
        {
            Debug.Log("Interacted with: " + currentOutline.name);
            // Check if the object is a Lens or DayNightCycler
            if (currentOutline.tag == "Interactable")
            {
                playerInventory.inventory.AddItem(currentOutline.gameObject.GetComponent<InstanceItemContainer>().item);

                if (currentOutline.name == "Lens")
                {
                    hasLens = true; // Set hasLens to true when interacting with Lens
                    Debug.Log("Lens picked up.");
                }

                if (currentOutline.name == "DayNightCycler")
                {
                    hasDayNightCycler = true; // Set hasDayNightCycler to true when interacting with DayNightCycler
                    Debug.Log("DayNightCycler picked up.");
                }
                else
                {
                    Debug.LogWarning("No valid item to use! Please Equip a valid device");
                }
            }
            // Perform interaction logic here, such as picking up the object
            Destroy(currentOutline.gameObject);
        }
        else
        {
            Debug.Log("No object to interact with");
        }
    }

    private void UseSelectedItem(InputAction.CallbackContext context)
    {
        Debug.Log("UseSelectedItem action triggered."); // Log when the action is triggered

        // Check if the currently selected item is not null
        if (currentlySelectedItem != null)
        {
            // Attempt to get the PlayerDevice component attached to the currently selected item
            PlayerDevice playerDevice = currentlySelectedItem.GetComponent<PlayerDevice>();

            if (playerDevice != null)
            {
                if (playerDevice.itemInstanceInEquipmentSlot != null)
                {
                    // Use the Name string inside PlayerDevice
                    string deviceName = playerDevice.itemInstanceInEquipmentSlot.itemType.itemName;

                    Debug.Log("Selected device name: " + deviceName);

                    // Check if the selected device is the Lens
                    if (deviceName == "Lens")
                    {
                        ToggleLens(); // Toggle the lens effect when equipped
                    }
                    else if (deviceName == "DayNightCycler")
                    {
                        ToggleDayNightCycle(); // Toggle day-night cycle
                    }
                    else
                    {
                        // If it's not a recognized device, reset the lens effect
                        if (isLensEquipped)
                        {
                            ToggleLens(); // Toggle off the lens when another device is selected
                        }
                        Debug.LogWarning("Unknown device selected!");
                    }
                }
            }
            else
            {
                Debug.LogWarning("Currently selected item does not have a PlayerDevice component!");
            }
        }
        else
        {
            Debug.Log("No item selected to use.");
        }
    }



    private void ToggleLens()
    {
        // Toggle the lens effect on or off
        isLensEquipped = !isLensEquipped; // Change the equipped status

        // Update the grayscale effect based on whether the lens is equipped
        if (isLensEquipped)
        {
            GrayscaleManager.Instance.InstantRestoreColor(); // Restore full color when the lens is equipped
            Debug.Log("Lens equipped. Color restored.");
        }
        else
        {
            GrayscaleManager.Instance.InstantGrayscale(); // Apply grayscale effect when the lens is unequipped
            Debug.Log("Lens unequipped. Grayscale applied.");
        }
    }



    private void ToggleDayNightCycle()
    {
        var dayNightManager = DayNightCycleManager.Instance; // Reference to DayNightCycleManager
        if (dayNightManager != null && !dayNightManager.isTransitioning) // Ensure not in transition
        {
            if (dayNightManager.isDaytime)
            {
                StartCoroutine(dayNightManager.ChangeToNight());
            }
            else
            {
                StartCoroutine(dayNightManager.ChangeToDay());
            }
            Debug.Log("Day-Night cycle toggled.");
        }
        else
        {
            Debug.Log("Cannot toggle day-night cycle: already in transition or manager not available.");
        }
    }

    private void DisableCurrentOutline()
    {
        if (currentOutline != null)
        {
            currentOutline.DisableOutline();
            currentOutline = null;
        }
    }

    private void UpdateGrayscaleEffect()
    {
        if (isLensEquipped)
        {
            GrayscaleManager.Instance.InstantRestoreColor(); // Grayscale amount set to 0
        }
        else
        {
            GrayscaleManager.Instance.InstantGrayscale(); // Grayscale amount set to 1
        }
    }
}
