using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RectAttackIndicator : MonoBehaviour
{
    public Transform growEffect;
    private float _width;
    private float _speed;

    private void Awake()
    {
        var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _width = spriteRenderer.sprite.bounds.size.x;
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
        //Graphics.DrawMesh()
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
                gameObject.SetActive(false);
                yield break;
            }
        }
    }

}
