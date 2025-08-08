using UnityEngine;

public class FABRIKArm : MonoBehaviour
{
    [Header("Configuración de la garra")]
    public Transform[] joints;               // Segment0, Segment1, ..., Pinza
    public float[] segmentLengths;           // Largo de cada segmento
    public Transform target;

    [Header("Parámetros de precisión")]
    public float reachThreshold = 0.01f;
    public int maxIterations = 10;

    private Vector2[] positions;

    void Start()
    {
        if (joints.Length != segmentLengths.Length + 1)
        {
            Debug.LogError("La cantidad de segmentos debe ser joints.Length - 1");
            return;
        }

        positions = new Vector2[joints.Length];

        // Rotar todos los sprites 90° para que se vean verticales
        foreach (Transform joint in joints)
        {
            SpriteRenderer sr = joint.GetComponent<SpriteRenderer>();
            if (sr != null)
                joint.localRotation = Quaternion.Euler(0, 0, 90);
        }
    }

    void Update()
    {
        // Guardar posiciones actuales
        for (int i = 0; i < joints.Length; i++)
            positions[i] = joints[i].position;

        Vector2 basePosition = positions[0];
        float totalLength = 0f;
        foreach (var len in segmentLengths)
            totalLength += len;

        float distToTarget = Vector2.Distance(basePosition, (Vector2)target.position);

        if (distToTarget > totalLength)
        {
            // Estirar en línea recta hacia el objetivo
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 dir = ((Vector2)target.position - positions[i - 1]).normalized;
                positions[i] = positions[i - 1] + dir * segmentLengths[i - 1];
            }
        }
        else
        {
            // FABRIK
            int iterations = 0;
            while ((positions[positions.Length - 1] - (Vector2)target.position).sqrMagnitude > reachThreshold * reachThreshold &&
                   iterations < maxIterations)
            {
                // Backward
                positions[positions.Length - 1] = target.position;
                for (int i = positions.Length - 2; i >= 0; i--)
                {
                    Vector2 dir = (positions[i] - positions[i + 1]).normalized;
                    positions[i] = positions[i + 1] + dir * segmentLengths[i];
                }

                // Forward
                positions[0] = basePosition;
                for (int i = 1; i < positions.Length; i++)
                {
                    Vector2 dir = (positions[i] - positions[i - 1]).normalized;
                    positions[i] = positions[i - 1] + dir * segmentLengths[i - 1];
                }

                iterations++;
            }
        }

        // Aplicar posiciones y rotaciones
        for (int i = 0; i < joints.Length - 1; i++)
        {
            joints[i].position = positions[i];

            if (i == 0)
            {
                joints[i].rotation = Quaternion.identity; // base sin rotación
                continue;
            }

            Vector2 dir = positions[i + 1] - positions[i];
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            joints[i].rotation = Quaternion.Euler(0, 0, angle + -90); // +90 para vertical
        }

        // Posicionar y rotar la pinza al final
        int last = joints.Length - 1;
        joints[last].position = positions[last];

        // Dirección desde el penúltimo punto hacia la pinza (NO hacia el objetivo)
        Vector2 finalDir = positions[last] - positions[last - 1];
        float finalAngle = Mathf.Atan2(finalDir.y, finalDir.x) * Mathf.Rad2Deg;

        // Aplicar rotación con el mismo offset de los demás (+90 si sprite apunta arriba)
        joints[last].rotation = Quaternion.Euler(0, 0, finalAngle + -90);
    }
}
