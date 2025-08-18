using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BannerSceneLoader : MonoBehaviour
{
    [SerializeField] private string timeUpSceneName = "TimeOut";
    [SerializeField] private string albumCompletedSceneName = "Congratulations";

    private bool _subscribed;

    private void OnEnable()
    {
        TrySubscribe();
        if (!_subscribed) StartCoroutine(WaitAndSubscribe());

        EventsManager.Instance.OnAlbumCompleted += LoadAlbumCompletedScene;
    }

    private IEnumerator WaitAndSubscribe()
    {
        // Espera hasta que EventsManager est� listo
        while (EventsManager.Instance == null) yield return null;
        TrySubscribe();
    }

    private void TrySubscribe()
    {
        if (EventsManager.Instance != null && !_subscribed)
        {
            EventsManager.Instance.OnTimeUp += HandleTimeUp;
            EventsManager.Instance.OnCongrats += LoadAlbumCompletedScene;

            _subscribed = true;
        }
    }

    private void OnDisable()
    {
        if (_subscribed && EventsManager.Instance != null)
            EventsManager.Instance.OnTimeUp -= HandleTimeUp;

        _subscribed = false;

        EventsManager.Instance.OnAlbumCompleted -= LoadAlbumCompletedScene;
    }

    private void LoadAlbumCompletedScene()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(albumCompletedSceneName);
    }

    private void HandleTimeUp()
    {
        // Descongelar por si el GameManager dej� el tiempo en 0
        Time.timeScale = 1f;
        SceneManager.LoadScene(timeUpSceneName);
    }

    void OnDestroy()
    {
            EventsManager.Instance.OnTimeUp -= HandleTimeUp;
            EventsManager.Instance.OnCongrats -= LoadAlbumCompletedScene;
    }
}
