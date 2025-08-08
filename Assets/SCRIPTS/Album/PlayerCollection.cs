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

    // Devuelve true si la especie indicada ya fue coleccionada
    public bool HasCollected(SpeciesSO species)
    {
        if (species == null)
        {
            Debug.LogWarning("Tried to check a null species in HasCollected.");
            return false;
        }

        return collectedSpecies.Contains(species);
    }

    // Intenta añadir una nueva especie a la colección
    public bool AddSpecies(SpeciesSO species)
    {
        if (species == null)
        {
            Debug.LogWarning("Tried to add a null species to the collection.");
            return false;
        }

        if (collectedSpecies.Contains(species))
        {
            Debug.Log($"Species '{species.speciesName}' already collected.");
            return false;
        }

        bool added = collectedSpecies.Add(species);

        if (added)
        {
            Debug.Log($"Collected new species: {species.speciesName}");
        }

        return added;
    }
}

