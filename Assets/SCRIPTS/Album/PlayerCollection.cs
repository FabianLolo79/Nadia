using System.Collections.Generic;
using UnityEngine;

public class PlayerCollection : MonoBehaviour
{
    private HashSet<SpeciesSO> collectedSpecies = new HashSet<SpeciesSO>();

    // Exponer como solo lectura
    public IReadOnlyCollection<SpeciesSO> CollectedSpecies => collectedSpecies;

    public bool HasCollected(SpeciesSO species)
    {
        if (species == null)
        {
            Debug.LogWarning("Tried to check a null species in HasCollected.");
            return false;
        }

        return collectedSpecies.Contains(species);
    }

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

