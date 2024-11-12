using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlatformManager : MonoBehaviour
{
    public PlayerController playerController;

    public GameObject[] platforms;
    
    public Material normalIndicatorMaterial;
    public Material zoomedIndicatorMaterial;

    int pathPatternSelected = 0;

    // Pre-determined pattern. Can be changed.
    int[][] possiblePaths = new int[][]
    {
        new int[] { 0,4,5,6,10,14,13,17 },
        new int[] { 1,5,4,8,12,13,14,15,19 },
        new int[] { 2,6,7,11,15,14,18 },
        new int[] { 3,7,11,10,9,8,12,16 }
    };

    // Start is called before the first frame update
    void Start()
    {
        RandomizePlatformPattern();
    }

    void RandomizePlatformPattern()
    {
        GeneratePlatformPattern(pathPatternSelected = Random.Range(0, possiblePaths.GetLength(0)));
    }

    void GeneratePlatformPattern(int index)
    {
        for (int i = 0; i < possiblePaths[index].Length; i++)
        {
            // Enables the meshcollider for a walkable path
            platforms[possiblePaths[index][i]].GetComponent<MeshCollider>().enabled = true;
        }
    }

    private void Update()
    {
        CheckIfZoomed();
    }

    void CheckIfZoomed()
    {
        if (playerController.isZoomed)
        {
            for (int i = 0; i < possiblePaths[pathPatternSelected].Length; i++)
            {
                platforms[possiblePaths[pathPatternSelected][i]].GetComponent<Renderer>().material = zoomedIndicatorMaterial;
            }
            return;
        }

        for (int i = 0; i < possiblePaths[pathPatternSelected].Length; i++)
        {
            platforms[possiblePaths[pathPatternSelected][i]].GetComponent<Renderer>().material = normalIndicatorMaterial;
        }
    }
}
