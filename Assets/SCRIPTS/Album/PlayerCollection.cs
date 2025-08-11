using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestiona las especies que el jugador ha coleccionado
/// y permite consultar o agregar nuevas especies.
/// </summary>
public class PlayerCollection : MonoBehaviour
{
    private HashSet<SpeciesSO> collectedSpecies = new HashSet<SpeciesSO>(); // Conjunto de especies únicas recolectadas

    // Propiedad de solo lectura para acceder a la colección
    public IReadOnlyCollection<SpeciesSO> CollectedSpecies => collectedSpecies;

    public static PlayerCollection Instance { get; private set; }

    // Devuelve true si la especie indicada ya fue coleccionada
    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }

    public bool HasCollected(SpeciesSO species)
    {
        if (species == null)
        {
            Debug.LogWarning("Tried to check a null species in HasCollected.");
            return false;
        }

        return collectedSpecies.Contains(species);
    }

    void Start()
    {
        GameManager.Instance.OnFishCatch += AddSpecies;
    }

    // Intenta añadir una nueva especie a la colección
    public void AddSpecies(SpeciesSO species)
    {
        if (species == null)
        {
            Debug.LogWarning("Tried to add a null species to the collection.");
            return;
        }

        if (collectedSpecies.Contains(species))
        {
            Debug.Log($"Species '{species.speciesID}' already collected.");
            return;
        }

        bool added = collectedSpecies.Add(species);

        if (added)
        {
            Debug.Log($"Collected new species: {species.speciesID}");
        }

        
    }
}

