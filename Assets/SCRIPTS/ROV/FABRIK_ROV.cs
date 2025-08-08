using UnityEngine;

public class FABRIK_ROV : MonoBehaviour
{
    [Header("Configuración de la garra")]
    public Transform[] joints;               // Segment0, Segment1, ..., Pinza
    public float[] segmentLengths;           // Largo de cada segmento
    //public Transform target;

    [Header("Detección de objetivo para la pinza")]
    public float detectionRadius = 2f;
    public float tipAimSpeedDeg = 180f;      // Velocidad de rotación en la detencción de obejtivos (Feeling))

    [Header("Parámetros de precisión")]
    public float armMoveSpeed = 6f;          // Velocidad de desplazamiento  de la garra
    public float reachThreshold = 0.01f;
    public int maxIterations = 10;

    private Vector2[] positions;

    private Vector2 filteredTarget;
    private bool initialized;

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

        // Movimiento dinámico de la garra mediante el mouse
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        Vector2 rawTarget = mouseWorldPos;

        if (!initialized)
        {
            filteredTarget = rawTarget;
            initialized = true;
        }
        else
        {
            filteredTarget = Vector2.MoveTowards(filteredTarget, rawTarget, armMoveSpeed * Time.deltaTime);
        }

        float totalLength = 0f;
        foreach (var len in segmentLengths) totalLength += len;

        float distToTarget = Vector2.Distance(basePosition,filteredTarget);

        if (distToTarget > totalLength)
        {
            // Estirar en línea recta hacia el objetivo
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 dir = (filteredTarget - positions[i - 1]).normalized;
                positions[i] = positions[i - 1] + dir * segmentLengths[i - 1];
            }
        }
        else
        {
            // FABRIK
            int iterations = 0;
            while ((positions[positions.Length - 1] - filteredTarget).sqrMagnitude > reachThreshold * reachThreshold &&
                   iterations < maxIterations)
            {
                // Backward
                positions[positions.Length - 1] = filteredTarget;
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
            joints[i].rotation = Quaternion.Euler(0, 0, angle - 90); // +90 para vertical
        }

        // Posicionar y rotar la pinza al final
        int last = joints.Length - 1;
        joints[last].position = positions[last];

        Vector2 pinzaPos = joints[last].position;
        GameObject closest = FindClosestTarget(joints[last].position);

        if (closest != null && Vector2.Distance(joints[last].position, closest.transform.position) <= detectionRadius)
        {
            Vector2 aimDir = (Vector2)closest.transform.position - (Vector2)joints[last].position;
            float desired = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90f;
            float current = joints[last].eulerAngles.z;
            float newZ = Mathf.MoveTowardsAngle(current, desired, tipAimSpeedDeg * Time.deltaTime);
            joints[last].rotation = Quaternion.Euler(0, 0, newZ);
        }
        else
        {
            // Si no hay objetivo, mira la dirección del último segmento
            Vector2 dirUltimo = positions[last] - positions[last - 1];
            float desired = Mathf.Atan2(dirUltimo.y, dirUltimo.x) * Mathf.Rad2Deg - 90f;
            float current = joints[last].eulerAngles.z;
            float newZ = Mathf.MoveTowardsAngle(current, desired, tipAimSpeedDeg * Time.deltaTime);
            joints[last].rotation = Quaternion.Euler(0, 0, newZ);
        }
    }
    GameObject FindClosestTarget(Vector2 fromPosition)
    {
        float minDist = Mathf.Infinity;
        GameObject closest = null;

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Target"))
        {
            float dist = Vector2.Distance(fromPosition, g.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = g;
            }
        }

        return closest;
    }
    void OnDrawGizmosSelected()
    {
        if (joints != null && joints.Length > 0)
        {
            Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.4f);
            Gizmos.DrawWireSphere(joints[joints.Length - 1].position, detectionRadius);
        }
    }
}
