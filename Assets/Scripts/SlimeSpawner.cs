using UnityEngine;

public class SlimeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] slimePrefabs;  // 5 prefabs
    [SerializeField] private Transform[] spawnPoints;     // Locais de spawn

    void Start()
    {
        SpawnAll();
    }

    public void SpawnAll()
    {
        foreach (Transform point in spawnPoints)
        {
            int index = Random.Range(0, slimePrefabs.Length);
            Instantiate(slimePrefabs[index], point.position, Quaternion.identity);
        }
    }
}
