using UnityEngine;

public class UnderwaterEffectsController : MonoBehaviour
{
    public Light directionalLight;
    public Color underwaterLightColor = new Color(0.2f, 0.5f, 0.8f); // Light color underwater
    public float underwaterLightIntensity = 0.4f;
    private Color originalLightColor;
    private float originalLightIntensity;

    public GameObject playerCamera;

    void Start()
    {
        // Store original lighting settings for restoring when leaving the water
        if (directionalLight != null)
        {
            originalLightColor = directionalLight.color;
            originalLightIntensity = directionalLight.intensity;
        }
    }

    // Called when another object enters the trigger zone
    void OnTriggerEnter(Collider other)
    {

        // Check if the object that entered the trigger is the Main Camera
        if (other.gameObject == playerCamera)
        {
            Debug.Log("main Camera entered the underwater zone");
            EnableUnderwaterEffects(true);
        }
        else
        {
            Debug.Log("something else entered the underwater zone: " + other.gameObject.name);
        }
    }

    // Called when another object exits the trigger zone
    void OnTriggerExit(Collider other)
    {

        // Check if the object that exited the trigger is the Main Camera (Player Camera)
        if (other.gameObject == playerCamera)
        {
            Debug.Log("main Camera exited the underwater zone");
            EnableUnderwaterEffects(false);
        }
        else
        {
            Debug.Log("something else exited the underwater zone: " + other.gameObject.name);
        }
    }

    // Method to enable/disable underwater effects (fog and lighting)
    void EnableUnderwaterEffects(bool enable)
    {
        // Control fog through RenderSettings
        if (enable)
        {
            RenderSettings.fog = true;  // Enable fog
            RenderSettings.fogColor = new Color(0.29f, 0.35f, 0.33f); // Underwater fog color
            RenderSettings.fogDensity = 0.0007f;
            Debug.Log("Fog Enabled!");
        }
        else
        {
            RenderSettings.fog = false;  // Disable fog
            Debug.Log("Fog Disabled!");
        }

        // Change lighting for underwater environment
        if (directionalLight != null)
        {
            if (enable)
            {
                directionalLight.color = underwaterLightColor;
                directionalLight.intensity = underwaterLightIntensity;
                Debug.Log("Lighting changed to underwater settings.");
            }
            else
            {
                directionalLight.color = originalLightColor;
                directionalLight.intensity = originalLightIntensity;
                Debug.Log("Lighting returned to original settings.");
            }
        }
    }
}
