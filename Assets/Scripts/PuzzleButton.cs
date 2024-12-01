public class PuzzleButton : Interactables
{
    public string colorToRestore = string.Empty;

    public override void Interact()
    {
        AudioManager.instance.PlaySFX("PuzzleFinish");

        if (gameObject.name == "FinalPuzzleButton")
            PuzzleManager.instance.CompletePuzzle();
        else
            PuzzleManager.instance.OnPuzzleCompletion(colorToRestore);
    }
}