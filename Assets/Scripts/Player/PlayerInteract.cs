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

    //Grab variables
    private GameObject grabbedObject;
    private Rigidbody grabbedObjectRb;
    private float grabDistance = 2f;

    private void Awake()
    {
        playerInventory = GetComponent<PlayerInventory>();
        inputActions = PlayerInputActions.Instance;

        if (inputActions == null)
        {
            Debug.LogError("PlayerInputActions.Instance is null");
            return;
        }

        inputActions.Player.Interact.performed += InteractItem; // F key
        inputActions.Player.UseItem.performed += UseSelectedItem; // E key

        // Bind Grab action for holding objects
        inputActions.Player.Grab.started += StartGrab; // When LMB is pressed
        inputActions.Player.Grab.canceled += EndGrab; // When LMB is released

        //inputActions.Enable();
        playerCamera = Camera.main;
    }

    private void OnDestroy()
    {
        inputActions.Player.Interact.performed -= InteractItem;
        inputActions.Player.UseItem.performed -= UseSelectedItem;
        inputActions.Player.Grab.started -= StartGrab;
        inputActions.Player.Grab.canceled -= EndGrab;
    }

    private void Update()
    {

        //Check if player is grabbing an object
        if (grabbedObject != null && inputActions.Player.Grab.IsPressed())
        {
            MoveGrabbedObject();
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

    public void StartGrab(InputAction.CallbackContext context)
    {
        if (grabbedObject != null)
            return;

        // Perform raycast to detect interactable objects
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactRange))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                grabbedObject = hit.collider.gameObject;
                grabbedObjectRb = grabbedObject.GetComponent<Rigidbody>();

                if (grabbedObjectRb != null)
                {
                    grabbedObjectRb.isKinematic = true; // Disable physics while grabbing
                }
            }
        }
    }

    private void EndGrab(InputAction.CallbackContext context)
    {
        ReleaseObject();
    }

    private void ReleaseObject()
    {
        if (grabbedObject != null && grabbedObjectRb != null)
        {
            grabbedObjectRb.isKinematic = false;
        }
        grabbedObject = null;
        grabbedObjectRb = null;
    }

    private void MoveGrabbedObject()
    {
        Vector3 targetPosition = playerCamera.transform.position + playerCamera.transform.forward * grabDistance;
        grabbedObject.transform.position = Vector3.Lerp(grabbedObject.transform.position, targetPosition, Time.deltaTime * 10f);
    }

    private void InteractItem(InputAction.CallbackContext context)
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
            GrayscaleManager.Instance.GraduallyRestoreColor();
        }
        else
        {
            GrayscaleManager.Instance.GraduallyApplyGrayscale();
        }
    }
}
