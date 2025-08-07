using UnityEngine;
using UnityEngine.UI;
using TMPro; // si usás TextMeshPro

public class SpeciesEntry : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    public void Setup(SpeciesSO species, bool collected)
    {
        // Nombre y descripción
        nameText.text = species.speciesName;
        descriptionText.text = species.description;

        // Imagen
        iconImage.sprite = species.speciesImage;


        // Color según estado de colección
        if (collected)
        {
            iconImage.color = Color.white;
            nameText.color = Color.white;
            descriptionText.color = Color.white;
        }
        else
        {
            Color faded = new Color(0.2f, 0.2f, 0.2f, 0.2f);
            iconImage.color = faded;
            nameText.color = faded;
            descriptionText.color = faded;
        }
    }
}
