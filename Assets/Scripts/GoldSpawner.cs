using UnityEngine;

public class GoldSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] goldPrefabs; // Array de prefabs (Gold1 e Gold2)
    [SerializeField] private int numberOfGolds = 20;
    [SerializeField] private Vector2 spawnAreaMin;
    [SerializeField] private Vector2 spawnAreaMax;

    void Start()
    {
        for (int i = 0; i < numberOfGolds; i++)
        {
            // Escolhe um prefab aleatório do array
            GameObject randomGoldPrefab = goldPrefabs[Random.Range(0, goldPrefabs.Length)];
            
            Vector2 randomPosition = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );

            Instantiate(randomGoldPrefab, randomPosition, Quaternion.identity, transform);
        }
    }
}