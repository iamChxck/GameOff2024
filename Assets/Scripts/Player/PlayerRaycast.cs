using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRaycast : MonoBehaviour
{
    public static PlayerRaycast instance;

    [SerializeField] private float interactRange = 3f;

    private Camera playerCamera;

    public OutlineEffect currentOutline;

    private void Awake()
    {
        instance = this;
        playerCamera = Camera.main;
    }

    void Update()
    {
        HandleRaycast();
    }

    public RaycastHit? HandleRaycast()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                OutlineEffect outline = hit.collider.GetComponent<OutlineEffect>();
                
                if (outline != null && currentOutline != outline)
                {
                    CheckForOutlineThenDisableCurrentOutline();
                    currentOutline = outline;
                    currentOutline.EnableOutline();
                }
                return hit;
            }
            else
            {
                CheckForOutlineThenDisableCurrentOutline();
            }
        }
        else
        {
            CheckForOutlineThenDisableCurrentOutline();
        }

        return null;
    }

    private void CheckForOutlineThenDisableCurrentOutline()
    {
        if (currentOutline != null)
        {
            currentOutline.DisableOutline();
            currentOutline = null;
        }
    }
}
