using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class AttackIndicatorCircle : MonoBehaviour
{
    public Transform growEffect;
    private float _maxDist;
    private float _distance;
    private float _velocity;
    private float _size = 0;
    private bool bStart = false;
    private void Start()
    {
        _size = 0;
    }

    private void Update()
    {
        if (bStart == false) return;
        _distance -= _velocity * Time.deltaTime;
        _size = (_maxDist - _distance) / _maxDist;
        growEffect.localScale = new Vector3(_size, _size, _size);

        if(_size >= 1.0f)
        {
            Destroy(gameObject);
        }
    }

    public void Set(float dist, float velocity, float scale)// set scale?
    {
        transform.localScale = Vector3.one * scale * 5.0f;
        _maxDist = dist;
        _distance = dist;
        _velocity = velocity;
        bStart = true;
    }
}
