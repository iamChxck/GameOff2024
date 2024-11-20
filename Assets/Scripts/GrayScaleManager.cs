using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrayscaleManager : MonoBehaviour
{
    public static GrayscaleManager Instance { get; private set; } // Singleton instance

    [Range(0, 1)]
    public float grayscaleAmount = 1f; // Start with full grayscale
    public float transitionDuration = 1f; // Duration for gradual transitions

    private List<Material> targetMaterials; // List of Materials to modify
    private Coroutine currentCoroutine;

    public List<string> restoredColors; // List of material names to exclude from grayscale

    private void Awake()
    {
        // Implement Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        Instance = this; // Set the instance
        DontDestroyOnLoad(gameObject); // Optional: Keep the instance alive across scenes

        LoadMaterialsFromResources();
    }

    private void Start()
    {
        // Set grayscaleAmount to 1f to apply full grayscale at the start
        grayscaleAmount = 1f;

        // Ensure that grayscale is applied to all materials at the start
        UpdateGrayscaleAmount();
    }

    void Update()
    {
        // Update the grayscale amount in each material
        UpdateGrayscaleAmount();
    }

    // Gradually apply grayscale effect
    public void GraduallyApplyGrayscale()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(ApplyGrayscale(1f));
    }

    // Gradually restore color
    public void GraduallyRestoreColor()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(ApplyGrayscale(0f));
    }

    // Instant grayscale effect
    public void InstantGrayscale()
    {
        grayscaleAmount = 1f; // Set grayscale amount to full
        UpdateGrayscaleAmount();
    }

    // Instant restoration of color
    public void InstantRestoreColor()
    {
        grayscaleAmount = 0f; // Set grayscale amount to none
        UpdateGrayscaleAmount();
    }

    private IEnumerator ApplyGrayscale(float targetGrayscale)
    {
        float initialGrayscale = grayscaleAmount;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            grayscaleAmount = Mathf.Lerp(initialGrayscale, targetGrayscale, elapsedTime / transitionDuration);
            UpdateGrayscaleAmount();
            yield return null; // Wait for the next frame
        }

        grayscaleAmount = targetGrayscale; // Ensure the target value is set
        UpdateGrayscaleAmount();
    }

    private void UpdateGrayscaleAmount()
    {
        foreach (var material in targetMaterials)
        {
            if (material != null)
            {
                // Skip materials in the restoredColors list
                if (restoredColors.Contains(material.name))
                {
                    if (material.HasProperty("_GrayscaleAmount"))
                        material.SetFloat("_GrayscaleAmount", 0f); // No grayscale for restored materials
                    continue;
                }

                // Apply grayscale effect
                if (material.HasProperty("_GrayscaleAmount"))
                    material.SetFloat("_GrayscaleAmount", grayscaleAmount);
            }
        }
    }

    private void LoadMaterialsFromResources()
    {
        targetMaterials = new List<Material>();

        // Load all materials in the entire Resources folder
        Material[] allMaterials = Resources.LoadAll<Material>("");

        if (allMaterials.Length == 0)
        {
            Debug.LogWarning("No materials found in the Resources folder.");
            return;
        }

        // Filter materials to include only those in the Materials folder or its subfolders
        foreach (var material in allMaterials)
        {
            string materialPath = UnityEditor.AssetDatabase.GetAssetPath(material); // This line works only in the editor
            if (materialPath.Contains("/Materials/")) // Ensure it's in the Materials folder or its subfolders
            {
                targetMaterials.Add(material);
            }
        }

        Debug.Log($"Loaded {targetMaterials.Count} materials from Resources/Materials and subfolders.");
    }

}
