using UnityEngine;

public class FollowMouse : MonoBehaviour

{
    [SerializeField] private Camera _cam; // Asigna tu cámara si no es la principal

    void Start()
    {
        if (_cam == null)
            _cam = Camera.main;
    }

    void Update()
    {
        // Posición del mouse en coordenadas de mundo
        Vector3 mouseWorldPos = _cam.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, -_cam.transform.position.z)
        );

        // Mantener z original del objeto
        mouseWorldPos.z = transform.position.z;

        // Mover el objeto a esa posición
        transform.position = mouseWorldPos;
    }
}

