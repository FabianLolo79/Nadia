using UnityEngine;
using UnityEngine.InputSystem;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private Joystick _joystick;
    public static Vector2 InputDirection {  get; private set; }

    private void Update()
    {
        Vector2 input = new Vector2(_joystick.Horizontal, _joystick.Vertical);
        InputDirection = input.magnitude > 0.1f ? input.normalized : Vector2.zero;
    }
}
