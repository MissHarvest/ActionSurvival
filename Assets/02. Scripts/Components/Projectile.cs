using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private bool _isFired;
    private Vector3 _direction;
    private float _moveDistance = 0.0f;
    public Action OnDestroy;
    public float MaxDistance { get; private set; }
    [field: SerializeField] public float Speed { get; private set; } = 10.0f;

    private void Update()
    {
        if (_isFired == false) return;
        transform.Translate(Vector3.forward * Time.deltaTime * Speed);
        _moveDistance += Time.deltaTime * Speed;

        if (MaxDistance <= _moveDistance) Destroy();
    }

    public void Fire(Vector3 direction, float speed, float maxDistance)
    {
        direction.Normalize();
        transform.rotation = Quaternion.LookRotation(direction);
        Speed = speed;
        MaxDistance = maxDistance;
        _isFired = true;
    }

    public void Fire(Vector3 start, Vector3 direction, float speed, float maxDistance)
    {
        gameObject.transform.position = start;
        Fire(direction, speed, maxDistance);
    }

    public void Destroy()
    {
        _isFired = false;
        _moveDistance = 0.0f;

        if (OnDestroy == null)
        {
            Destroy(gameObject);
        }
        else
        {
            OnDestroy.Invoke();
        }
    }
}
