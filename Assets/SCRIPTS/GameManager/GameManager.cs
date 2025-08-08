using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // ==== Eventos ====
    // Se disparan cuando el fondo y las criaturas deben moverse o detenerse
    public event Action OnStartScroll;
    public event Action OnStopScroll;

    // Eventos para interacción con peces
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
    [SerializeField] private Text timerText;
    [SerializeField] private float gameTime = 60f; // Duración en segundos

    private float timeRemaining;
    private bool timerRunning = false;

    // ==== Singleton ====
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton básico para acceso global
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Estado inicial: esperando inicio de partida
        CurrentState = GameState.Waiting;
        timeRemaining = gameTime;
        UpdateTimerUI();
    }

    private void Update()
    {
        // Control del temporizador solo si el juego está activo
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

    // Inicia el juego, empieza el timer y el scroll del mar
    public void StartGame()
    {
        CurrentState = GameState.Playing;
        timerRunning = true;
        OnStartScroll?.Invoke();
    }

    // Pausa el juego y el timer
    public void PauseGame()
    {
        if (CurrentState != GameState.Playing) return;
        CurrentState = GameState.Paused;
        timerRunning = false;
    }

    // Reanuda el juego y el timer
    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused) return;
        CurrentState = GameState.Playing;
        timerRunning = true;
    }

    // Termina la partida, muestra panel final y detiene el scroll
    public void EndGame()
    {
        CurrentState = GameState.Ended;
        timerRunning = false;
        endGamePanel.SetActive(true);
        OnStopScroll?.Invoke();
    }

    // ==== Métodos para controlar el scroll del mar y criaturas ====

    public void TriggerStartScroll()
    {
        OnStartScroll?.Invoke();
    }

    public void TriggerStopScroll()
    {
        OnStopScroll?.Invoke();
    }
    
    // ==== Métodos para disparar eventos de interacción con peces ====

    // Llamar cuando el brazo toca un pez
    public void FishTouched(SpeciesSO species)
    {
        OnFishTouch?.Invoke(species);
    }

    // Llamar cuando se captura un pez
    public void FishCaught(SpeciesSO species)
    {
        OnFishCatch?.Invoke(species);
    }

    // ==== Control del álbum ====

    // Alterna abrir/cerrar álbum, pausa/reanuda el juego según estado
    public void ToggleAlbum()
    {
        bool isActive = albumPanel.activeSelf;
        albumPanel.SetActive(!isActive);

        if (!isActive)
        {
            PauseGame(); // Pausa al abrir álbum
        }
        else
        {
            ResumeGame(); // Reanuda al cerrar
        }
    }

    // ==== Actualiza texto del timer en UI ====
    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
