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
    [SerializeField] private string gameplayScene = "ScrollingMap";
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

    [SerializeField] float baseDifficulty = 1f;
    [SerializeField] float difficultyGrowth = 0.2f;
    [SerializeField] private float maxDifficulty = 3f;

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

    [Header("UI Timer")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float gameTime = 60f;
    private bool isWarningSoundPlaying = false;
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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        ResetRun();
        CurrentState = GameState.Playing; // Intercambiar por Playing para que el timer corra hasta que exista el endpanel
        timeRemaining = gameTime;

        Time.timeScale = 1f; // aseguramos que esté activo

        UpdateTimerUI();

        albumPanel.SetActive(false);
    }

    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    RebindSceneRefs();
    //    // cada vez que entro a Menú o Gameplay, reseteo
    //    if (scene.name == menuSceneName || scene.name == gameplayScene)
    //    {
    //        ResetRun();
    //        // Si en Menú querés Waiting y en Gameplay auto-start, podés diferenciar acá
    //        CurrentState = (scene.name == gameplayScene) ? GameState.Playing : GameState.Waiting;
    //        Time.timeScale = 1f;
    //        UpdateTimerUI();
    //    }
    //}

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RebindSceneRefs();

        if (scene.name == menuSceneName)
        {
            ResetRun();
            CurrentState = GameState.Waiting;
            Time.timeScale = 1f;
            UpdateTimerUI();

            // Música del menú 
            //AudioManager.Instance.PlayMusicMenu();
        }
        else if (scene.name == gameplayScene)
        {
            ResetRun();
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
            UpdateTimerUI();

            // Música del gameplay
            AudioManager.Instance.PlayGameMusicWithEngine();
        }
    }

    private void Update()
    {

        if (CurrentState == GameState.Playing)
        {
            DifficultyMult = Mathf.Min(DifficultyMult + Time.deltaTime * difficultyGrowth, maxDifficulty);
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
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;

        OnGameStart?.Invoke();
        OnStartScroll?.Invoke();

        //AudioManager.Instance.PlayGameMusicWithEngine();
    }

    public void PauseGame()
    {
        if (CurrentState != GameState.Playing) return;

        AudioManager.Instance.StartSnapshotEnPausa();

        CurrentState = GameState.Paused;
        Time.timeScale = 0f;
        OnGamePause?.Invoke();

        //AudioManager.Instance.PauseGameMusicWithEngine();
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused) return;

        AudioManager.Instance.StopSnapshotEnPausa();

        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
        OnGameResume?.Invoke();

        //AudioManager.Instance.ResumeGameMusicWithEngine();
    }

    // NUEVO - solo para caso tiempo agotado
    private void EndGameByTime()
    {
        if (CurrentState == GameState.Ended) return;

        AudioManager.Instance.PauseGameMusicWithEngine();

        AudioManager.Instance.StopMusic();

        CurrentState = GameState.Ended;
        Time.timeScale = 0f;
        OnGameEnd?.Invoke();
        OnStopScroll?.Invoke();
             

        // Lanzar evento global para BannerSceneLoader
        EventsManager.Instance?.TimeUp(); // NUEVO


    }

    public void EndGameByAlbum()
    {
        if (CurrentState == GameState.Ended) return;

        CurrentState = GameState.Ended;
        Time.timeScale = 0f;
        OnGameEnd?.Invoke();
        OnStopScroll?.Invoke();

    }


    public void EndGame()
    {
        if (CurrentState == GameState.Ended) return;

        CurrentState = GameState.Ended;
        Time.timeScale = 0f;
        OnGameEnd?.Invoke();
        OnStopScroll?.Invoke();

        // Cambio de música
        AudioManager.Instance.PlayMusicMenu();

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

        //AudioManager.Instance.PlayMusicMenu(); TODO que onda
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

        // Lógica para reproducir sonido en los últimos 10 segundos
        if (timeRemaining <= 5f && timeRemaining > 0 && !isWarningSoundPlaying)
        {
            AudioManager.Instance.PlayClockAlarm(); // Reproducir sonido de advertencia
            isWarningSoundPlaying = true;
        }
        else if (timeRemaining == 0)
        {
            AudioManager.Instance.StopMusic();
        }
    }

    private void ResetRun()
    {
        // Estado base para una nueva partida
        CurrentState = GameState.Waiting;       // o Playing si querés auto‑start
        Time.timeScale = 1f;

        timeRemaining = gameTime;
        DifficultyMult = baseDifficulty;        //  Reinicio dificultad

        UpdateTimerUI();
    }

    private void RebindSceneRefs()
    {
        // TimerText
        var tgo = GameObject.FindWithTag("TimerText");
        timerText = tgo ? tgo.GetComponent<TMP_Text>() : null;

        // Actualizá la UI apenas re-vinculada
        UpdateTimerUI();
    }


}


