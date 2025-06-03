using UnityEngine;
using System.Collections;

public class SlimeSpawner : MonoBehaviour
{
    [Header("Slime Prefabs")]
    public GameObject[] slimePrefabs;

    [Header("Spawn Settings")]
    public float timeBetweenWaves = 6f;
    public float delayBetweenEachSlime = 0.6f;
    public int minSlimesPerWave = 1;
    public int maxSlimesPerWave = 4;
    public static int slimeCount = 0;
    public int maxSlimes = 10;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            int slimeCount = Random.Range(minSlimesPerWave, maxSlimesPerWave + 1);

            for (int i = 0; i < slimeCount; i++)
            {
                SpawnSlime();
                yield return new WaitForSeconds(delayBetweenEachSlime);
            }

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private void SpawnSlime()
    {
        if (slimePrefabs.Length == 0) return;

        int randomIndex = Random.Range(0, slimePrefabs.Length);
        GameObject slime = slimePrefabs[randomIndex];

        Vector2 randomOffset = Random.insideUnitCircle * 2f; // Distância aleatória
        Vector3 spawnPosition = transform.position + (Vector3)randomOffset;

        Instantiate(slime, spawnPosition, Quaternion.identity);
    }
}
