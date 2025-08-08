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
         
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isPaused)
            {
                ResumeScroll();
            }
            else
            {
                PauseScroll();
            }

        }

        if (!isPaused)
        {
            currentOffset.y += speed * Time.deltaTime;
            currentOffset.y %= 1f; // evita valores muy grandes
            _mat.SetVector("_Offset", currentOffset);
        }
    }

    private void StopScroll()
    {
        _mat.SetVector("_Speed", new Vector2(0, 0));
    }


    public void PauseScroll()
    {
        isPaused = true;
    }

    public void ResumeScroll()
    {
        isPaused = false;
    }
}
