using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleButton : Interactables
{
    public string colorToRestore = "";

    public override void Interact()
    {

        PuzzleManager.instance.OnPuzzleCompletion(colorToRestore);
    }

 


}
