using UnityEngine;

/// <summary>
/// Representa una especie coleccionable en el mundo.
/// Al ser recogida por el jugador, se agrega a su colección
/// y el objeto desaparece.
/// </summary>
public class SpeciesCollectable : MonoBehaviour
{
    [SerializeField] private SpeciesSO speciesData; // Datos de la especie (ScriptableObject)

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica si el objeto que entra en el trigger es el jugador
        PlayerCollection player = other.GetComponent<PlayerCollection>();
        if (player == null || speciesData == null) return;

        // Si el jugador la agrega con éxito, mostrar mensaje y destruir el objeto
        if (player.AddSpecies(speciesData))
        {
            Debug.Log($"Recolectaste: {speciesData.speciesName}");
            Destroy(gameObject); 
        }
    }
}

