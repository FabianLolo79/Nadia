using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseTextUI; // Texto UI que dice "Juego en pausa"
    [SerializeField] private GameObject PauseMenu;
    private bool isPaused = false;
    public float pauseTime = 0.1f;  // Ajustalo según necesites


    void Update()
    {
        Debug.Log(Time.timeScale);
    }



    public void Menu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }


    public void Pause()
    {
        GameManager.Instance.PauseTimer();

        isPaused = true;

        PauseMenu.SetActive(true);


        pauseTextUI.SetActive(false);
        // if (pauseTextUI != null)
        // pauseTextUI.SetActive(true);
    }
    public void Resume()
    {
        GameManager.Instance.UnpauseTimer();
        pauseTextUI.SetActive(true);

        isPaused = false;

        PauseMenu.SetActive(false);
    }


}
