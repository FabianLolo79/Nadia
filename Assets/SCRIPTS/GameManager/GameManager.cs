using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

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
    public event Action<SpeciesSO> OnFishTouch;  // Cuando el brazo toca un pez
    public event Action<SpeciesSO> OnFishCatch;  // Cuando el pez es capturado

    // ==== Estados del juego ====
    public enum GameState { Waiting, Playing, Paused, Ended }
    public GameState CurrentState { get; private set; } = GameState.Waiting;

    // ==== Referencias a UI ====
    [Header("UI Panels")]
    [SerializeField] private GameObject albumPanel; 
    [SerializeField] private GameObject endGamePanel;

    [Header("UI Timer")]
    [SerializeField] private TMPro.TMP_Text timerText;
    [SerializeField] private float gameTime = 60f; // Duración en segundos

    private float timeRemaining;
    private bool timerRunning = false;

    // ==== Singleton ====
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        CurrentState = GameState.Waiting;
        timeRemaining = gameTime;
        UpdateTimerUI();
    }

    private void Update()
    {
        if (timerRunning)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                EndGame();
            }
            UpdateTimerUI();
        }
    }

    // ==== Métodos para controlar el flujo del juego ====
    public void StartGame()
    {
        CurrentState = GameState.Playing;
        timerRunning = true;
        OnGameStart?.Invoke(); // evento general
        OnStartScroll?.Invoke(); // comienza scroll
    }

    public void PauseGame()
    {
        if (CurrentState != GameState.Playing) return;
        CurrentState = GameState.Paused;
        timerRunning = false;
        OnGamePause?.Invoke();
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused) return;
        CurrentState = GameState.Playing;
        timerRunning = true;
        OnGameResume?.Invoke();
    }

    public void EndGame()
    {
        CurrentState = GameState.Ended;
        timerRunning = false;
        endGamePanel.SetActive(true);
        OnGameEnd?.Invoke();
        OnStopScroll?.Invoke();
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
    
    // ==== Eventos de interacción con peces ====
    public void FishTouched(SpeciesSO species)
    {
        OnFishTouch?.Invoke(species);
    }

    public void FishCaught(SpeciesSO species)
    {
        OnFishCatch?.Invoke(species);
    }

    // ==== Control del álbum ====
    public void ToggleAlbum()
    {
        bool isActive = albumPanel.activeSelf;
        albumPanel.SetActive(!isActive);

        if (!isActive) PauseGame();
        else ResumeGame();
    }

    // ==== UI Timer ====
    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}

