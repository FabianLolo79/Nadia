using System.Collections;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    [SerializeField] float lifeTime;

    void Start()
    {
        StartCoroutine(DestroyAfterr(lifeTime));
    }

    private IEnumerator DestroyAfterr(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);    
    }
}
