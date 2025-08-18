using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class StopDescriptions : MonoBehaviour
{
    //[Header("Música de fondo")]
    //[SerializeField] private EventReference backgroundMusic;  Música que debe seguir sonando

    private EventInstance musicInstance;

    // Llamar desde el botón "Salir del álbum"
    public void StopAudioDescription()
    {
        // Detener todos los eventos activos
        RuntimeManager.GetBus("bus:/AudiosDescriptivos").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void PauseMusic()
    {
        AudioManager.Instance.PauseMusic();
    }

    public void PlayMusic() 
    {
        AudioManager.Instance.ResumeMusic();
    }

}
