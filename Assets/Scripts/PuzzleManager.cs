using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager instance;

    public int puzzleCount;
    public GameObject puzzleDoor;

    void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        OpenDoor();
    }

    public void OnPuzzleCompletion(string colorToRestore)
    {
        if (GrayscaleManager.Instance != null)
        {
            // Add the material name to the restoredColors list to prevent it from being grayscaled
            if (!GrayscaleManager.Instance.restoredColors.Contains(colorToRestore))
            {
                GrayscaleManager.Instance.restoredColors.Add(colorToRestore);
                puzzleCount++;
            }
        }
    }



    public void CompletePuzzle()
    {
        Debug.Log("Puzzle Complete!");
        GrayscaleManager.Instance.RestoreAllColors();
    }

    public void OpenDoor()
    {
        if (puzzleCount == 6)
        {
            puzzleDoor.SetActive(false);
        }
    }

}
