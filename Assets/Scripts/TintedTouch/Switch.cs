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

        AudioManager.instance.PlaySFX("ButtonClick");

        isOn = !isOn;
        isInteractable = false;
    }

    public void Reset()
    {
        isOn = false;
        isInteractable = true;
        AudioManager.instance.PlaySFX("IncorrectPuzzle");
    }
}