using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla la apertura/cierre del álbum y la creación dinámica
/// de las entradas para cada especie.
/// </summary>
public class AlbumManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject albumPanel;           // Panel principal del álbum (activar/desactivar)
    [SerializeField] private Transform entriesContainer;      // Contenedor donde se instancian los speciesEntry
    [SerializeField] private GameObject speciesEntryPrefab;   // Prefab de la entrada de cada especie
    [SerializeField] private GameObject confirmClearPanel;

    [Header("Data")]
    [SerializeField] private List<SpeciesSO> allSpecies;      // Todas las SpeciesSO que deben aparecer en el álbum
    [SerializeField] private PlayerCollection playerCollection;// Referencia al PlayerCollection para consultar estado

    private bool isOpen = false; // Estado del álbum

    // Alterna entre abrir y cerrar el álbum
    public void ToggleAlbum()
    {
        if (isOpen)
            HideAlbum();
        else
            ShowAlbum();
    }

    // Muestra el álbum: limpia entradas previas, instancia nuevas y pausa el juego
    public void ShowAlbum()
    {
        ClearEntries();

        foreach (var species in allSpecies)
        {
            GameObject entryGO = Instantiate(speciesEntryPrefab, entriesContainer);
            SpeciesEntry entry = entryGO.GetComponent<SpeciesEntry>();

            bool collected = playerCollection.HasCollected(species);
            entry.Setup(species, collected);
        }

        albumPanel.SetActive(true);
        Time.timeScale = 0f; // Pausa el juego
        isOpen = true;
    }

    // Oculta el álbum y reanuda el juego
    public void HideAlbum()
    {
        albumPanel.SetActive(false);
        Time.timeScale = 1f; // Reanuda el juego
        isOpen = false;
    }

    // Borra todas las entradas instanciadas en el contenedor
    private void ClearEntries()
    {
        foreach (Transform child in entriesContainer)
        {
            Destroy(child.gameObject);
        }
    }

    // ==== NUEVAS FUNCIONES PARA BORRAR PROGRESO ====

    // Abre el panel de confirmación
    public void PromptClearProgress()
    {
        if (confirmClearPanel != null)
            confirmClearPanel.SetActive(true);
    }

    // Confirma y borra el progreso
    public void ConfirmClearProgress()
    {
        playerCollection.ClearProgress();
        if (confirmClearPanel != null)
            confirmClearPanel.SetActive(false);

        // Si el álbum está abierto, refrescar contenido
        if (isOpen)
            ShowAlbum();
    }

    // Cancela la acción
    public void CancelClearProgress()
    {
        if (confirmClearPanel != null)
            confirmClearPanel.SetActive(false);
    }
}

