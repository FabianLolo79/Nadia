using System;
using UnityEngine;

public class SpeciesMovement : MonoBehaviour
{
    private bool isPaused;

    [SerializeField] private float speed = 0.1f;
    [SerializeField] private float minSpeed = -3f; // Velocidad mínima permitida
    [SerializeField] private float maxSpeed = -1f; // Velocidad máxima permitida
    [SerializeField] private float velocity;

    void Start()
    {
        GameManager.Instance.OnStartScroll += ResumeScroll;
        GameManager.Instance.OnStopScroll += PauseScroll;
        GameManager.Instance.OnGamePause += PauseScroll;
        GameManager.Instance.OnGameResume += ResumeScroll;

    }

    public void PauseScroll()
    {
        isPaused = true;
    }

    public void ResumeScroll()
    {
        isPaused = false;
    }

void Update()
{
        if (!isPaused)
        {
            Transform trans = transform;
            Vector3 pos = trans.position;

            velocity = speed * GameManager.Instance.DifficultyMult * 0.001f;

            // Forzamos a que sea hacia abajo
            velocity *= -0.1f;

            // Limita la velocidad
            velocity = Mathf.Clamp(velocity, minSpeed, maxSpeed);

            pos.y += velocity * 0.05f;
            trans.position = pos;
        }
}
    private void StopMovement()
    {
        if (!isPaused)
        {
            isPaused = true;
        }
        else
        {
            isPaused = false;
        }
    }

    void OnDestroy()
    {
        GameManager.Instance.OnStartScroll -= ResumeScroll;
        GameManager.Instance.OnStopScroll -= PauseScroll;
    }
}
