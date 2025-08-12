using UnityEngine;
using UnityEngine.SceneManagement;

public class BannerSceneLoader : MonoBehaviour
{
    [SerializeField] private string timeUpSceneName = "TimeOut";
    [SerializeField] private string albumCompletedSceneName = "Congratulations";

    private void OnEnable()
    {
        EventsManager.Instance.OnTimeUp += LoadTimeUpScene;
        EventsManager.Instance.OnAlbumCompleted += LoadAlbumCompletedScene;
    }

    private void OnDisable()
    {
        EventsManager.Instance.OnTimeUp -= LoadTimeUpScene;
        EventsManager.Instance.OnAlbumCompleted -= LoadAlbumCompletedScene;
    }

    private void LoadTimeUpScene()
    {
        SceneManager.LoadScene(timeUpSceneName);
    }

    private void LoadAlbumCompletedScene()
    {
        SceneManager.LoadScene(albumCompletedSceneName);
    }
}
