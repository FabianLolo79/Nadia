using UnityEngine;
using System;

public class FishQTE : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public RectTransform safeZone;
    public float moveSpeed = 100f;

    public event Action<bool> OnQTEFinished; 
    // true = éxito, false = fallo

    private RectTransform pointerTransform;
    private Vector3 targetPosition;
    private bool isRunning = false;

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

        // Mover puntero
        pointerTransform.position = Vector3.MoveTowards(pointerTransform.position, targetPosition, moveSpeed * Time.deltaTime);

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool success = RectTransformUtility.RectangleContainsScreenPoint(safeZone, pointerTransform.position, null);
            FinishQTE(success);
        }
    }

    void FinishQTE(bool success)
    {
        isRunning = false;
        OnQTEFinished?.Invoke(success);
    }
}