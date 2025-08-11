using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Buscar en la escena actual
                _instance = FindObjectOfType<AudioManager>();

                // Si no existe, crearlo automáticamente
                if (_instance == null)
                {
                    GameObject obj = new GameObject("AudioManager");
                    _instance = obj.AddComponent<AudioManager>();
                }
            }
            return _instance;
        }
    }

    [Header("MÚSICA")]
    [SerializeField] private EventReference musicAmbientGame;
    [SerializeField] private EventReference musicFinalHimno;
    [SerializeField] private EventReference musicGame;
    [SerializeField] private EventReference musicMenu;
    [SerializeField] private EventReference musicCreditos;

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
    [SerializeField] private EventReference changeMandoBoat;
    [SerializeField] private EventReference changeMandoGarra;
    [SerializeField] private EventReference engineAsc;
    [SerializeField] private EventReference engineDesc;
    [SerializeField] private EventReference garra;
    [SerializeField] private EventReference lintern;
    [SerializeField] private EventReference bubble;
    [SerializeField] private EventReference winTakeObject;
    [SerializeField] private EventReference wrongTakeObject;
    [SerializeField] private EventReference clockAlarm;

    [Header("SFX UI")]
    [SerializeField] private EventReference buttonTap;

    [Header("SNAPSHOTS")]
    [SerializeField] private EventReference snapshotEnPausa;
    [SerializeField] private EventReference snapshotSinMusica;
    [SerializeField] private EventReference snapshotSinAmbiente;
    [SerializeField] private EventReference snapshotSinSfx;

    // Instancias activas
    private EventInstance currentMusic;
    private EventInstance activeSnapshot;
    private EventInstance engineAscInstance;
    private EventInstance engineDescInstance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

    }
    private void Start()
    {
        PlayMusicMenu();
    }

    // ------------------- MÉTODOS GENERALES -------------------
    public void PlayOneShot(EventReference soundRef)
    {
        RuntimeManager.PlayOneShot(soundRef);
    }

    public EventInstance CreateInstance(EventReference soundRef)
    {
        return RuntimeManager.CreateInstance(soundRef);
    }

    // ------------------- MÚSICA -------------------
    public void PlayMusic(EventReference musicRef)
    {
        StopMusic();
        currentMusic = RuntimeManager.CreateInstance(musicRef);
        currentMusic.start();
    }

    public void StopMusic()
    {
        if (currentMusic.isValid())
            currentMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    // Pausar la música actual sin destruirla
    public void PauseMusic() => currentMusic.setPaused(true);
    public void ResumeMusic() => currentMusic.setPaused(false);

    public void SwitchMusic(EventReference newMusic)
    {
        StopMusic();
        PlayMusic(newMusic);
    }

    public void PlayCreditToMusic()
    {
        SwitchMusic(musicCreditos);
    }

    public void PlayCreditToMusicBack()
    {
        SwitchMusic(musicMenu);
    }

    public void PlayGameMusicWithEngine()
    {
        StopMusic();                    // Para musicMenu
        PlayMusic(musicGame);           // Reproduce música de juego
        StartEngineAsc();               // Arranca sonido motor ascendente
    }

    public void PlayMusicMenu() => PlayMusic(musicMenu);
    public void PlayMusicGame() => PlayMusic(musicGame);
    public void PlayMusicAmbientGame() => PlayMusic(musicAmbientGame);
    public void PlayMusicCreditos() => PlayMusic(musicCreditos);
    public void PlayMusicFinalHimno() => PlayMusic(musicFinalHimno);

    // ------------------- SNAPSHOTS -------------------

    public void StartSnapshot(EventReference snapshotRef)
    {
        StopSnapshot();
        activeSnapshot = RuntimeManager.CreateInstance(snapshotRef);
        activeSnapshot.start();
    }

    public void StopSnapshot()
    {
        if (activeSnapshot.isValid())
            activeSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }


    // ------------------- MOTORES -------------------
    public void StartEngineAsc()
    {
        if (engineAscInstance.isValid())
            engineAscInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        engineAscInstance = CreateInstance(engineAsc);
        engineAscInstance.start();

        StartCoroutine(ChangeMotorIntensity(engineAscInstance, 0, 3, 3f));
    }
    public void StartEngineDesc()
    {
        if (engineDescInstance.isValid())
            engineDescInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        engineDescInstance = CreateInstance(engineDesc);
        engineDescInstance.start();

        StartCoroutine(ChangeMotorIntensity(engineDescInstance, 0, 3, 3f));
    }


    private IEnumerator ChangeMotorIntensity(EventInstance instance, int startValue, int endValue, float delay)
    {
        for (int value = startValue; value <= endValue; value++)
        {
            instance.setParameterByName("intensidad_motor", value);
            yield return new WaitForSeconds(delay);
        }
    }
   
    // ------------------- SFX -------------------
    public void PlayTapButton() => PlayOneShot(buttonTap);
    public void PlayChangeMandoBoat() => PlayOneShot(changeMandoBoat);
    public void PlayChangeMandoGarra() => PlayOneShot(changeMandoGarra);
    public void PlayLintern() => PlayOneShot(lintern);
    public void PlayTakeObject() => PlayOneShot(winTakeObject);
    public void PlayFallObject() => PlayOneShot(wrongTakeObject);
    public void PlayClockAlarm() => PlayOneShot(clockAlarm);

    public void PlaySfx(EventReference sfx)
    {
        RuntimeManager.PlayOneShot(sfx);
    }

    // ------------------- LIMPIEZA -------------------
    private void OnDestroy()
    {
        StopMusic();
        StopSnapshot();

        if (engineAscInstance.isValid()) engineAscInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        if (engineDescInstance.isValid()) engineDescInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
