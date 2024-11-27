using UnityEngine;

public class TileTrigger : MonoBehaviour
{
    public string tileColor; // To specify which tile it is
    private EchoingColor chromaticPair; // Cached reference to the ChromaticPair script

    private void Start()
    {

        // Cache the ChromaticPair component reference for better performance
        chromaticPair = FindObjectOfType<EchoingColor>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Log the player's interaction with the tile
            Debug.Log("Player stepped on: " + tileColor);

            // Notify ChromaticPair or handle sequence logic
            if (chromaticPair != null)
            {
                chromaticPair.CheckTileSequence(tileColor); // Pass the tile color to ChromaticPair
            }
            else
            {
                Debug.LogError("ChromaticPair component not found in the scene.");
            }
        }
    }
}
