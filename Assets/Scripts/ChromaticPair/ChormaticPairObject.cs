using UnityEngine;

public class ChromaticPairObject : MonoBehaviour
{
    public string tileColor; // The color of the corresponding tile (Red, Yellow, or Blue)
    private bool isOnTile = false; // Track if the cube is on the correct tile

    // This will reference the ChromaticPair script to notify if cube is on tile
    private ChromaticPair chromaticPairScript;

    private void Start()
    {
        // Set the tileColor automatically based on the name of the parent object
        tileColor = gameObject.name.Replace("Cube", ""); // This assumes the object name is something like "RedCube", "YellowCube", etc.

        // Find the ChromaticPair script on the parent object (or wherever it's located)
        chromaticPairScript = FindObjectOfType<ChromaticPair>();

        // Log the color to verify
        Debug.Log($"Tile color for this cube: {tileColor}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tile"))
        {
            if (other.name.Contains(tileColor)) // If tile name matches cube's color
            {
                isOnTile = true;
                chromaticPairScript.UpdateCubeOnTile(tileColor, true);
                Debug.Log($"{tileColor} Cube is on the correct tile!");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tile"))
        {
            if (other.name.Contains(tileColor)) // If tile name matches cube's color
            {
                isOnTile = false;
                chromaticPairScript.UpdateCubeOnTile(tileColor, false);
                Debug.Log($"{tileColor} Cube is no longer on the correct tile.");
            }
        }
    }

    public bool IsOnTile()
    {
        return isOnTile;
    }
}
