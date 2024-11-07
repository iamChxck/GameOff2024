using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public GameObject currentlySelectedItem; // Store the currently selected item

    public PlayerInventory playerInventory; // Reference to the player's inventory
    private PlayerInputActions inputActions; // Reference to the generated input actions
    private Camera playerCamera; // Reference to the player's camera for raycasting direction
    private OutlineEffect currentOutline; // Store the currently highlighted outline

    public float interactRange = 3f; // Range of interaction
    public bool hasLens; // Track whether the player has picked up the Lens
    public bool hasDayNightCycler; // Track whether the player has picked up the DayNightCycler
    public bool isLensEquipped; // Track whether the Lens is currently equipped

    // Lens stamina
    public float lensStamina = 100f; // Starting stamina
    public float lensDrainRate = 25f; // Stamina drain per second
    public float lensRegenRate = 10f; // Stamina regeneration per second when unequipped

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
        // Directly check if F is pressed - TEMPORARY MEASURE AS I HAVE NO IDEA WHY THE INTERACT BIND IS NOT WORKING WHEN THE USEITEM BIND IS WORKING JUST FINE (WILL FIX THIS LATER)
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            Debug.Log("F key pressed directly.");
            OnInteract(new InputAction.CallbackContext()); // Simulate calling the method
        }

        if (currentlySelectedItem != null && currentlySelectedItem.GetComponent<PlayerDevice>() != null &&
            currentlySelectedItem.GetComponent<PlayerDevice>().itemInstanceInEquipmentSlot == null)
        {
            isLensEquipped = false;
            UpdateGrayscaleEffect();
        }

        if (isLensEquipped)
        {
            DrainLensStamina();
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactRange))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                OutlineEffect outline = hit.collider.GetComponent<OutlineEffect>();
                if (outline != null && currentOutline != outline)
                {
                    DisableCurrentOutline();
                    currentOutline = outline;
                    currentOutline.EnableOutline();
                }
            }
            else
            {
                DisableCurrentOutline();
            }
        }
        else
        {
            DisableCurrentOutline();
        }

    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (currentOutline != null && currentOutline.CompareTag("Interactable"))
        {
            playerInventory.inventory.AddItem(currentOutline.gameObject.GetComponent<InstanceItemContainer>().item);

            if (currentOutline.name == "Lens")
            {
                hasLens = true;
            }
            else if (currentOutline.name == "DayNightCycler")
            {
                hasDayNightCycler = true;
            }

            Destroy(currentOutline.gameObject);
        }
    }

    private void UseSelectedItem(InputAction.CallbackContext context)
    {
        if (currentlySelectedItem != null)
        {
            PlayerDevice playerDevice = currentlySelectedItem.GetComponent<PlayerDevice>();
            if (playerDevice?.itemInstanceInEquipmentSlot != null)
            {
                string deviceName = playerDevice.itemInstanceInEquipmentSlot.itemType.itemName;

                if (deviceName == "Lens")
                {
                    ToggleLens();
                }
                else if (deviceName == "DayNightCycler")
                {
                    ToggleDayNightCycle();
                }
                else if (isLensEquipped)
                {
                    ToggleLens();
                }
            }
        }
    }

    private void ToggleLens()
    {
        isLensEquipped = !isLensEquipped;
        UpdateGrayscaleEffect();

    }

    private void DrainLensStamina()
    {
        lensStamina -= lensDrainRate * Time.deltaTime;
        if (lensStamina <= 0)
        {
            lensStamina = 0;
            ToggleLens();
            Debug.Log("Lens stamina depleted. Lens has been toggled off.");
        }
    }

    public void RegenerateLensStamina()
    {
        if (!isLensEquipped && lensStamina < 100f)
        {
            lensStamina += 25f * Time.deltaTime; // Adjust rate as needed
            if (lensStamina > 100f)
            {
                lensStamina = 100f;
            }
        }
    }

    private void ToggleDayNightCycle()
    {
        var dayNightManager = DayNightCycleManager.Instance;
        if (dayNightManager != null && !dayNightManager.isTransitioning)
        {
            if (dayNightManager.isDaytime)
                StartCoroutine(dayNightManager.ChangeToNight());
            else
                StartCoroutine(dayNightManager.ChangeToDay());
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
            GrayscaleManager.Instance.InstantRestoreColor();
        }
        else
        {
            GrayscaleManager.Instance.InstantGrayscale();
        }
    }
}
