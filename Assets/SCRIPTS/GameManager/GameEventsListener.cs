using UnityEngine;

public class GameEventsListener : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.OnGameStart += HandleGameStart;
        GameManager.Instance.OnGamePause += HandleGamePause;
        GameManager.Instance.OnGameResume += HandleGameResume;
        GameManager.Instance.OnGameEnd += HandleGameEnd;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameStart -= HandleGameStart;
        GameManager.Instance.OnGamePause -= HandleGamePause;
        GameManager.Instance.OnGameResume -= HandleGameResume;
        GameManager.Instance.OnGameEnd -= HandleGameEnd;
    }

    private void HandleGameStart() { Debug.Log("Juego iniciado"); }
    private void HandleGamePause() { Debug.Log("Juego pausado"); }
    private void HandleGameResume() { Debug.Log("Juego reanudado"); }
    private void HandleGameEnd() { Debug.Log("Juego terminado"); }
}

