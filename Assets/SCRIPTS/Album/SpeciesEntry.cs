using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controla cómo se muestra una entrada del álbum.
/// Muestra nombre, descripción e imagen de una especie,
/// y ajusta su apariencia dependiendo de si fue recolectada o no.
/// </summary>
public class SpeciesEntry : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image iconImage;         
    [SerializeField] private TMP_Text nameText;       
    [SerializeField] private TMP_Text descriptionText;

    [Header("Collected Colors")]
    [SerializeField] private Color collectedIconColor = Color.white;
    [SerializeField] private Color collectedNameColor = Color.white;
    [SerializeField] private Color collectedDescriptionColor = Color.white;

    [Header("Not Collected Colors")]
    [SerializeField] private Color notCollectedIconColor = new Color(0.4f, 0.4f, 0.4f, 0.4f);
    [SerializeField] private Color notCollectedNameColor = new Color(0.4f, 0.4f, 0.4f, 0.4f);
    [SerializeField] private Color notCollectedDescriptionColor = new Color(0.4f, 0.4f, 0.4f, 0.4f);

    /// <summary>
    /// Configura la entrada del álbum con los datos de la especie.
    /// </summary>
    public void Setup(SpeciesSO species, bool collected)
    {
        // Asignar nombre y descripción
        nameText.text = species.speciesName;
        descriptionText.text = species.description;

        // Asignar imagen
        iconImage.sprite = species.speciesImage;

        // Ajustar colores según estado
        if (collected)
        {
            iconImage.color = collectedIconColor;
            nameText.color = collectedNameColor;
            descriptionText.color = collectedDescriptionColor;
        }
        else
        {
            iconImage.color = notCollectedIconColor;
            nameText.color = notCollectedNameColor;
            descriptionText.color = notCollectedDescriptionColor;
        }
    }
}

