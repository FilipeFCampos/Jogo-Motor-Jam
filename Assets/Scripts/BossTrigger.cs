using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public GameObject bossPrefab;       // O prefab do boss
    public Transform spawnPoint;        // Onde o boss vai aparecer
    private bool hasSpawned = false;    // Para não spawnar várias vezes

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasSpawned && other.CompareTag("Player"))
        {
            Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
            hasSpawned = true;
        }
    }
}
