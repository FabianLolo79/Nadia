using UnityEngine;

public class StopSounds : MonoBehaviour
{

    public void PauseMusic()
    {
        AudioManager.Instance.PauseMusic();
        AudioManager.Instance.PauseGameMusicWithEngine();
    }

    public void StartMusic()
    {
        AudioManager.Instance.ResumeMusic();
        AudioManager.Instance.ResumeGameMusicWithEngine();
    }
}
