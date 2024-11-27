using UnityEngine;

public class Switch : Interactables
{
    public bool isOn = false;
    
    bool isInteractable = true;

    public override void Interact()
    {
        if(!isInteractable)
        {
            return;
        }

        isOn = !isOn;
        isInteractable = false;
    }

    public void Reset()
    {
        isOn = false;
        isInteractable = true;
    }
}