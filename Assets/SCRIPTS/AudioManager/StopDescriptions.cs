using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class StopDescriptions : MonoBehaviour
{
    [Header("Música de fondo")]
    [SerializeField] private EventReference backgroundMusic; // Música que debe seguir sonando

    private EventInstance musicInstance;

    private void Start()
    {
        // Si hay música configurada, la creamos y arrancamos
        if (!backgroundMusic.IsNull)
        {
            musicInstance = RuntimeManager.CreateInstance(backgroundMusic);
            musicInstance.start();
            musicInstance.release(); // Libera memoria al terminar
        }
    }

    // Llamar desde el botón "Salir del álbum"
    public void StopAllAudioExceptMusic()
    {
        // Detener todos los eventos activos
        RuntimeManager.GetBus("bus:/AudiosDescriptivos").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

        // Volver a iniciar la música
        if (!backgroundMusic.IsNull)
        {
            musicInstance = RuntimeManager.CreateInstance(backgroundMusic);
            musicInstance.start();
            musicInstance.release();
        }

        Debug.Log("Todos los audios detenidos, música de fondo reiniciada.");
    }
}
