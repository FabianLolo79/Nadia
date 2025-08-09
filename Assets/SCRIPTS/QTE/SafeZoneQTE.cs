using UnityEngine;
public class SafeZoneQTE : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float margin = 50f; // margen en píxeles para que no toque el borde

    private RectTransform safeZoneRect;

    void Awake()
    {
        safeZoneRect = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        RandomizePosition();
    }

    public void RandomizePosition()
    {
        // Tomamos las posiciones en el espacio del canvas
        Vector3 aPos = pointA.position;
        Vector3 bPos = pointB.position;

        // Calculamos la distancia total entre A y B
        float totalDistance = Vector3.Distance(aPos, bPos);

        // Obtenemos un valor aleatorio con margen
        float randomOffset = Random.Range(margin, totalDistance - margin);

        // Dirección normalizada entre A y B
        Vector3 direction = (bPos - aPos).normalized;

        // Posicionamos la safeZone
        safeZoneRect.position = aPos + direction * randomOffset;
    }
}

