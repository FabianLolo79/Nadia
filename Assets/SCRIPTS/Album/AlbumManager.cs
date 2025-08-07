using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlbumManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject albumPanel;
    [SerializeField] private Transform entriesContainer;
    [SerializeField] private GameObject speciesEntryPrefab;

    [Header("Data")]
    [SerializeField] private List<SpeciesSO> allSpecies;
    [SerializeField] private PlayerCollection playerCollection;

    private bool isOpen = false;

    public void ToggleAlbum()
    {
        if (isOpen)
            HideAlbum();
        else
            ShowAlbum();
    }

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

    public void HideAlbum()
    {
        albumPanel.SetActive(false);
        Time.timeScale = 1f; // Reanuda el juego
        isOpen = false;
    }

    private void ClearEntries()
    {
        foreach (Transform child in entriesContainer)
        {
            Destroy(child.gameObject);
        }
    }
}


