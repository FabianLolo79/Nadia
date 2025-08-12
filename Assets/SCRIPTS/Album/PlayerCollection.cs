using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestiona las especies que el jugador ha coleccionado
/// y permite consultar o agregar nuevas especies.
/// </summary>
public class PlayerCollection : MonoBehaviour
{
    private const string PREFS_KEY = "ALBUM_SAVE_V1";

    // Índice para reconstruir desde IDs → SpeciesSO al cargar
    [Header("Índice de especies (todas las que existen en el juego)")]
    [SerializeField] private List<SpeciesSO> allSpecies = new List<SpeciesSO>();

    private HashSet<SpeciesSO> collectedSpecies = new HashSet<SpeciesSO>(); // Conjunto de especies únicas recolectadas

    // Propiedad de solo lectura para acceder a la colección
    public IReadOnlyCollection<SpeciesSO> CollectedSpecies => collectedSpecies;

    // IDs persistentes (lo que realmente guardamos)
    private HashSet<string> collectedIds = new HashSet<string>();

    // Mapa rápido id → SO
    private Dictionary<string, SpeciesSO> idToSo = new Dictionary<string, SpeciesSO>();

    [Serializable] private class SaveData { public List<string> ids = new List<string>(); }

    //SingleTon
    public static PlayerCollection Instance { get; private set; }

    public event Action OnCollectionChanged;

    private void Awake()
    {
        BuildIndex();
        Load();
        if (Instance == null) 
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }

    private void OnDestroy()
    {
        //el GameManager persiste entre escenas
        if (GameManager.Instance != null)
            GameManager.Instance.OnFishCatch -= AddSpecies;
    }


    void Start()
    {
        GameManager.Instance.OnFishCatch += AddSpecies;
    }

    // Devuelve true si la especie indicada ya fue coleccionada
    public bool HasCollected(SpeciesSO species)
    {
        if (species == null)
        {
            Debug.LogWarning("Tried to check a null species in HasCollected.");
            return false;
        }

        return collectedSpecies.Contains(species);
    }

    private void BuildIndex()
    {
        idToSo.Clear();
        foreach (var so in allSpecies)
        {
            if (so == null) continue;
            if (string.IsNullOrWhiteSpace(so.speciesID))
            {
                Debug.LogWarning($"[PlayerCollection] SpeciesSO '{so.name}' no tiene speciesID asignado.");
                continue;
            }

            if (!idToSo.ContainsKey(so.speciesID))
                idToSo.Add(so.speciesID, so);
            else
                Debug.LogWarning($"[PlayerCollection] speciesID duplicado: {so.speciesID}");
        }
    }

    // Intenta añadir una nueva especie a la colección
    public void AddSpecies(SpeciesSO species)
    {
        if (species == null)
        {
            Debug.LogWarning("Tried to add a null species to the collection.");
            return;
        }

        if (collectedSpecies.Contains(species))
        {
            Debug.Log($"Species '{species.speciesID}' already collected.");
            return;
        }

        bool added = collectedSpecies.Add(species);

        if (added)
        {
            collectedIds.Add(species.speciesID); // 🔹 Importante: registrar el ID
            Save(); // 🔹 Guardar inmediatamente
            Debug.Log($"Collected new species: {species.speciesID}");
            OnCollectionChanged?.Invoke();
        }

    }

    // ===== Persistencia simple (PlayerPrefs + JSON) =====

    public void Save()
    {
        var data = new SaveData { ids = new List<string>(collectedIds) };
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(PREFS_KEY, json);
        PlayerPrefs.Save();
#if UNITY_EDITOR
        Debug.Log($"[PlayerCollection] Guardado {data.ids.Count} especies.");
#endif
    }

    public void Load()
    {
        collectedIds.Clear();
        collectedSpecies.Clear();

        if (!PlayerPrefs.HasKey(PREFS_KEY))
        {
            OnCollectionChanged?.Invoke();
            return;
        }

        try
        {
            string json = PlayerPrefs.GetString(PREFS_KEY);
            var data = JsonUtility.FromJson<SaveData>(json);
            if (data?.ids != null)
            {
                foreach (var id in data.ids)
                {
                    if (string.IsNullOrWhiteSpace(id)) continue;
                    collectedIds.Add(id);
                    if (idToSo.TryGetValue(id, out var so))
                        collectedSpecies.Add(so); // reconstruye el set de SO
                }
            }
#if UNITY_EDITOR
            Debug.Log($"[PlayerCollection] Cargado {collectedIds.Count} especies.");
#endif
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[PlayerCollection] Error al cargar: {e.Message}. Se limpia el estado.");
            collectedIds.Clear();
            collectedSpecies.Clear();
        }

        OnCollectionChanged?.Invoke();
    }

    public void ClearProgress()
    {
        collectedIds.Clear();
        collectedSpecies.Clear();
        PlayerPrefs.DeleteKey(PREFS_KEY);
        PlayerPrefs.Save();
#if UNITY_EDITOR
        Debug.Log("[PlayerCollection] Progreso borrado.");
#endif
        OnCollectionChanged?.Invoke();
    }

    // ===== Helpers útiles =====

    public void RebuildIndexAndRelink()
    {
        BuildIndex();
        // volver a enlazar SpeciesSO ya coleccionadas según IDs
        collectedSpecies.Clear();
        foreach (var id in collectedIds)
            if (idToSo.TryGetValue(id, out var so)) collectedSpecies.Add(so);
        OnCollectionChanged?.Invoke();
    }
}

