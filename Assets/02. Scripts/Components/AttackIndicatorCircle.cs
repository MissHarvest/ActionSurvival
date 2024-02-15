using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[ExecuteAlways]
public class AttackIndicatorCircle : MonoBehaviour
{
    private IObjectPool<AttackIndicatorCircle> _managedPool;
    public Transform growEffect;
    private bool bStart = false;
    private float _growSpeed;

    private void Update()
    {
        if (bStart == false) return;
        growEffect.localScale += Vector3.one * Time.deltaTime * _growSpeed;
        
        if(growEffect.localScale.x >= 1.0f)
        {
            Destroy();
        }
    }

    public void SetManagedPool(IObjectPool<AttackIndicatorCircle> managedPool)
    {
        _managedPool = managedPool;
    }

    public void Activate(Vector3 position, float targetSpeed, float remainDist, SphereCollider collider)
    {
        transform.position = position + Vector3.up * 0.1f;
        transform.rotation = Quaternion.identity;
        transform.localScale = collider.radius * Vector3.one * 5.0f;
        growEffect.transform.localScale = Vector3.zero;
        var t = remainDist / targetSpeed;
        _growSpeed = 1.0f / t;
        gameObject.SetActive(true);
        bStart = true;
    }

    private void Destroy()
    {
        bStart = false;

        if (_managedPool == null)
        {
            Destroy(gameObject);
        }
        else
        {
            _managedPool.Release(this);
        }
    }
}
