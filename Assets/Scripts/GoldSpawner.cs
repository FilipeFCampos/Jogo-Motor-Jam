using UnityEngine;

public class GoldSpawner : MonoBehaviour
{
    public GameObject[] goldPrefabs;         // Gold1 e Gold2
    public Transform[] spawnPoints;          // Posições nas paredes

    void Start()
    {
        SpawnGold();
    }

    void SpawnGold()
    {
        foreach (Transform point in spawnPoints)
        {
            int randomIndex = Random.Range(0, goldPrefabs.Length);
            GameObject chosenPrefab = goldPrefabs[randomIndex];

            Instantiate(chosenPrefab, point.position, Quaternion.identity);
        }
    }
}
