using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoingColor : MonoBehaviour
{
    public GameObject door;  // Reference to the door object
    public GameObject tiles; // Reference to the tiles object
    private MeshRenderer pattern1Renderer, pattern2Renderer, pattern3Renderer;
    private Material redTileMaterial, blueTileMaterial, yellowTileMaterial;
    private List<Material> tileMaterials = new List<Material>();
    private List<Material> randomizedPattern = new List<Material>();

    private int currentTileIndex = 0;
    public List<MeshRenderer> steppedOnTiles = new List<MeshRenderer>();  // Track the tiles the player has stepped on

    public string colorToRestore = "Red";

    void Start()
    {
        // Get references to the door's patterns
        pattern1Renderer = door.transform.Find("Pattern/Pattern 1").GetComponent<MeshRenderer>();
        pattern2Renderer = door.transform.Find("Pattern/Pattern 2").GetComponent<MeshRenderer>();
        pattern3Renderer = door.transform.Find("Pattern/Pattern 3").GetComponent<MeshRenderer>();

        // Load the materials from Resources
        redTileMaterial = Resources.Load<Material>("Materials/EchoingColor/RedTile");
        blueTileMaterial = Resources.Load<Material>("Materials/EchoingColor/BlueTile");
        yellowTileMaterial = Resources.Load<Material>("Materials/EchoingColor/YellowTile");

        tileMaterials.Add(redTileMaterial);
        tileMaterials.Add(blueTileMaterial);
        tileMaterials.Add(yellowTileMaterial);

        // Randomize the pattern on the door
        RandomizePattern();

        // Set up the tile colliders as triggers
        SetupTileColliders();
    }

    void Update()
    {
        // Any additional behavior related to tile interaction can be added here.
    }

    void RandomizePattern()
    {
        // Ensure we start with a clean pattern
        randomizedPattern.Clear();

        // Create a copy of the tileMaterials list to shuffle
        List<Material> patternMaterials = new List<Material>(tileMaterials);

        // Shuffle the patternMaterials list
        for (int i = patternMaterials.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Material temp = patternMaterials[i];
            patternMaterials[i] = patternMaterials[randomIndex];
            patternMaterials[randomIndex] = temp;
        }

        // Assign the shuffled materials to the patterns on the door
        pattern1Renderer.material = patternMaterials[0];
        pattern2Renderer.material = patternMaterials[1];
        pattern3Renderer.material = patternMaterials[2];

        // Save the randomized pattern
        randomizedPattern.Add(patternMaterials[0]);
        randomizedPattern.Add(patternMaterials[1]);
        randomizedPattern.Add(patternMaterials[2]);

        // Log the pattern for debugging
        Debug.Log("Randomized Door Pattern: " +
                  patternMaterials[0].name + ", " +
                  patternMaterials[1].name + ", " +
                  patternMaterials[2].name);
    }


    void SetupTileColliders()
    {
        // Assuming your tiles already have colliders set as triggers
        Transform redTile = tiles.transform.Find("RedTile");
        Transform blueTile = tiles.transform.Find("BlueTile");
        Transform yellowTile = tiles.transform.Find("YellowTile");

        // Enable the tile colliders as triggers (this ensures no collision but detection)
        redTile.GetComponent<Collider>().isTrigger = true;
        blueTile.GetComponent<Collider>().isTrigger = true;
        yellowTile.GetComponent<Collider>().isTrigger = true;
    }

    public void CheckTileSequence(string tileColor)
    {
        // Map color to material, convert tileColor to lowercase
        Material selectedMaterial = null;

        switch (tileColor.ToLower())  // Convert tileColor to lowercase for comparison
        {
            case "redtile": // Use lowercase name if tiles are named "RedTile", "BlueTile", "YellowTile"
                selectedMaterial = redTileMaterial;
                break;
            case "bluetile":
                selectedMaterial = blueTileMaterial;
                break;
            case "yellowtile":
                selectedMaterial = yellowTileMaterial;
                break;
            default:
                Debug.LogError("Unknown tile color: " + tileColor);
                return;
        }

        if (selectedMaterial != null)
        {
            // Log the current tile index and the material selected
            Debug.Log("Current Tile Index: " + currentTileIndex);
            Debug.Log("Selected Material: " + selectedMaterial.name);

            if (randomizedPattern[currentTileIndex] == selectedMaterial)
            {
                // Correct tile, add to stepped tiles
                steppedOnTiles.Add(GetTileRenderer(tileColor));
                currentTileIndex++;

                // Log the sequence of tiles stepped on
                LogSteppedOnTiles();

                if (currentTileIndex == randomizedPattern.Count)
                {
                    OpenDoor();
                }
            }
            else
            {
                // Incorrect tile, reset the tiles
                Debug.Log("Incorrect tile, sequence failed!");
                ResetSteppedOnTiles();
                currentTileIndex = 0;  // Restart the sequence
            }
        }
    }



    void ResetSteppedOnTiles()
    {
        // Reset all the stepped tiles to their original colors
        foreach (MeshRenderer tileRenderer in steppedOnTiles)
        {
            // Reset tile material to its original color
            if (tileRenderer.name.Contains("RedTile"))
                tileRenderer.material = redTileMaterial;
            else if (tileRenderer.name.Contains("BlueTile"))
                tileRenderer.material = blueTileMaterial;
            else if (tileRenderer.name.Contains("YellowTile"))
                tileRenderer.material = yellowTileMaterial;
        }

        // Clear the list of stepped-on tiles
        steppedOnTiles.Clear();
    }

    MeshRenderer GetTileRenderer(string tileColor)
    {
        // Returns the correct tile renderer based on the color
        switch (tileColor.ToLower())  // Make sure tileColor matches the exact names
        {
            case "redtile":
                return tiles.transform.Find("RedTile")?.GetComponent<MeshRenderer>();
            case "bluetile":
                return tiles.transform.Find("BlueTile")?.GetComponent<MeshRenderer>();
            case "yellowtile":
                return tiles.transform.Find("YellowTile")?.GetComponent<MeshRenderer>();
            default:
                Debug.LogError("Unknown tile color: " + tileColor);
                return null;
        }
    }


    void LogSteppedOnTiles()
    {
        if (steppedOnTiles == null || steppedOnTiles.Count == 0)
        {
            Debug.Log("No tiles stepped on yet.");
            return;
        }

        // Log the current sequence of stepped-on tiles
        string steppedOnSequence = "Stepped-on sequence: ";
        foreach (var tile in steppedOnTiles)
        {
            if (tile != null)  // Ensure the tile is not null
            {
                steppedOnSequence += tile.name + " -> ";
            }
        }
        Debug.Log(steppedOnSequence);
    }


    void OpenDoor()
    {
        // Open the door or trigger the next event
        Debug.Log("Pattern matched! The door opens.");

        // Deactivate the door (or perform whatever action opens the door)
        door.SetActive(false);

        // Disable the tile colliders after the door opens
        DisableTileColliders();

        PuzzleManager.instance.OnPuzzleCompletion(colorToRestore);
    }

    void DisableTileColliders()
    {
        // Disable colliders for all the tiles
        Transform redTile = tiles.transform.Find("RedTile");
        Transform blueTile = tiles.transform.Find("BlueTile");
        Transform yellowTile = tiles.transform.Find("YellowTile");

        if (redTile != null)
        {
            Collider redTileCollider = redTile.GetComponent<Collider>();
            if (redTileCollider != null)
            {
                redTileCollider.enabled = false;
            }
        }

        if (blueTile != null)
        {
            Collider blueTileCollider = blueTile.GetComponent<Collider>();
            if (blueTileCollider != null)
            {
                blueTileCollider.enabled = false;
            }
        }

        if (yellowTile != null)
        {
            Collider yellowTileCollider = yellowTile.GetComponent<Collider>();
            if (yellowTileCollider != null)
            {
                yellowTileCollider.enabled = false;
            }
        }
    }

}
