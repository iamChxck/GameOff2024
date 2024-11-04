using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrayscaleManager: MonoBehaviour
{
    public List<GameObject> targetGameObjects; // List of GameObjects to apply grayscale
    [Range(0, 1)]
    public float grayscaleAmount = 0f; // Start with no grayscale effect
    public float transitionDuration = 2f; // Duration for gradual transitions

    private List<Renderer> targetRenderers; // List of Renderers to modify
    private Coroutine currentCoroutine;

    void Awake()
    {
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

    void Update()
    {
        // Check for input keys for gradual grayscale and restore
        if (Input.GetKeyDown(KeyCode.Q)) // Press Q to apply grayscale gradually
        {
            GraduallyApplyGrayscale();
        }

        if (Input.GetKeyDown(KeyCode.W)) // Press W to restore color gradually
        {
            GraduallyRestoreColor();
        }

        // Check for input keys for instant grayscale and restore
        if (Input.GetKeyDown(KeyCode.A)) // Press A for instant grayscale
        {
            InstantGrayscale();
        }

        if (Input.GetKeyDown(KeyCode.S)) // Press S for instant color restoration
        {
            InstantRestoreColor();
        }

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
                renderer.material.SetFloat("_GrayscaleAmount", grayscaleAmount);
            }
        }
    }
}
