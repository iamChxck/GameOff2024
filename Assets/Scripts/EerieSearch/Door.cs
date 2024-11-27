using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactables
{
    [SerializeField] Animator animator;

    [SerializeField] PlayerInventory inventory;

    public override void Interact()
    {
        if (inventory.hasKey)
        {
            animator.SetTrigger("OpenDoor");
            return;
        }

        // Have a UI maybe to tell the player to find the key
    }
}
