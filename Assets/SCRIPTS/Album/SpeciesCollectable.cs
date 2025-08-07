using UnityEngine;

public class SpeciesCollectable : MonoBehaviour
{
    [SerializeField] private SpeciesSO speciesData;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Asegurate que el que colisiona tenga el PlayerCollection
        PlayerCollection player = other.GetComponent<PlayerCollection>();
        if (player == null || speciesData == null) return;

        if (player.AddSpecies(speciesData))
        {
            Debug.Log($"Recolectaste: {speciesData.speciesName}");
            Destroy(gameObject); 
        }
    }
}

