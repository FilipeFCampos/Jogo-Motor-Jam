using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab; // Reference to the player prefab
    [SerializeField] private Transform spawnPoint; // Point where the player will be spawned
                                                   // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private HealthBarController healthBarController; // Reference to the health bar controller
    public GameObject currentPlayer;
    void Start()
    {
        SpawnPlayer(); // Spawn the player at the start of the game
    }

    // Update is called once per frame

    void SpawnPlayer()
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer); // Destroy the current player if it exists
        }

        currentPlayer = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        healthBarController.SetPlayer(currentPlayer.GetComponent<PlayerController>());

        // Informa a câmera sobre o novo jogador
        CameraFollow camFollow = FindFirstObjectByType<CameraFollow>();
        if (camFollow != null)
        {
            camFollow.SetTarget(currentPlayer.transform);
        }
    }

    
    void RespawnPlayer()
    {
        currentPlayer.transform.position = spawnPoint.position; // Move the player to the spawn point
        currentPlayer.SetActive(true); // Reactivate the player
        healthBarController.SetPlayer(currentPlayer.GetComponent<PlayerController>());
    }

    public void PlayerDied()
    {
        Debug.Log("Player has died. Respawning...");
        RespawnPlayer(); // Respawn the player when they die
    }

    void Update()
    {
        
    }
}
