using UnityEngine;
using FMODUnity;

public class PlaySoundSfx : MonoBehaviour
{
    [SerializeField] private EventReference sfxEvent;

    public void PlaySfx()
    {
       // AudioManager.Instance.PlaySfx(sfxEvent);
    }
}
