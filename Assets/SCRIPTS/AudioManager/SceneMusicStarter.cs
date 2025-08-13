using UnityEngine;
using FMODUnity;

public class SceneMusicStarter : MonoBehaviour
{
    [Tooltip("Evento de música a reproducir al iniciar la escena")]
    public EventReference musicaDeEscena;

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic(); // <--- Detiene música anterior si aún suena
            AudioManager.Instance.PlayMusic(musicaDeEscena);
        }
    }
}