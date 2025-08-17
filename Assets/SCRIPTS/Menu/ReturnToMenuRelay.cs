using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReturnToMenuRelay : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "Menu";
    [SerializeField] private UnityEngine.UI.Button selfButton; // opcional

    public void ReturnToMenu()
    {
        AudioManager.Instance.PlayTapButton();

        // Evitar dobles clics durante el load
        if (selfButton) selfButton.interactable = false;

        Time.timeScale = 1f; // por si estabas en pausa
        SceneManager.LoadScene(menuSceneName);

        // IMPORTANTE: reanudar música por si estabas en el álbum
        AudioManager.Instance.ResumeMusic();

        RuntimeManager.GetBus("bus:/AudiosDescriptivos").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

    }
}
