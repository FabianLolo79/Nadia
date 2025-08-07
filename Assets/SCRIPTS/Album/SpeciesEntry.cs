using UnityEngine;
using UnityEngine.UI;
using TMPro; // Para usar TextMeshPro

/// <summary>
/// Controla cómo se muestra una entrada del álbum.
/// Muestra nombre, descripción e imagen de una especie,
/// y ajusta su apariencia dependiendo de si fue recolectada o no.
/// </summary>
public class SpeciesEntry : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image iconImage;         // Imagen de la especie
    [SerializeField] private TMP_Text nameText;       // Texto con el nombre
    [SerializeField] private TMP_Text descriptionText;// Texto con la descripción

    /// <summary>
    /// Configura la entrada del álbum con los datos de la especie.
    /// </summary>
    /// <param name="species">Datos de la especie (ScriptableObject)</param>
    /// <param name="collected">Indica si el jugador ya la recolectó</param>
    public void Setup(SpeciesSO species, bool collected)
    {
        // Asignar nombre y descripción
        nameText.text = species.speciesName;
        descriptionText.text = species.description;

        // Asignar imagen
        iconImage.sprite = species.speciesImage;

        // Ajustar color según estado de colección
        if (collected)
        {
            iconImage.color = Color.white;
            nameText.color = Color.white;
            descriptionText.color = Color.white;
        }
        else
        {
            Color faded = new Color(0.2f, 0.2f, 0.2f, 0.2f); // Color apagado
            iconImage.color = faded;
            nameText.color = faded;
            descriptionText.color = faded;
        }
    }
}
