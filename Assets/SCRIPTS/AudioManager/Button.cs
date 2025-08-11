using UnityEngine;

public class Button : MonoBehaviour
{
    public void PlayTapSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayTapButton();
        }
        else
        {
            Debug.LogWarning("No se encontró el AudioManager en la escena o en memoria.");
        }
    }

}
