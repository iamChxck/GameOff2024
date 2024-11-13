using UnityEngine;

public class ChromaticPair : MonoBehaviour
{
    public GameObject door; // Reference to the door
    public Transform tiles; // Reference to the tiles container (RedTile, YellowTile, BlueTile)
    public Transform objects; // Reference to the objects container (RedCube, YellowCube, BlueCube)

    public bool redCubeOnTile = false, yellowCubeOnTile = false, blueCubeOnTile = false;

    private void Start()
    {
        // Initialization - no changes needed here
    }

    private void Update()
    {
        // Check if all cubes are on their matching tiles
        if (redCubeOnTile && yellowCubeOnTile && blueCubeOnTile)
        {
            OpenDoor();
        }
    }

    public void UpdateCubeOnTile(string tileColor, bool isOnTile)
    {
        // Update the status of the specific cube based on the tileColor
        if (tileColor == "Red")
            redCubeOnTile = isOnTile;
        else if (tileColor == "Yellow")
            yellowCubeOnTile = isOnTile;
        else if (tileColor == "Blue")
            blueCubeOnTile = isOnTile;
    }

    private void OpenDoor()
    {
        // Door opening logic (could be opening a door, animating, etc.)
        door.SetActive(false);
    }
}
