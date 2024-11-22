using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrayscaleManager : MonoBehaviour
{
    public static GrayscaleManager Instance { get; private set; } // Singleton instance

    public List<GameObject> targetGameObjects; // List of GameObjects to apply grayscale
    [Range(0, 1)]
    public float grayscaleAmount = 1f; // Start with no grayscale effect
    public float transitionDuration = 1f; // Duration for gradual transitions

    private List<Renderer> targetRenderers; // List of Renderers to modify
    private Coroutine currentCoroutine;

    public List<string> restoredColors;

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

        // Initialize the list of renderers
        targetRenderers = new List<Renderer>();

        // Find and store the Renderer for each target GameObject or its children
        foreach (var gameObject in targetGameObjects)
        {
            var renderer = gameObject.GetComponent<Renderer>(); // Try to get Renderer on GameObject
            if (renderer == null)
            {
                // If not found, check in children
                renderer = gameObject.GetComponentInChildren<Renderer>();
            }

            if (renderer != null)
            {
                targetRenderers.Add(renderer);
            }
            else
            {
                Debug.LogWarning($"No Renderer found on {gameObject.name} or its children.");
            }
        }
    }

    private void Start()
    {
        // Set grayscaleAmount to 1f to apply full grayscale at the start
        grayscaleAmount = 1f;

        // Ensure that grayscale is applied to all objects at the start
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
        foreach (var renderer in targetRenderers)
        {
            if (renderer != null)
            {
                // Check if the GameObject's tag is in the excludedTags list
                if (restoredColors.Contains(renderer.gameObject.tag))
                {
                    // Set grayscale to 0 for objects with tags in the excludedTags list
                    Material material = renderer.material;
                    if (material.HasProperty("_GrayscaleAmount"))
                        material.SetFloat("_GrayscaleAmount", 0f);
                    continue; // Skip applying the general grayscale effect
                }

                // Apply grayscale to the renderer as usual
                Material mat = renderer.material;

                // Set the BaseColor to red if no texture is assigned
                if (mat.HasProperty("_BaseColor") && !mat.HasProperty("_MainTex"))
                    mat.SetColor("_BaseColor", Color.red);

                // Set the _GrayscaleAmount property if it exists on the material
                if (mat.HasProperty("_GrayscaleAmount"))
                    mat.SetFloat("_GrayscaleAmount", grayscaleAmount);
            }
        }
    }
}
