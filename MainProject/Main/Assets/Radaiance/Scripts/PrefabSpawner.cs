


using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn; 
    public Transform playerCamera; 
    public float spawnDistance = 2.0f; 

    void Start()
    {
        SpawnPrefab();
    }

    void SpawnPrefab()
    {
        Vector3 spawnPosition = playerCamera.position + playerCamera.forward * spawnDistance;

        Instantiate(prefabToSpawn, spawnPosition, Quaternion.LookRotation(playerCamera.forward));
    }
}
