using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panel interno de créditos")]
    [SerializeField] private GameObject creditsPanel;

    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private string albumSceneName = "AlbumScene";

    public void PlayGame()
    {
        bool tienePartidaPausada = PlayerPrefs.GetInt("HasPausedGame", 0) == 1;

        if (tienePartidaPausada)
            PlayerPrefs.SetInt("ResumeAfterLoad", 1);
        else
            PlayerPrefs.SetInt("StartGameOnLoad", 1);

        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenAlbum()
    {
        SceneManager.LoadScene(albumSceneName);
    }

    public void ShowCredits()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Salir del juego");
        Application.Quit();
    }
}



