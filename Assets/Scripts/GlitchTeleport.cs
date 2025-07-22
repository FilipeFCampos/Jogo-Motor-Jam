using UnityEngine;

public class GlitchTeleport : MonoBehaviour
{
    public Transform teleportTarget; // Defina isso no Inspector: ponto para onde o player vai ser teleportado
    private bool playerInZone = false;
    private GameObject player;

    void Update()
    {
        if (playerInZone && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.G))
        {
            TeleportPlayer();
        }
    }

    private void TeleportPlayer()
    {
        if (player != null && teleportTarget != null)
        {
            player.transform.position = teleportTarget.position;
            Debug.Log("Player teleportado para o Glitch Boss.");
        }
        else
        {
            Debug.LogWarning("Teleport falhou. Player ou teleportTarget não atribuídos.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            player = null;
        }
    }
}
