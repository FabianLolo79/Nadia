using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;
using TMPro;

public class SpeciesEntry : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI idText;



    [Header("Colors")]
    [SerializeField] private Color collectedColor = Color.white;
    [SerializeField] private Color notCollectedColor = new Color(1f, 1f, 1f, 1f);

    [SerializeField] private EventReference descriptiveAudio;

    // Estáticas: compartidas por TODAS las instancias
    private static bool isAnyAudioPlaying = false;
    private static EventInstance currentAudioInstance;

    public void Setup(SpeciesSO species, bool collected)
    {
        iconImage.sprite = species.speciesImage;
        iconImage.color = collected ? collectedColor : notCollectedColor;
        descriptiveAudio = species.audio_descriptivo;

        if (idText != null) 
            idText.text = species.speciesID;
    }

    public void PlayDescriptiveAudio()
    {
        // 1. Si ya hay audio sonando, lo paramos inmediatamente
        if (isAnyAudioPlaying && currentAudioInstance.isValid())
        {
            currentAudioInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            currentAudioInstance.release();
            isAnyAudioPlaying = false;
        }

        // 2. Si no hay audio asignado, nos vamos
        if (descriptiveAudio.IsNull) return;

        // 3. Creamos y lanzamos el nuevo
        currentAudioInstance = RuntimeManager.CreateInstance(descriptiveAudio);
        currentAudioInstance.start();
        currentAudioInstance.release(); // se auto-libera al terminar
        isAnyAudioPlaying = true;

        // 4. Detectar cuando termine para liberar la bandera
        CheckAudioEnd();
    }

    //public void PlayDescriptiveAudio()
    //{
    //    // Si ya hay un audio en reproducción, no hacer nada
    //    if (isAnyAudioPlaying)
    //    {
    //        Debug.Log("Ya hay un audio reproduciéndose, espera a que termine.");
    //        return;
    //    }

    //    if (!descriptiveAudio.IsNull)
    //    {
    //        // Marcar que hay audio en reproducción
    //        isAnyAudioPlaying = true;

    //        // Crear instancia del evento para controlar fin de reproducción
    //        currentAudioInstance = RuntimeManager.CreateInstance(descriptiveAudio);
    //        currentAudioInstance.start();

    //        // 🔹 Detectar cuando el audio termina
    //        currentAudioInstance.release(); // Libera memoria al finalizar
    //        CheckAudioEnd();
    //    }
    //}



    private async void CheckAudioEnd()
    {
        // Revisar el estado del evento periódicamente
        PLAYBACK_STATE state;
        do
        {
            await System.Threading.Tasks.Task.Delay(100);
            currentAudioInstance.getPlaybackState(out state);
        }
        while (state != PLAYBACK_STATE.STOPPED);

        // Al terminar, resetear la bandera
        isAnyAudioPlaying = false;
    }
}



