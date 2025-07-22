using UnityEngine;
using UnityEngine.UI;

public class PowerTimer : MonoBehaviour
{
    public Slider powerSlider; // Reference to the UI Slider component for the power timer
    private float maxPowerDuration = 25.1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (powerSlider != null)
        {
            powerSlider.maxValue = maxPowerDuration;
            powerSlider.value = maxPowerDuration;
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
        }
    }

    void ResetTimer()
    {
        powerSlider.value = maxPowerDuration;
    }
}
