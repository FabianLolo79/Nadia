using UnityEngine;

/// <summary>
/// Representa una especie coleccionable en el mundo.
/// Al ser recogida por el jugador, se agrega a su colección
/// y el objeto desaparece.
/// </summary>

public class SpeciesCollectable : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer; // Muestra la imagen en pixel art
    [SerializeField] private SpeciesSO speciesData; // Datos de la especie

    private void Awake()
    {
        if (speciesData == null)
        {
            Debug.LogWarning($"[SpeciesCollectable] No se asignó SpeciesSO en {gameObject.name}");
            return;
        }

        if (spriteRenderer != null)
        {
            // Asignar pixel art al sprite
            spriteRenderer.sprite = speciesData.speciesImagePixel;
        }
        else
        {
            Debug.LogWarning($"[SpeciesCollectable] No se asignó SpriteRenderer en {gameObject.name}");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerCollection player = other.GetComponent<PlayerCollection>();
        if (player == null || speciesData == null) return;

        if (player.AddSpecies(speciesData))
        {
            Debug.Log($"Recolectaste: {speciesData.speciesName}");
            Destroy(gameObject);
        }
    }
}


