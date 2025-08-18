using UnityEngine;

public class SfxButtonTap : MonoBehaviour
{
    public void PlaySfxTapSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayTapButton();
        }
        else
        {
            Debug.LogWarning("No se encontro el AudioManager en la escena o en memoria.");
        }
    }
}
