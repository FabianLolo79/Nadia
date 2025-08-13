using UnityEngine;

public class StartSnapshotPause : MonoBehaviour
{
    public void OnClickStartPause()
    {
        // Activa el snapshot para reducir los volúmenes
        AudioManager.Instance.StartSnapshotEnPausa();
    }
}

