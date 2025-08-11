using UnityEngine;
using System;

public class FishQTE : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public RectTransform safeZone;
    public float pointerSpeed = 40f;
    public static FishQTE Instance { get; private set; }

    public event Action<bool> OnQTEFinished; 
    // true = éxito, false = fallo

    private RectTransform pointerTransform;
    private Vector3 targetPosition;
    private bool isRunning = false;

    public bool IsRunning => isRunning;

        private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }

    public void StartQTE()
    {
        pointerTransform = GetComponent<RectTransform>();
        pointerTransform.position = pointA.position;
        targetPosition = pointB.position;
        isRunning = true;
    }

    void Update()
    {
        if (!isRunning) return;
    // Aumenta velocidad, pero cada vez menos rápido

        // Limita la velocidad
        pointerTransform.position = Vector3.MoveTowards(pointerTransform.position, targetPosition, pointerSpeed + GameManager.Instance.DifficultyMult * 0.3f);

        // Cambiar dirección
        if (Vector3.Distance(pointerTransform.position, pointA.position) < 0.1f)
        {
            targetPosition = pointB.position;
        }
        else if (Vector3.Distance(pointerTransform.position, pointB.position) < 0.1f)
        {
            // Llegó a punto B, termina automáticamente si no apretó nada
            FinishQTE(false);
        }

        // Input jugador
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            bool success = RectTransformUtility.RectangleContainsScreenPoint(
                safeZone,
                pointerTransform.position,
                null
            );
            FinishQTE(success);
        }
        }

    void FinishQTE(bool success)
    {
        isRunning = false;

        OnQTEFinished?.Invoke(success);
    }
}