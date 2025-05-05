using UnityEngine;

public class UnderwaterEffectsController : MonoBehaviour
{
    public Light directionalLight;  // Reference to directional light (sunlight)
    public Color underwaterLightColor = new Color(0.2f, 0.5f, 0.8f); // Light color underwater
    public float underwaterLightIntensity = 0.2f;  // Light intensity underwater
    private Color originalLightColor;
    private float originalLightIntensity;

    // Reference to the player's VR Camera (inside the VR Rig)
    public GameObject playerCamera; // This will be the "Main Camera" in your VR Rig

    // AudioSource references for sounds
    public AudioSource splashSound; // Splash sound effect (for entering underwater)
    public AudioSource underwaterAmbience; // Underwater ambient sound (looped)
    public AudioSource windAmbience; // Wind sound for flying over the loch
    public AudioSource leavingWaterSound; // Sound for leaving the underwater zone

    void Start()
    {
        // Store original lighting settings for restoring when leaving the water
        if (directionalLight != null)
        {
            originalLightColor = directionalLight.color;
            originalLightIntensity = directionalLight.intensity;
        }

        // Start the wind ambience immediately when the game starts
        if (windAmbience != null && !windAmbience.isPlaying)
        {
            windAmbience.Play();  // Start playing the wind sound from the beginning
            windAmbience.loop = true;  // Ensure it loops
            Debug.Log("Wind ambience started.");
        }
    }

    // Called when another object enters the trigger zone
    void OnTriggerEnter(Collider other)
    {
        // Debug log to check what object is entering the trigger zone
        Debug.Log("OnTriggerEnter triggered by: " + other.gameObject.name);

        // Check if the object that entered the trigger is the Main Camera (Player Camera)
        if (other.gameObject == playerCamera)
        {
            Debug.Log("Main Camera entered the underwater zone!");
            EnableUnderwaterEffects(true);
            PlaySplashSound();

            // Stop wind sound when underwater
            if (windAmbience != null && windAmbience.isPlaying)
            {
                windAmbience.Pause();  // Pause wind sound when underwater
                Debug.Log("Wind ambience paused.");
            }
        }
        else
        {
            Debug.Log("Something else entered the underwater zone: " + other.gameObject.name);
        }
    }

    // Called when another object exits the trigger zone
    void OnTriggerExit(Collider other)
    {
        // Debug log to check what object is exiting the trigger zone
        Debug.Log("OnTriggerExit triggered by: " + other.gameObject.name);

        // Check if the object that exited the trigger is the Main Camera (Player Camera)
        if (other.gameObject == playerCamera)
        {
            Debug.Log("Main Camera exited the underwater zone!");
            EnableUnderwaterEffects(false);
            PlayLeavingWaterSound();

            // Resume the wind sound when leaving underwater
            if (windAmbience != null && !windAmbience.isPlaying)
            {
                windAmbience.Play();  // Resume the wind sound when leaving underwater
                Debug.Log("Wind ambience resumed.");
            }
        }
        else
        {
            Debug.Log("Something else exited the underwater zone: " + other.gameObject.name);
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
            RenderSettings.fogDensity = 0.001f;  // Adjust this value for fog density (you can make it thicker or thinner)
            Debug.Log("Fog Enabled!");

            // Play underwater ambience
            if (underwaterAmbience != null && !underwaterAmbience.isPlaying)
            {
                underwaterAmbience.Play();  // Start the looped underwater sound
                Debug.Log("Underwater ambience started.");
            }
        }
        else
        {
            RenderSettings.fog = false;  // Disable fog
            Debug.Log("Fog Disabled!");

            // Stop the underwater ambience when exiting the water
            if (underwaterAmbience != null && underwaterAmbience.isPlaying)
            {
                underwaterAmbience.Stop();  // Stop the underwater sound loop
                Debug.Log("Underwater ambience stopped.");
            }
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

    // Play the splash sound when entering the underwater zone
    void PlaySplashSound()
    {
        if (splashSound != null)
        {
            splashSound.Play();  // Play the splash sound effect once
            Debug.Log("Splash sound played.");
        }
    }

    // Play the leaving water sound when exiting the underwater zone
    void PlayLeavingWaterSound()
    {
        if (leavingWaterSound != null)
        {
            leavingWaterSound.Play();  // Play the leaving water sound effect once
            Debug.Log("Leaving water sound played.");
        }
    }
}
