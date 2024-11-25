using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager instance;

    void Awake()
    {
        // Ensure only one instance exists
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

    public void OnPuzzleCompletion(string colorToRestore)
    { 
        if (GrayscaleManager.Instance != null)
        {
            // Add the material name to the restoredColors list to prevent it from being grayscaled
            if (!GrayscaleManager.Instance.restoredColors.Contains(colorToRestore))
            {
                GrayscaleManager.Instance.restoredColors.Add(colorToRestore);
            }
        }
    }
}
