using System.Collections.Generic;
using UnityEngine;

public class SpeciesSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> species;

    [SerializeField] private BoxCollider2D spawnArea; // área para spawnear

    [SerializeField] private float spawnInterval = 1f;
    [SerializeField]  float totalTime;

    private bool _canSpawn = true;
    private float timer;

    void Start()
    {
        GameManager.Instance.OnStartScroll += ResumeSpawn;
        GameManager.Instance.OnStopScroll += PauseSpawn;
    }

    private void PauseSpawn()
    {
        _canSpawn = false;
    }

    private void ResumeSpawn()
    {
        _canSpawn = true;
    }

    void Update()
    {
        totalTime += Time.deltaTime;
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnRandom();
            timer = 0f;

            if (totalTime == 30 && spawnInterval >= 1)
            {
                spawnInterval -= 0.5f;
            }
            else if (totalTime == 60 && spawnInterval >= 1)
            {
                spawnInterval -= 0.5f;
            }
            else if (totalTime == 90 && spawnInterval >= 1)
            {
                spawnInterval -= 0.5f;
            }
            else if (totalTime > 120f && spawnInterval >= 1)
            {
                spawnInterval -= Time.deltaTime * 0.1f;

            }
        }
    }

    void SpawnRandom()
    {

        if (!_canSpawn) return;
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

    void OnDestroy()
    {
        GameManager.Instance.OnStartScroll -= ResumeSpawn;
        GameManager.Instance.OnStopScroll -= PauseSpawn;
    }

}
