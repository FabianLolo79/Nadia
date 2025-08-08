using UnityEngine;
using System;



public class FollowClaw : MonoBehaviour
{
    [SerializeField] private Material _mat;

    [SerializeField] private Transform _clawTransform;

    void Start()
    {
        _mat = GetComponent<SpriteRenderer>().material;

        if (_mat == null)
        {
            Debug.LogError("Material not found");
        }

        if (_clawTransform == null)
        {
            Debug.LogError("Claw not assigned");
        }
    }

    void Update()
    {
        Vector3 realDeal = new Vector3(-0.3f * _clawTransform.position.x + 0.5f, -0.3f * _clawTransform.position.y + 0.5f);
    
        _mat.SetVector("_LightPos", realDeal);
    }
}
