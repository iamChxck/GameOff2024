using UnityEngine;

public class ChargingStation : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInteract playerInteract = other.GetComponent<PlayerInteract>();

            // Ensure the player has PlayerInteract and the lens is unequipped
            if (playerInteract != null && !playerInteract.isLensEquipped)
            {
                playerInteract.RegenerateLensStamina(); // Call regeneration method
            }
        }
    }
}
