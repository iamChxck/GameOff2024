using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class OutlineEffect : MonoBehaviour
{
    private Renderer objectRenderer;
    private Material originalMaterial; // Store the shared original material
    private Material outlineMaterial; // Outline material (shared)

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();

        // Save the shared original material
        originalMaterial = objectRenderer.sharedMaterial;

        // Load the outline shader from Resources/Shaders
        Shader outlineShader = Resources.Load<Shader>("Shaders/OutlineEffect");

        // Check if the shader was loaded successfully
        if (outlineShader != null)
        {
            // Create the outline material using the loaded shader
            outlineMaterial = new Material(outlineShader)
            {
                name = "OutlineMaterial (Instance)"
            };

            // Set default outline color and width
            outlineMaterial.SetColor("_OutlineColor", Color.yellow); // Set the outline color
            outlineMaterial.SetFloat("_OutlineWidth", 0.03f); // Set the outline width
        }
        else
        {
            Debug.LogError("OutlineEffect shader not found in Resources/Shaders");
        }
    }

    public void EnableOutline()
    {
        // Temporarily assign the shared outline material
        objectRenderer.sharedMaterial = outlineMaterial;
    }

    public void DisableOutline()
    {
        // Restore the shared original material
        objectRenderer.sharedMaterial = originalMaterial;
    }
}
