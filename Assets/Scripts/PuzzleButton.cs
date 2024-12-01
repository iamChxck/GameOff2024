public class PuzzleButton : Interactables
{
    public string colorToRestore = string.Empty;
    public EndGameAnimation endGameAnim;

    public override void Interact()
    {
        AudioManager.instance.PlaySFX("PuzzleFinish");

        if (gameObject.name == "FinalPuzzleButton") {
            PuzzleManager.instance.CompletePuzzle();
            if (endGameAnim != null) {
                endGameAnim.StartEndScene();
            }
        } else
            PuzzleManager.instance.OnPuzzleCompletion(colorToRestore);
    }
}