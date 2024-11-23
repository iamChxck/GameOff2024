using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    private void Awake()
    {
        InputActionSingleton.Instance.Player.Interact.started += InteractItem;
    }

    private void InteractItem(InputAction.CallbackContext context)
    {
        if (PlayerRaycast.instance.currentOutline != null && PlayerRaycast.instance.currentOutline.CompareTag("Interactable"))
        {
            PlayerRaycast.instance.currentOutline.GetComponent<Interactables>().Interact();
            Debug.Log("Interacted");
        }
    }

    private void OnDestroy()
    {
        InputActionSingleton.Instance.Player.Interact.started -= InteractItem;
    }
}
