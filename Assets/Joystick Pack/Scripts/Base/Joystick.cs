using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public float Horizontal { get { return (snapX) ? SnapFloat(input.x, AxisOptions.Horizontal) : input.x; } }
    public float Vertical { get { return (snapY) ? SnapFloat(input.y, AxisOptions.Vertical) : input.y; } }
    public Vector2 Direction { get { return new Vector2(Horizontal, Vertical); } }

    public float HandleRange
    {
        get { return handleRange; }
        set { handleRange = Mathf.Abs(value); }
    }

    public float DeadZone
    {
        get { return deadZone; }
        set { deadZone = Mathf.Abs(value); }
    }

    public AxisOptions AxisOptions { get { return AxisOptions; } set { axisOptions = value; } }
    public bool SnapX { get { return snapX; } set { snapX = value; } }
    public bool SnapY { get { return snapY; } set { snapY = value; } }

    [Header("Joystick")]
    [SerializeField] private float handleRange = 1;
    [SerializeField] private float deadZone = 0;
    [SerializeField] private AxisOptions axisOptions = AxisOptions.Both;
    [SerializeField] private bool snapX = false;
    [SerializeField] private bool snapY = false;

    [Header("SFX")]
    [SerializeField] private EventReference garra;
    private EventInstance garraInstance;
    private bool garraSonando = false;

    [SerializeField] protected RectTransform background = null;
    [SerializeField] private RectTransform handle = null;
    private RectTransform baseRect = null;

    private Canvas canvas;
    private Camera cam;

    private Vector2 input = Vector2.zero;

    // === Nuevo: Doble toque / Scroll ===
    [Header("Doble toque / Scroll")]
    [Tooltip("Tiempo máximo entre toques para contar como doble toque")]
    [SerializeField] private float doubleTapMaxDelay = 0.3f;
    private float lastTapTime = -1f;

    [SerializeField] private bool scrollMode = false;


    [Header("UI Icono")]
    public Image icon;            // ícono a cambiar
    public Sprite normalIcon;     // ícono normal
    public Sprite scrollIcon;     // ícono cuando está en modo scroll


    protected virtual void Start()
    {
        HandleRange = handleRange;
        DeadZone = deadZone;
        baseRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        Debug.LogError("The Joystick is not placed inside a canvas");

        Vector2 center = new Vector2(0.5f, 0.5f);
        background.pivot = center;
        handle.anchorMin = center;
        handle.anchorMax = center;
        handle.pivot = center;
        handle.anchoredPosition = Vector2.zero;

        UpdateIcon();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        // Detectar doble toque
        float now = Time.unscaledTime;
        if (now - lastTapTime <= doubleTapMaxDelay)
        {
            // Doble toque: alternar modo scroll
            ToggleScroll();
            lastTapTime = -1f; // reset
            return;            // no empezamos drag si fue doble toque
        }

        lastTapTime = now;

        if (!scrollMode) OnDrag(eventData);

    }
           

    public void OnDrag(PointerEventData eventData)
    {
        //if (scrollMode) return;

        cam = null;
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            cam = canvas.worldCamera;

        Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
        Vector2 radius = background.sizeDelta / 2;
        input = (eventData.position - position) / (radius * canvas.scaleFactor);
        FormatInput();

        // Comienza el sonido la primera vez que se mueve
        if (!garraSonando)
        {
            garraInstance = RuntimeManager.CreateInstance(garra);
            garraInstance.start();
            garraSonando = true;
        }
        //else NO FUNCIONA PARA EL FINAL DEL TIMER
        //{
        //    garraInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        //    garraSonando = false;

        //}


            HandleInput(input.magnitude, input.normalized, radius, cam);
        handle.anchoredPosition = input * radius * handleRange;

       
    }

    protected virtual void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (magnitude > deadZone)
        {
            if (magnitude > 1)
                input = normalised;

        }
        else
            input = Vector2.zero;

        
    }

    private void FormatInput()
    {
        if (axisOptions == AxisOptions.Horizontal)
            input = new Vector2(input.x, 0f);
        else if (axisOptions == AxisOptions.Vertical)
            input = new Vector2(0f, input.y);
    }

    private float SnapFloat(float value, AxisOptions snapAxis)
    {
        if (value == 0)
            return value;

        if (axisOptions == AxisOptions.Both)
        {
            float angle = Vector2.Angle(input, Vector2.up);
            if (snapAxis == AxisOptions.Horizontal)
            {
                if (angle < 22.5f || angle > 157.5f)
                    return 0;
                else
                    return (value > 0) ? 1 : -1;
            }
            else if (snapAxis == AxisOptions.Vertical)
            {
                if (angle > 67.5f && angle < 112.5f)
                    return 0;
                else
                    return (value > 0) ? 1 : -1;
            }
            return value;
        }
        else
        {
            if (value > 0)
                return 1;
            if (value < 0)
                return -1;
        }
        return 0;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        //if (scrollMode) return; // en modo scroll no reseteamos la palanca (ya está inmóvil)
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;

        // Para el sonido al soltar
        if (garraSonando)
        {
            garraInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            garraInstance.release();
            garraSonando = false;
        }
    }

    protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
    {
        Vector2 localPoint = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPosition, cam, out localPoint))
        {
            Vector2 pivotOffset = baseRect.pivot * baseRect.sizeDelta;
            return localPoint - (background.anchorMax * baseRect.sizeDelta) + pivotOffset;
        }
        return Vector2.zero;
    }

    private void ToggleScroll()
    {
        scrollMode = !scrollMode;

        if (scrollMode)
        {
            // Reset visual del joystick al entrar a scroll
            input = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
        }

        // Notificar al GameManager
        if (GameManager.Instance != null)
        {
            if (scrollMode) GameManager.Instance.TriggerStartScroll();
            else GameManager.Instance.TriggerStopScroll();
        }
        else
        {
            Debug.LogWarning("GameManager.Instance no encontrado en la escena.");
        }

        UpdateIcon();
    }

    private void UpdateIcon()
    {
        if (icon != null)
        {
            icon.sprite = scrollMode ? scrollIcon : normalIcon;
        }

    }

    private void OnDisable()
    {
        // Detener y liberar el sonido de la garra al desactivar el objeto
        if (garraInstance.isValid() && garraSonando)
        {
            garraInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            garraInstance.release();
            garraSonando = false;
        }
    }
}

public enum AxisOptions { Both, Horizontal, Vertical }