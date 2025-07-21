using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public Slider healthBar; // Reference to the UI Slider component for the health bar
    private PlayerController playerController; // Reference to the PlayerController script
    
    public void SetPlayer(PlayerController player)
    {
        playerController = player;

        if (healthBar != null)
        {
            healthBar.maxValue = playerController.GetMaxHealth();
            healthBar.value = (float)playerController.health;
            Debug.Log("[HealthBarController] Player assigned!");
        }
        else
        {
            Debug.LogError("[HealthBarController] healthBar is not assigned.");
        }
    }

    void Update()
    {
        if (playerController != null)
        {
            healthBar.maxValue = playerController.GetMaxHealth();
            healthBar.value = (float)playerController.health;
        }
        else
        {
            Debug.LogError("[HealthBarController] PlayerController reference is missing in HealthBarController.");
        }
    }
}
