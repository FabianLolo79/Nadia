using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

public class SpeciesEntry : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image iconImage;

    [Header("Colors")]
    [SerializeField] private Color collectedColor = Color.white;
    [SerializeField] private Color notCollectedColor = new Color(0.4f, 0.4f, 0.4f, 0.4f);

    [SerializeField] private EventReference descriptiveAudio;

    // 🔹 Bandera estática: compartida por TODAS las instancias
    private static bool isAnyAudioPlaying = false;

    // 🔹 Instancia del evento para poder detenerlo
    private EventInstance currentAudioInstance;

    public void Setup(SpeciesSO species, bool collected)
    {
        iconImage.sprite = species.speciesImage;
        iconImage.color = collected ? collectedColor : notCollectedColor;
        descriptiveAudio = species.audio_descriptivo;
    }

    public void PlayDescriptiveAudio()
    {
        // Si ya hay un audio en reproducción, no hacer nada
        if (isAnyAudioPlaying)
        {
            Debug.Log("Ya hay un audio reproduciéndose, espera a que termine.");
            return;
        }

        if (!descriptiveAudio.IsNull)
        {
            // Marcar que hay audio en reproducción
            isAnyAudioPlaying = true;

            // Crear instancia del evento para controlar fin de reproducción
            currentAudioInstance = RuntimeManager.CreateInstance(descriptiveAudio);
            currentAudioInstance.start();

            // 🔹 Detectar cuando el audio termina
            currentAudioInstance.release(); // Libera memoria al finalizar
            CheckAudioEnd();
        }
    }

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



