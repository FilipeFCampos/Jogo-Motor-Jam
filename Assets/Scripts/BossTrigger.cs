using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public GameObject bossPrefab;       // O prefab do boss
    public Transform spawnPoint;        // Onde o boss vai aparecer
    private bool hasSpawned = false;    // Para n�o spawnar v�rias vezes
    public GameObject holeTrigger;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasSpawned && other.CompareTag("Player"))
        {
            holeTrigger.SetActive(true);
            Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
            hasSpawned = true;
        }
    }
}
