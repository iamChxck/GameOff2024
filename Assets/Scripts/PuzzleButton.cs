public class PuzzleButton : Interactables
{
    public string colorToRestore = "";

    public override void Interact()
    {
        if (gameObject.name == "FinalPuzzleButton")
            PuzzleManager.instance.CompletePuzzle();
        else
            PuzzleManager.instance.OnPuzzleCompletion(colorToRestore);
    }

}
