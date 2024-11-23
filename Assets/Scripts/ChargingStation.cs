using UnityEngine;

public class ChargingStation : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerDeviceController playerDevice = other.GetComponent<PlayerDeviceController>();

            // Ensure the player has PlayerInteract and the lens is unequipped
            if (playerDevice != null && !playerDevice.isLensEquipped)
            {
                playerDevice.RegenerateLensStamina(); // Call regeneration method
            }
        }
    }
}
