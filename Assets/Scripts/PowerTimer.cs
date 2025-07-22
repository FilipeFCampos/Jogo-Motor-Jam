using UnityEngine;
using UnityEngine.UI;

public class PowerTimer : MonoBehaviour
{
    public Slider powerSlider; // Reference to the UI Slider component for the power timer
    private float maxPowerDuration = 25.1f;
    public PlayerController playerController; // Reference to the PlayerController script
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("[PowerTimer] PlayerController reference is missing.");
            return;
        }
        if (powerSlider != null)
        {
            ResetTimer(0f);
            Debug.Log("[PowerTimer] Slider assigned!");
        }
        else
        {
            Debug.LogError("[PowerTimer] powerSlider is not assigned.");
        }
    }

    void UpdateTimer()
    {
        powerSlider.value -= Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (powerSlider.value > 0)
        {
            UpdateTimer();
        }
        else
        {
            powerSlider.value = 0f;
            playerController.ResetAbilities(); // Reset the player's power state
            Debug.Log("[PowerTimer] Power timer has reached zero. Resetting abilities.");
        }
    }

    public void ChangeColor(Color newColor)
    {
        if (powerSlider != null)
        {
            powerSlider.fillRect.GetComponent<Image>().color = newColor;
        }
        else
        {
            Debug.LogError("[PowerTimer] powerSlider is not assigned.");
        }
    }

    public void ResetTimer(float powerDuration)
    {
        powerSlider.maxValue = powerDuration;
        powerSlider.value = powerDuration;
        Debug.Log("[PowerTimer] Timer reset to: " + powerDuration);
    }
}
