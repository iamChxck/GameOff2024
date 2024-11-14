using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class OutlineEffect : MonoBehaviour
{
    private Renderer objectRenderer;
    private Material originalMaterial; // Store the original material
    private Material outlineMaterial; // Outline material

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();

        // Save the original material
        originalMaterial = objectRenderer.material;

        // Load the outline shader from Resources/Materials
        Shader outlineShader = Resources.Load<Shader>("Shaders/OutlineEffect");

        // Check if the shader was loaded successfully
        if (outlineShader != null)
        {
            // Create the outline material using the loaded shader
            outlineMaterial = new Material(outlineShader);
            // Set default outline color and width
            outlineMaterial.SetColor("_OutlineColor", Color.yellow); // Set the outline color
            outlineMaterial.SetFloat("_OutlineWidth", 0.03f); // Set the outline width
        }
        else
        {
            Debug.LogError("OutlineEffect shader not found in Resources/Materials");
        }
    }

    public void EnableOutline()
    {
        objectRenderer.material = outlineMaterial; // Switch to outline material
    }

    public void DisableOutline()
    {
        objectRenderer.material = originalMaterial; // Restore original material
    }
}
