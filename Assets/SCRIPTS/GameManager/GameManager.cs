using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // ==== Eventos de flujo general ====
    public event Action OnGameStart;
    public event Action OnGamePause;
    public event Action OnGameResume;
    public event Action OnGameEnd;

    // ==== Eventos para scroll ====
    public event Action OnStartScroll;
    public event Action OnStopScroll;

    // ==== Eventos para interacción con peces ====
    public event Action<SpeciesSO> OnFishTouch;
    public event Action<SpeciesSO> OnFishCatch;
    public event Action<SpeciesSO> OnFishNotCatch;

    // ==== Estados del juego ====
    public enum GameState { Waiting, Playing, Paused, Ended }
    public GameState CurrentState { get; private set; } = GameState.Waiting;

    // ==== Referencias a UI ====
    [Header("UI Panels")]
    [SerializeField] private GameObject endGamePanel;

    [Header("UI Timer")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float gameTime = 60f;

    private float timeRemaining;

    // ==== Nombres de escenas (setear desde Inspector) ====
    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    // ==== Singleton ====
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Solo inicializamos si estamos en la escena de juego
        if (SceneManager.GetActiveScene().name == gameSceneName)
            InitializeGame();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == gameSceneName)
        {
            InitializeGame();

            // Control para continuar o iniciar nueva partida segun PlayerPrefs
            if (PlayerPrefs.GetInt("ResumeAfterLoad", 0) == 1)
            {
                PlayerPrefs.SetInt("ResumeAfterLoad", 0);
                ContinueGame();
            }
            else if (PlayerPrefs.GetInt("StartGameOnLoad", 0) == 1)
            {
                PlayerPrefs.SetInt("StartGameOnLoad", 0);
                StartNewGame();
            }
            else
            {
                CurrentState = GameState.Waiting;
                Time.timeScale = 0f;
            }
        }
    }

    private void InitializeGame()
    {
        CurrentState = GameState.Waiting;
        timeRemaining = gameTime;
        Time.timeScale = 0f; // Pausamos el tiempo hasta que empiece la partida
        UpdateTimerUI();
        if (endGamePanel != null)
            endGamePanel.SetActive(false);
    }

    private void Update()
    {
        if (CurrentState == GameState.Playing)
        {
            timeRemaining -= Time.unscaledDeltaTime;

            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                EndGame();
            }

            UpdateTimerUI();
        }
    }

    // ==== Control de flujo ====

    public void StartNewGame()
    {
        if (CurrentState != GameState.Waiting) return;

        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
        OnGameStart?.Invoke();
        OnStartScroll?.Invoke();

        PlayerPrefs.SetInt("HasPausedGame", 0);
    }

    public void ContinueGame()
    {
        if (CurrentState != GameState.Waiting && CurrentState != GameState.Paused) return;

        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
        OnGameResume?.Invoke();
        OnStartScroll?.Invoke();

        PlayerPrefs.SetInt("HasPausedGame", 0);
    }

    public void PauseGame()
    {
        if (CurrentState != GameState.Playing) return;

        CurrentState = GameState.Paused;
        Time.timeScale = 0f;
        OnGamePause?.Invoke();

        PlayerPrefs.SetInt("HasPausedGame", 1);
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused) return;

        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
        OnGameResume?.Invoke();
    }

    public void EndGame()
    {
        if (CurrentState == GameState.Ended) return;

        CurrentState = GameState.Ended;
        Time.timeScale = 0f;
        if (endGamePanel != null)
            endGamePanel.SetActive(true);

        OnGameEnd?.Invoke();
        OnStopScroll?.Invoke();

        PlayerPrefs.SetInt("HasPausedGame", 0);
    }

    // ==== Scroll manual ====
    public void TriggerStartScroll() => OnStartScroll?.Invoke();
    public void TriggerStopScroll() => OnStopScroll?.Invoke();

    // ==== Eventos peces ====
    public void FishTouched(SpeciesSO species) => OnFishTouch?.Invoke(species);
    public void FishCaught(SpeciesSO species) => OnFishCatch?.Invoke(species);
    public void FishNotCaught(SpeciesSO species) => OnFishNotCatch?.Invoke(species);

    // ==== UI actualización ====
    private void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    // ==== Volver al menú ====
    public void ReturnToMainMenu()
    {
        PauseGame();
        SceneManager.LoadScene(mainMenuSceneName);
    }
}




