using UnityEngine;

public class FABRIK_ROV : MonoBehaviour
{
    [Header("Configuración de Joystick")]
    [SerializeField] private Joystick _joystick;
    [SerializeField] private float _joystickRange = 5f;                  // tope manual (en unidades de mundo)
    [SerializeField, Range(0f, 0.5f)] private float _deadZone = 0.1f;
    [SerializeField] private bool _snapToMaxWhenFullTilt = true;         // a tope del pad => alcance máximo
    private float _maxReach;                                             // suma de segmentLengths

    [Header("Configuración de la garra")]
    public Transform[] joints;               // Segment0, Segment1, ..., Pinza
    public float[] segmentLengths;           // Largo de cada segmento

    [Header("Detección de objetivo para la pinza")]
    public float detectionRadius = 2f;
    public float tipAimSpeedDeg = 180f;

    [Header("Parámetros de precisión")]
    public float armMoveSpeed = 6f;          // velocidad de seguimiento del objetivo (feeling)
    public float reachThreshold = 0.01f;
    public int maxIterations = 20;

    private Vector2[] _positions;
    private Vector2 _filteredTarget;          // objetivo filtrado (suavizado)

    void Start()
    {
        if (joints.Length != segmentLengths.Length + 1)
        {
            Debug.LogError("La cantidad de segmentos debe ser joints.Length - 1");
            enabled = false;
            return;
        }

        _positions = new Vector2[joints.Length];

        // Alinear sprites 90° para verse verticales
        foreach (Transform joint in joints)
        {
            var sr = joint.GetComponent<SpriteRenderer>();
            if (sr != null) joint.localRotation = Quaternion.Euler(0, 0, 90);
        }

        // Alcance máximo real del brazo
        _maxReach = 0f;
        foreach (var len in segmentLengths) _maxReach += len;

        // Arrancar filtrado desde la posición actual de la punta
        _filteredTarget = joints[joints.Length - 1].position;
    }

    void Update()
    {
        // Snapshot posiciones actuales
        for (int i = 0; i < joints.Length; i++)
            _positions[i] = joints[i].position;

        Vector2 basePosition = _positions[0];

        // ===== INPUT JOYSTICK =====
        Vector2 input = (_joystick != null) ? new Vector2(_joystick.Horizontal, _joystick.Vertical) : Vector2.zero;

        // Deadzone
        if (input.sqrMagnitude < _deadZone * _deadZone) input = Vector2.zero;

        Vector2 dir = (input == Vector2.zero) ? Vector2.zero : input.normalized;

        // Magnitud del pad (0..1) => distancia objetivo
        float distFactor = input.magnitude; // 0..1

        // Límite de distancia: menor entre alcance real y _joystickRange
        float maxUsableReach = Mathf.Min(_maxReach, _joystickRange);
        float targetDist = distFactor * maxUsableReach;

        // Objetivo crudo en mundo
        Vector2 rawTarget = (dir == Vector2.zero) ? _filteredTarget : (basePosition + dir * targetDist);

        // Suavizado (feeling)
        _filteredTarget = Vector2.MoveTowards(_filteredTarget, rawTarget, armMoveSpeed * Time.deltaTime);

        // ===== FABRIK =====
        float totalLength = _maxReach; // suma de segmentLengths calculada en Start
        float distToTarget = Vector2.Distance(basePosition, _filteredTarget);

        if (distToTarget > totalLength)
        {
            // Estirar en línea recta hacia el objetivo
            for (int i = 1; i < _positions.Length; i++)
            {
                Vector2 direction = (_filteredTarget - _positions[i - 1]).normalized;
                _positions[i] = _positions[i - 1] + direction * segmentLengths[i - 1];
            }
        }
        else
        {
            int iterations = 0;
            while ((_positions[_positions.Length - 1] - _filteredTarget).sqrMagnitude > reachThreshold * reachThreshold
                   && iterations < maxIterations)
            {
                // Backward
                _positions[_positions.Length - 1] = _filteredTarget;
                for (int i = _positions.Length - 2; i >= 0; i--)
                {
                    Vector2 direction = (_positions[i] - _positions[i + 1]).normalized;
                    _positions[i] = _positions[i + 1] + direction * segmentLengths[i];
                }

                // Forward
                _positions[0] = basePosition;
                for (int i = 1; i < _positions.Length; i++)
                {
                    Vector2 direction = (_positions[i] - _positions[i - 1]).normalized;
                    _positions[i] = _positions[i - 1] + direction * segmentLengths[i - 1];
                }

                iterations++;
            }
        }

        // Aplicar posiciones y rotaciones
        for (int i = 0; i < joints.Length - 1; i++)
        {
            joints[i].position = _positions[i];

            if (i == 0)
            {
                joints[i].rotation = Quaternion.identity; // base fija sin rotación
                continue;
            }

            Vector2 direction = _positions[i + 1] - _positions[i];
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            joints[i].rotation = Quaternion.Euler(0, 0, angle - 90);
        }

        // Pinza
        int last = joints.Length - 1;
        joints[last].position = _positions[last];

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
            Vector2 dirUltimo = _positions[last] - _positions[last - 1];
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
            if (dist < minDist) { minDist = dist; closest = g; }
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
