using UnityEngine;

public class BossRoomPortal : MonoBehaviour
{
    public Transform bossRoomSpawnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = bossRoomSpawnPoint.position;
        }
    }
}
