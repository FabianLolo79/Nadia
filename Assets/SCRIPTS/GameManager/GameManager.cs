using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // <-- Necesario para cambiar de escena

public class GameManager : MonoBehaviour
{
    // ==== Configuración de escenas ====
    [Header("Nombre de la escena del menú")]
    [SerializeField] private string menuSceneName = "Menu"; // Cambiable desde Inspector

    // ==== Eventos de flujo general ====
    public event Action OnGameStart;
    public event Action OnGamePause;
    public event Action OnGameResume;
    public event Action OnGameEnd;

    // ==== Eventos para scroll ====
    public event Action OnStartScroll;
    public event Action OnStopScroll;

    // ==== Configuración de dificultad ====
    public float DifficultyMult;

    [SerializeField] float difficulty => DifficultyMult;
    [SerializeField] private float difficultyLevel = 1;

    // ==== Eventos para interacción con peces ====
    public event Action<SpeciesSO> OnFishTouch;
    public event Action<SpeciesSO> OnFishCatch;
    public event Action<SpeciesSO> OnFishNotCatch;

    // ==== Estados del juego ====
    public enum GameState { Waiting, Playing, Paused, Ended }
    public GameState CurrentState { get; private set; } = GameState.Waiting;

    // ==== Referencias a UI ====
    [Header("UI Panels")]
    [SerializeField] private GameObject albumPanel;
    [SerializeField] private GameObject endGamePanel;

    [Header("UI Timer")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float gameTime = 60f;

    private float timeRemaining;

    // ==== Singleton ====
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        CurrentState = GameState.Waiting; // Intercambiar por Playing para que el timer corra hasta que exista el endpanel
        timeRemaining = gameTime;

        Time.timeScale = 1f; // aseguramos que esté activo

        UpdateTimerUI();

        endGamePanel.SetActive(false);
        albumPanel.SetActive(false);
    }

    private void Update()
    {
        DifficultyMult += Time.deltaTime * difficultyLevel;

        if (CurrentState == GameState.Playing)
        {
            timeRemaining -= Time.unscaledDeltaTime;

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                EndGameByTime(); // NUEVO - reemplaza EndGame() cuando es por tiempo
            }
            UpdateTimerUI();
        }

    }

    // ==== Control del flujo ====
    public void StartGame()
    {
        if (CurrentState != GameState.Waiting) return;

        DifficultyMult = difficultyLevel * 0.4f;
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;

        OnGameStart?.Invoke();
        OnStartScroll?.Invoke();
    }

    public void PauseGame()
    {
        if (CurrentState != GameState.Playing) return;

        CurrentState = GameState.Paused;
        Time.timeScale = 0f;
        OnGamePause?.Invoke();
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused) return;

        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
        OnGameResume?.Invoke();
    }

    // NUEVO - solo para caso tiempo agotado
    private void EndGameByTime()
    {
        if (CurrentState == GameState.Ended) return;

        CurrentState = GameState.Ended;
        Time.timeScale = 0f;
        OnGameEnd?.Invoke();
        OnStopScroll?.Invoke();

        // Lanzar evento global para BannerSceneLoader
        EventsManager.Instance?.TimeUp(); // NUEVO
    }

    public void EndGame()
    {
        if (CurrentState == GameState.Ended) return;

        CurrentState = GameState.Ended;
        Time.timeScale = 0f;
        endGamePanel.SetActive(true);
        OnGameEnd?.Invoke();
        OnStopScroll?.Invoke();
        SceneManager.LoadScene(menuSceneName);
    }

    // NUEVO - para caso álbum completado
    public void OnAlbumComplete()
    {
        EventsManager.Instance?.AlbumCompleted();
    }

    // ==== Volver al menú ====
    public void ReturnToMenu()
    {
        Time.timeScale = 1f; // aseguramos que no esté pausado
        SceneManager.LoadScene(menuSceneName);
    }

    // ==== Scroll manual ====
    public void TriggerStartScroll()
    {
        OnStartScroll?.Invoke();
    }

    public void TriggerStopScroll()
    {
        OnStopScroll?.Invoke();
    }

    // ==== Eventos peces ====
    public void FishTouched(SpeciesSO species)
    {
        OnFishTouch?.Invoke(species);
    }

    public void FishCaught(SpeciesSO species)
    {
        OnFishCatch?.Invoke(species);
    }

    public void FishNotCaught(SpeciesSO species)
    {
        OnFishNotCatch?.Invoke(species);
    }

    // ==== Control álbum ====
    public void ToggleAlbum()
    {
        bool isActive = albumPanel.activeSelf;
        albumPanel.SetActive(!isActive);

        if (!isActive) PauseGame();
        else ResumeGame();
    }

    // ==== Actualizar UI timer ====
    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }


}


