using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDeviceController : MonoBehaviour
{
    public static GameObject currentlySelectedItem;

    public bool isLensEquipped;

    #region Lens Variables
    public float lensStamina = 100f; // Starting stamina
    public float lensDrainRate = 25f; // Stamina drain per second
    public float lensRegenRate = 10f; // Stamina regeneration per second when unequipped
    #endregion

    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = InputActionSingleton.Instance;

        if (inputActions == null)
        {
            Debug.LogError("PlayerInputActions.Instance is null");
            return;
        }

        // Bind actions to methods
        inputActions.Player.UseItem.performed += UseSelectedItem;
    }

    private void Update()
    {
        HandleLensEquippedState();
        HandleLensStamina();
        TurnOffLensWhenStaminaIsDrained();
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
            lensStamina += lensRegenRate * Time.deltaTime;
            if (lensStamina > 100f)
            {
                lensStamina = 100f;
            }
        }
    }

    private void UseSelectedItem(InputAction.CallbackContext context)
    {
        Debug.Log("Using Item");
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
                }
                else if (deviceName == "DayNightCycler")
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
