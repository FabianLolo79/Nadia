using UnityEngine;

public class SpeciesMovement : MonoBehaviour
{
    private bool isPaused;
    [SerializeField] float speed = 0.1f;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) StopMovement();

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
}
