using UnityEngine;
using System;

public class FishQTE : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private RectTransform safeZone;
    [SerializeField] private float pointerSpeed = 40f;
    [SerializeField] private float minPointerSpeed = 2f; // Mínimo permitido
    [SerializeField] private float maxPointerSpeed = 5f; // Máximo permitido

    [SerializeField] private float currentSpeed;
    public static FishQTE Instance { get; private set; }

    public event Action<bool> OnQTEFinished;

    private RectTransform pointerTransform;
    private Vector3 targetPosition;
    private bool isRunning = false;

    public bool IsRunning => isRunning;


    void Start()
    {
        currentSpeed = 0.2f;    
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
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
        if (currentSpeed < 4.2f) currentSpeed = pointerSpeed + GameManager.Instance.DifficultyMult * 0.3f;

        // Limita la velocidad
        pointerSpeed = Mathf.Clamp(pointerSpeed, minPointerSpeed, maxPointerSpeed);

        // Movimiento
        pointerTransform.position = Vector3.MoveTowards(
            pointerTransform.position,
            targetPosition,
            currentSpeed
        );

        // Cambiar dirección o terminar
        if (Vector3.Distance(pointerTransform.position, pointA.position) < 0.1f)
        {
            targetPosition = pointB.position;
        }
        else if (Vector3.Distance(pointerTransform.position, pointB.position) < 0.1f)
        {
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

        // Reproducir sonido según el resultado
        if (success)
        {
            AudioManager.Instance.PlayTakeObject(); // Éxito
        }
        else
        {
            AudioManager.Instance.PlayFallObject(); // Fallo
        }
    }
}