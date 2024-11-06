using System.Collections;
using UnityEngine;

public class DayNightCycleManager : MonoBehaviour
{
    // Singleton instance
    public static DayNightCycleManager Instance { get; private set; }

    public Light directionalLight; // The directional light to control
    public Color dayLightColor = Color.white; // Daylight color
    public Color nightLightColor = new Color(0.2f, 0.2f, 0.5f); // Nightlight color (moonlight)
    public float dayIntensity = 1f; // Intensity of the sunlight
    public float nightIntensity = 0.5f; // Intensity of the moonlight
    public float transitionDuration = 2f; // Duration of the transition between day and night
    public float rotationSpeed = 10f; // Speed of light rotation

    public bool isDaytime = true;
    public bool isTransitioning = false; // Flag to check if a transition is happening

    // Rotation variables
    private float currentRotationAngle = 1f; // Start at the minimum angle
    private float rotationDirection = 1f; // 1 for positive, -1 for negative

    private void Awake()
    {
        // Singleton logic
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
    }

    void Start()
    {
        // Set the initial lighting to daytime
        SetDayLighting();
    }

    void Update()
    {
        // Toggle between day and night when the user presses the 'T' key
        if (Input.GetKeyDown(KeyCode.T) && !isTransitioning)
        {
            if (isDaytime)
            {
                StartCoroutine(ChangeToNight());
            }
            else
            {
                StartCoroutine(ChangeToDay());
            }
        }

        // Rotate the directional light to simulate sun movement during transitions
        if (isTransitioning)
        {
            RotateDirectionalLight();
        }
    }

    private void RotateDirectionalLight()
    {
        // Rotate the light based on the current rotation angle and direction
        currentRotationAngle += rotationDirection * rotationSpeed * Time.deltaTime;

        // Clamp the rotation angle between 1 and 179 degrees
        if (currentRotationAngle >= 179f)
        {
            currentRotationAngle = 179f; // Clamp to max
            rotationDirection = -1f; // Reverse direction
        }
        else if (currentRotationAngle <= 1f)
        {
            currentRotationAngle = 1f; // Clamp to min
            rotationDirection = 1f; // Reverse direction
        }

        // Apply the rotation to the directional light
        directionalLight.transform.localRotation = Quaternion.Euler(currentRotationAngle, -110, 0);
    }

    private void SetDayLighting()
    {
        if (directionalLight != null)
        {
            directionalLight.color = dayLightColor;
            directionalLight.intensity = dayIntensity;
            isDaytime = true;
        }
    }

    private void SetNightLighting()
    {
        if (directionalLight != null)
        {
            directionalLight.color = nightLightColor;
            directionalLight.intensity = nightIntensity;
            isDaytime = false;
        }
    }

    public IEnumerator ChangeToNight()
    {
        isTransitioning = true; // Set transitioning flag
        float elapsedTime = 0f;
        Color initialColor = directionalLight.color;
        float initialIntensity = directionalLight.intensity;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;

            // Interpolate the color and intensity
            directionalLight.color = Color.Lerp(initialColor, nightLightColor, t);
            directionalLight.intensity = Mathf.Lerp(initialIntensity, nightIntensity, t);

            yield return null; // Wait for the next frame
        }

        SetNightLighting(); // Ensure the final values are set
        isTransitioning = false; // Reset transitioning flag
    }

    public IEnumerator ChangeToDay()
    {
        isTransitioning = true; // Set transitioning flag
        float elapsedTime = 0f;
        Color initialColor = directionalLight.color;
        float initialIntensity = directionalLight.intensity;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;

            // Interpolate the color and intensity
            directionalLight.color = Color.Lerp(initialColor, dayLightColor, t);
            directionalLight.intensity = Mathf.Lerp(initialIntensity, dayIntensity, t);

            yield return null; // Wait for the next frame
        }

        SetDayLighting(); // Ensure the final values are set
        isTransitioning = false; // Reset transitioning flag
    }
}
