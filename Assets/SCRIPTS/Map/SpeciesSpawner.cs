using System.Collections.Generic;
using UnityEngine;

public class SpeciesSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> species; 

    [SerializeField] private BoxCollider2D spawnArea; // área para spawnear

    [SerializeField] private float spawnInterval = 1f;


    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnRandom();
            timer = 0f;
        }
    }

        void SpawnRandom()
    {
        if (species.Count == 0 || spawnArea == null) return;

        // Sacar centro y tamaño del collider
        Bounds bounds = spawnArea.bounds;

        // Elegir una posición aleatoria dentro
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);
        Vector3 spawnPos = new Vector3(randomX, randomY, 0f);

        // Instanciar un prefab aleatorio
        int index = Random.Range(0, species.Count);
        Instantiate(species[index], spawnPos, Quaternion.identity);
    }
}
