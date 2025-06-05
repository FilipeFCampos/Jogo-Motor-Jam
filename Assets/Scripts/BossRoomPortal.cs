using UnityEngine;

public class BossRoomPortal : MonoBehaviour
{
    public Transform bossSpawnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController inventory = other.GetComponent<PlayerController>();
            if (inventory != null && inventory.hasKey)
            {
                other.transform.position = bossSpawnPoint.position;
                Debug.Log("Player entrou na sala do boss!");
            }
            else
            {
                Debug.Log("Você precisa da chave para entrar!");
            }
        }
    }
}
