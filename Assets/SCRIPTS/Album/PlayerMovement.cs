using UnityEngine;

/// <summary>
/// Movimiento básico y provisorio del jugador
/// para pruebas y testing de mecánicas.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f; // Velocidad de desplazamiento

    void Update()
    {
        // Lee el input del jugador en ambos ejes
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        // Mueve el objeto en base al input y la velocidad
        transform.Translate(new Vector2(x, y) * speed * Time.deltaTime);
    }
}

