using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrab : MonoBehaviour
{
    public GameObject currentlySelectedItem; 
    public PlayerInventory playerInventory; 

    //Internal Variables
    private PlayerInputActions inputActions;  

    public bool isLensEquipped; // Track whether the Lens is currently equipped

    #region Lens Variables
    public float lensStamina = 100f; // Starting stamina
    public float lensDrainRate = 25f; // Stamina drain per second
    public float lensRegenRate = 10f; // Stamina regeneration per second when unequipped
    #endregion

    #region Grab Variables
    private GameObject grabbedObject;
    private Rigidbody grabbedObjectRb;
    private float grabDistance = 2f;
    #endregion

    private void Awake()
    {
        playerInventory = GetComponent<PlayerInventory>();
        inputActions = InputActionSingleton.Instance;

        if (inputActions == null)
        {
            Debug.LogError("PlayerInputActions.Instance is null");
            return;
        }

        //Bind the actions to a method
        //inputActions.Player.Interact.performed += InteractItem; 
        inputActions.Player.UseItem.performed += UseSelectedItem; 
        inputActions.Player.Grab.started += StartGrab; 
        inputActions.Player.Grab.canceled += EndGrab; 
    }

    private void Update()
    {
        HandleGrabbedObjectMovement();
        TurnOffLensWhenStaminaIsDrained();
        HandleLensEquippedState();
        HandleLensStamina();
    }

    #region Raycasting
    

    #endregion

    private void HandleGrabbedObjectMovement()
    {
        // Check if player is grabbing an object
        if (grabbedObject != null && inputActions.Player.Grab.IsPressed())
        {
            MoveGrabbedObject();
        }
    }

    private void HandleLensEquippedState()
    {
        // Check if the player doesn't have the lens set as their current device
        if (currentlySelectedItem != null && currentlySelectedItem.GetComponent<PlayerDevice>() != null &&
            currentlySelectedItem.GetComponent<PlayerDevice>().itemInstanceInEquipmentSlot == null)
        {
            Debug.Log("Deactivating lens");
            isLensEquipped = false;
            UpdateGrayscaleEffect();
        }
    }

    private void HandleLensStamina()
    {
        if (isLensEquipped)
        {
            DrainLensStamina();
        }
    }

    private void TurnOffLensWhenStaminaIsDrained()
    {
        if (lensStamina <= 0)
        {
            isLensEquipped = false;
            UpdateGrayscaleEffect();
        }
    }

    #region Grab
    private void MoveGrabbedObject()
    {
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * grabDistance;
        grabbedObject.transform.position = Vector3.Lerp(grabbedObject.transform.position, targetPosition, Time.deltaTime * 10f);
    }

    public void StartGrab(InputAction.CallbackContext context)
    {
        if (grabbedObject != null)
            return;

        // Use HandleRaycast() to perform the raycast and check for interactable objects
        RaycastHit? hitInfo = PlayerRaycast.instance.HandleRaycast();
        if (hitInfo.HasValue)
        {
            grabbedObject = hitInfo.Value.collider.gameObject;
            grabbedObjectRb = grabbedObject.GetComponent<Rigidbody>();

            if (grabbedObjectRb != null)
            {
                grabbedObjectRb.isKinematic = true; // Disable physics while grabbing
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

    #endregion

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
                    UpdateGrayscaleEffect();
                    return;
                }
                if (deviceName == "DayNightCycler")
                {
                    ToggleDayNightCycle();
                }
            }
        }
    }

    private void ToggleLens()
    {
        isLensEquipped = !isLensEquipped;
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

    #region Lens Mechanics
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
    #endregion

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
}
