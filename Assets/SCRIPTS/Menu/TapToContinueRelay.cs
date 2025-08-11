using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class TapToContinueRelay : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private ReturnToMenuRelay relay;
    [SerializeField] private Image tapIcon;

    [Header("Efectos")]
    [SerializeField] private float blinkSpeed = 1.2f;
    [SerializeField] private float fadeOutOnTap = 0.18f;

    [Header("Opcional")]
    [SerializeField] private bool ignoreTapOverUI = false; // true si querés que no tome taps sobre UI

    private bool touched;

    void Start()
    {
        if (tapIcon)
        {
            tapIcon.gameObject.SetActive(true);
            StartCoroutine(BlinkIcon());
        }
    }

    void Update()
    {
        if (touched) return;

        bool anyTap = Input.GetMouseButtonDown(0) || Input.touchCount > 0;
        if (!anyTap) return;

        if (ignoreTapOverUI && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        touched = true;
        if (tapIcon && fadeOutOnTap > 0f) StartCoroutine(FadeOutThenGo());
        else relay?.ReturnToMenu();
    }

    IEnumerator BlinkIcon()
    {
        while (!touched)
        {
            float a = Mathf.PingPong(Time.unscaledTime * blinkSpeed, 1f);
            var c = tapIcon.color; c.a = a; tapIcon.color = c;
            yield return null;
        }
    }

    IEnumerator FadeOutThenGo()
    {
        float t = 0f;
        Color c = tapIcon.color;
        float from = c.a, to = 0f;

        while (t < fadeOutOnTap)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(from, to, t / fadeOutOnTap);
            tapIcon.color = c;
            yield return null;
        }

        relay?.ReturnToMenu();
    }
}
