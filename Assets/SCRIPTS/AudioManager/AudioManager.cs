using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<AudioManager>();
            return _instance;
        }
    }

    [Header("MÚSICA")]
    [SerializeField] private EventReference musicEvent; // Evento ÚNICO de música en FMOD

    [Header("AUDIOS DESCRIPTIVOS")]
    [SerializeField] private EventReference amarillin;
    [SerializeField] private EventReference batata;
    [SerializeField] private EventReference centollaCoqueta;
    [SerializeField] private EventReference chanchitoDeMar;
    [SerializeField] private EventReference esponjaVitria;
    [SerializeField] private EventReference estrellaCulona;
    [SerializeField] private EventReference estrellaLoca1;
    [SerializeField] private EventReference estrellaLoca2;
    [SerializeField] private EventReference peluquita;
    [SerializeField] private EventReference raya;

    [Header("SFX SUBMARINE")]
    [SerializeField] private EventReference garra;
    [SerializeField] private EventReference winTakeObject;
    [SerializeField] private EventReference wrongTakeObject;
    [SerializeField] private EventReference clockAlarm;

    [Header("SFX UI")]
    [SerializeField] private EventReference buttonTap;

    // ------------------- INSTANCIAS -------------------
    private EventInstance musicInstance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject); // Persistente entre escenas

        StartMusic();
        SceneManager.sceneLoaded += OnSceneLoaded; // Detectar escena cargada
    }

    // ------------------- MÚSICA -------------------
    private void StartMusic()
    {
        musicInstance = RuntimeManager.CreateInstance(musicEvent);
        musicInstance.start();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateMusic(scene.name);
    }

    private void UpdateMusic(string sceneName)
    {
        // Usamos parámetro en FMOD llamado "MusicState"
        // Ejemplo: 0=Menu, 1=Tutorial, 2=Gameplay, 3=GameOver

        if (sceneName == "Menu") musicInstance.setParameterByName("music_play", 0); 
        else if (sceneName == "ScrollingMap") musicInstance.setParameterByName("music_play", 1); 
        else if (sceneName == "TimeOut") musicInstance.setParameterByName("music_play", 2); 
        else if (sceneName == "Congratulations") musicInstance.setParameterByName("music_play", 3);
    }

    public void PauseMusic() => musicInstance.setPaused(true);
    public void ResumeMusic() => musicInstance.setPaused(false);

    // ------------------- SFX -------------------
    public void PlayOneShot(EventReference soundRef)
    {
        RuntimeManager.PlayOneShot(soundRef);
    }

    public void PlayTapButton() => PlayOneShot(buttonTap);
    public void PlayTakeObject() => PlayOneShot(winTakeObject);
    public void PlayFallObject() => PlayOneShot(wrongTakeObject);
    public void PlayClockAlarm() => PlayOneShot(clockAlarm);

    public void StopClockAlarm()
    {
        if (!clockAlarm.IsNull)
        {
            var instance = RuntimeManager.CreateInstance(clockAlarm);
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();
        }
    }

    // ------------------- LIMPIEZA -------------------
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicInstance.release();
        }
    }
}
