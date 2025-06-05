using UnityEngine;

public class TeleportOnTouch : MonoBehaviour
{
    public Transform targetPosition; // Para onde o player ser� movido

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = targetPosition.position;
            Debug.Log("Teleportado para nova sala.");
        }
    }
}