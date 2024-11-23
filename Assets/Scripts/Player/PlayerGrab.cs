using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrab : MonoBehaviour
{
    public GameObject currentlySelectedItem;

    // Internal Variables
    private PlayerInputActions inputActions;

    #region Grab Variables
    private GameObject grabbedObject;
    private Rigidbody grabbedObjectRb;
    [SerializeField]
    private float grabDistance = 7f;
    #endregion

    private void Awake()
    {
        inputActions = InputActionSingleton.Instance;

        if (inputActions == null)
        {
            Debug.LogError("PlayerInputActions.Instance is null");
            return;
        }

        // Bind the actions to a method
        inputActions.Player.Grab.started += StartGrab;
        inputActions.Player.Grab.canceled += EndGrab;
    }

    private void Update()
    {
        HandleGrabbedObjectMovement();
    }

    #region Grab
    private void HandleGrabbedObjectMovement()
    {
        // Check if player is grabbing an object
        if (grabbedObject != null && inputActions.Player.Grab.IsPressed())
        {
            MoveGrabbedObject();
        }
    }

    private void MoveGrabbedObject()
    {
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * grabDistance;
        grabbedObject.transform.position = Vector3.Lerp(grabbedObject.transform.position, targetPosition, Time.deltaTime * 10f);
    }

    public void StartGrab(InputAction.CallbackContext context)
    {
        if (grabbedObject != null)
            return;

        GameObject raycastedObject = PlayerRaycast.instance.GetRaycastedObject();
        if (raycastedObject != null)
        {
            grabbedObject = raycastedObject;
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
}
