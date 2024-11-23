using UnityEngine;

public class Switch : Interactables
{
    public bool isOn = false;

    public override void Interact()
    {
        isOn = !isOn;
    }

    public void Reset()
    {
        isOn = false;
    }
}