using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class RectAttackIndicator : MonoBehaviour
{
    public Transform growEffect;
    private float _width;
    private float _speed;
    private IObjectPool<RectAttackIndicator> _managedPool;

    private void Awake()
    {
        var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _width = spriteRenderer.sprite.bounds.size.x;
    }

    public void SetManagedPool(IObjectPool<RectAttackIndicator> managedPool)
    {
        _managedPool = managedPool;
    }

    public void Activate(Vector3 position, Vector3 direction, float length, float chargeTime)
    {
        transform.position = position;
        growEffect.localScale = new Vector3(growEffect.localScale.x, 0.0f, growEffect.localScale.z);
        
        // 방향,
        transform.rotation = Quaternion.LookRotation(direction);
        gameObject.SetActive(true);

        _speed = 1.0f / chargeTime;

        // 사거리
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, length / _width);

        StartCoroutine(Grow());
    }

    IEnumerator Grow()
    {
        while(true)
        {
            yield return null;
            growEffect.localScale += Vector3.up * _speed * Time.deltaTime;

            if (growEffect.localScale.y >= 1.0f)
            {
                yield return new WaitForSeconds(1.0f);
                Destory();
                yield break;
            }
        }
    }


    private void Destory()
    {
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
