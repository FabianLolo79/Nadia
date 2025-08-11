using UnityEngine;

/// <summary>
/// Representa una especie coleccionable en el mundo.
/// Al ser recogida por el jugador, se agrega a su colección
/// y el objeto desaparece.
/// </summary>
public class SpeciesCollectable : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer; // Muestra la imagen en pixel art
    [SerializeField] private SpeciesSO speciesData;         // Datos de la especie

    [SerializeField] GameObject SucceedVFX;
    [SerializeField] GameObject FailedVFX;

    [SerializeField] GameObject NewSpeciesVFX;

    public SpeciesSO SpeciesSO => speciesData;

    [SerializeField] private bool isGrabbed = false;

    private void Awake()
    {
        // Si no se asignó en el inspector, buscarlo automáticamente
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

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
            Debug.LogWarning($"[SpeciesCollectable] No se encontró SpriteRenderer en {gameObject.name}");
        }

    }
void Start()
{
    FishQTE.Instance.OnQTEFinished += Dissapear;

    // Si el jugador todavía NO tiene esta especie, spawneamos halo
    if (!PlayerCollection.Instance.HasCollected(speciesData))
    {
        Instantiate(NewSpeciesVFX, transform.position, Quaternion.identity, transform);
    }
}

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerCollection player = other.GetComponent<PlayerCollection>();

        if (player == null || speciesData == null)
            return;
        if (FishQTE.Instance.IsRunning) return;

        GameManager.Instance.FishTouched(speciesData);

        Collider2D col = GetComponent<Collider2D>();
        col.enabled = false;

        isGrabbed = true;
    }

    public void Dissapear(bool bol)
    {
        if (!isGrabbed) return;
        if (bol)
        {
            Instantiate(SucceedVFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else
        {
            Instantiate(FailedVFX, transform.position, Quaternion.identity);
            Destroy(gameObject);

        }

    }

    void OnDestroy()
    {
        FishQTE.Instance.OnQTEFinished -= Dissapear;

    }


}
    



