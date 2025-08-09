using UnityEngine;

public class MapScroll : MonoBehaviour
{
    [SerializeField] private Material _mat;
    [SerializeField] private float speed = 0.1f;
    private Vector2 currentOffset;
    private bool isPaused;
    void Start()
    {
        _mat = GetComponent<SpriteRenderer>().material;

        if (_mat == null)
        {
            Debug.LogError("Material not found");
        }

        GameManager.Instance.OnStartScroll += ResumeScroll;
        GameManager.Instance.OnStopScroll += PauseScroll;


    }

    void Update()
    {
        if (!isPaused)
        {
            currentOffset.y += speed * Time.deltaTime;
            currentOffset.y %= 1f; // evita valores muy grandes
            _mat.SetVector("_Offset", currentOffset);
        }
    }

    public void PauseScroll()
    {
        isPaused = true;
    }

    public void ResumeScroll()
    {
        isPaused = false;
    }
    void OnDestroy()
    {
        GameManager.Instance.OnStartScroll -= ResumeScroll;
        GameManager.Instance.OnStopScroll -= PauseScroll;
    }
}
