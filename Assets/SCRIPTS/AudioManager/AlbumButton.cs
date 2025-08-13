using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class AlbumButton : MonoBehaviour
{
    // Asignás el ScriptableObject que tiene el EventReference con el audio descriptivo
    public SpeciesSO speciesData;

    // Método público para asignar al OnClick del botón
    public void PlaySpeciesSFX()
    {
        if (speciesData != null)
        {
            AudioManager.Instance.PlayOneShot(speciesData.audio_descriptivo);
        }
        else
        {
            Debug.LogWarning("No se asignó SpeciesSO en " + gameObject.name);
        }
    }

}