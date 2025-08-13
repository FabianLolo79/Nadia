using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReturnToMenuRelay : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "Menu";
    [SerializeField] private UnityEngine.UI.Button selfButton; // opcional

    public void ReturnToMenu()
    {
        // Evitar dobles clics durante el load
        if (selfButton) selfButton.interactable = false;

        Time.timeScale = 1f; // por si estabas en pausa
        SceneManager.LoadScene(menuSceneName);

        AudioManager.Instance.PlayMusicMenu();
    }
}
