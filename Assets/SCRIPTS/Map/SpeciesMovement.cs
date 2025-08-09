using System;
using UnityEngine;

public class SpeciesMovement : MonoBehaviour
{
    private bool isPaused;
    [SerializeField] float speed = 0.1f;

    void Start()
    {
        GameManager.Instance.OnStartScroll += ResumeScroll;
        GameManager.Instance.OnStopScroll += PauseScroll;
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
        //if (Input.GetKeyDown(KeyCode.Space)) StopMovement();

        if (!isPaused)
        {
            Transform trans = transform;
            Vector3 pos = trans.position; // copiamos la posición actual
            pos.y += Time.deltaTime * speed; // modificamos Y
            trans.position = pos; // reasignamos
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
