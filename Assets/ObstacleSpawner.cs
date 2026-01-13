using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Spawn timing")]
    [SerializeField] private float spawnInterval = 5f;
    private float spawnTimer = 0f;

    [Header("Obstacle settings")]
    public GameObject[] obstaclePrefabs;
    public List<GameObject> obstacles = new List<GameObject>();

    void Start()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0)
        {
            Debug.LogWarning("ObstacleSpawner: no obstaclePrefabs assigned in Inspector.");
            return;
        }

        // use single interval and spawn immediately
        // (keeps the previous behavior of spawning on start)
        
        Spawn();
        spawnTimer = 0f;
    }

    void Update()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            Spawn();
            spawnTimer = 0f;
        }
    }

    private void Spawn()
    {
        int idx = Random.Range(0, obstaclePrefabs.Length);
        GameObject prefab = obstaclePrefabs[idx];
        Vector3 spawnPos = transform.position;

        GameObject instance = Instantiate(prefab, spawnPos, Quaternion.identity);
        obstacles.Add(instance);

        Debug.Log($"Spawned prefab '{(prefab ? prefab.name : "null")}' at {spawnPos}");

        // if the spawned object has an Obstacle component, pass speed if available
        var obs = instance.GetComponent<Obstacle>();
        if (obs != null)
        {
            // keep default speed from prefab or set if you want
        }
    }
}
