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
    [SerializeField] private EventReference _musicEvent; // Evento ÚNICO de música en FMOD

    [Header("AUDIOS DESCRIPTIVOS")]
    [SerializeField] private EventReference _amarillin;
    [SerializeField] private EventReference _batata;
    [SerializeField] private EventReference _centollaCoqueta;
    [SerializeField] private EventReference _chanchitoDeMar;
    [SerializeField] private EventReference _esponjaVitria;
    [SerializeField] private EventReference _estrellaCulona;
    [SerializeField] private EventReference _estrellaLoca1;
    [SerializeField] private EventReference _estrellaLoca2;
    [SerializeField] private EventReference _peluquita;
    [SerializeField] private EventReference _raya;
    [SerializeField] private EventReference _merenguito;

    [Header("SFX SUBMARINE")]
    [SerializeField] private EventReference _garra;
    [SerializeField] private EventReference _winTakeObject;
    [SerializeField] private EventReference _wrongTakeObject;
    [SerializeField] private EventReference _clockAlarm;

    [Header("SFX UI")]
    [SerializeField] private EventReference _buttonTap;

    // ------------------- INSTANCIAS -------------------
    private EventInstance _musicInstance;

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
        _musicInstance = RuntimeManager.CreateInstance(_musicEvent);
        _musicInstance.start();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateMusic(scene.name);
    }

    private void UpdateMusic(string sceneName)
    {
        // Usamos parámetro en FMOD llamado "MusicState"
        // Ejemplo: 0=Menu, 1=Tutorial, 2=Gameplay, 3=GameOver

        if (sceneName == "Menu") _musicInstance.setParameterByName("music_play", 0); 
        else if (sceneName == "ScrollingMap") _musicInstance.setParameterByName("music_play", 1); 
        else if (sceneName == "TimeOut") _musicInstance.setParameterByName("music_play", 2); 
        else if (sceneName == "Congratulations") _musicInstance.setParameterByName("music_play", 3);
    }

    public void PauseMusic() => _musicInstance.setPaused(true);
    public void ResumeMusic() => _musicInstance.setPaused(false);

    // ------------------- SFX -------------------
    public void PlayOneShot(EventReference soundRef)
    {
        RuntimeManager.PlayOneShot(soundRef);
    }

    public void PlayTapButton() => PlayOneShot(_buttonTap);
    public void PlayTakeObject() => PlayOneShot(_winTakeObject);
    public void PlayFallObject() => PlayOneShot(_wrongTakeObject);
    public void PlayClockAlarm() => PlayOneShot(_clockAlarm);

    public void StopClockAlarm()
    {
        if (!_clockAlarm.IsNull)
        {
            var instance = RuntimeManager.CreateInstance(_clockAlarm);
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();
        }
    }

    // ------------------- LIMPIEZA -------------------
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (_musicInstance.isValid())
        {
            _musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _musicInstance.release();
        }
    }
}
