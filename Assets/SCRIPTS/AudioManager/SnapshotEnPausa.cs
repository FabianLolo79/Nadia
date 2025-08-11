using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SnapshotEnPausa : MonoBehaviour
{
    [Header("FMOD Snapshots")]
    [SerializeField] private EventReference snapshotEnPausa;
   // [SerializeField] private EventReference musicaJuego;  Arrastra aquí el evento de música

    private EventInstance activeSnapshot;
   // private EventInstance musicaInstance;
    private bool snapshotActiva = false;

    private void Start()
    {
        // Inicia la música del juego
        //musicaInstance = RuntimeManager.CreateInstance(musicaJuego);
        
    }

    public void ToggleSnapshotEnPausa()
    {
        if (!snapshotActiva)
        {
            activeSnapshot = RuntimeManager.CreateInstance(snapshotEnPausa);
            activeSnapshot.start();
            snapshotActiva = true;
        }
        else
        {
            if (activeSnapshot.isValid())
            {
                activeSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                activeSnapshot.release();
            }
            snapshotActiva = false;
        }
    }

    private void StartSnapshot(EventReference snapshotRef)
    {
        StopSnapshot();
        activeSnapshot = RuntimeManager.CreateInstance(snapshotRef);
        activeSnapshot.start();
    }

    private void StopSnapshot()
    {
        if (activeSnapshot.isValid())
        {
            activeSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            activeSnapshot.release();
        }
    }
}