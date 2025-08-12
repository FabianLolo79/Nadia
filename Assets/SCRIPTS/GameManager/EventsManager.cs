using System;
using UnityEngine;

/// <summary>
/// Centraliza eventos globales del juego.
/// No contiene lógica visual, solo define e invoca eventos.
/// </summary>
public class EventsManager : MonoBehaviour
{
    public static EventsManager Instance { get; private set; }

    public event Action OnTimeUp;
    public event Action OnAlbumCompleted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Mantener entre escenas
    }

    public void TimeUp() => OnTimeUp?.Invoke();
    public void AlbumCompleted() => OnAlbumCompleted?.Invoke();
}
