using System;
using UnityEngine;

public class TransformController : MonoBehaviour
{
    GameObject player;
    public enum Abilities { BLACK, BLUE, GREEN, RED } 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<GameManager>().currentPlayer.gameObject;
        if (player == null)
        {
            Debug.LogError("Player not found in GameManager.");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AbsorveAbility(Abilities ability)
    {
        
        switch (ability)
        {
            case Abilities.BLACK:
                Debug.Log("Absorbed BLACK ability.");
                break;
            case Abilities.BLUE:
                Debug.Log("Absorbed BLUE ability.");
                break;
            case Abilities.GREEN:
                Debug.Log("Absorbed GREEN ability.");
                break;
            case Abilities.RED:
                Debug.Log("Absorbed RED ability.");
                break;
            default:
                Debug.LogError("Unknown ability type: " + ability);
                break;
        }
    }
}
