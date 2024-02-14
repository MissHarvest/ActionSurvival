using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private bool _isFired;
    private Vector3 _direction;
    private float _maxDistance;
    private float _moveDistance = 0.0f;
    private bool _destroy = false;

    [field: SerializeField] public float Speed { get; private set; } = 10.0f;

    private void Update()
    {
        if (_isFired == false) return;
        transform.Translate(Vector3.forward * Time.deltaTime * Speed);
        _moveDistance += Time.deltaTime * Speed;

        if (_maxDistance <= _moveDistance) DestroySelf();
    }

    public void Fire(Vector3 destination, float maxDistance, bool destroy = true)
    {
        //Debug.Log("Fire");
        gameObject.SetActive(true);
        _direction = destination - transform.position;
        transform.rotation = Quaternion.LookRotation(_direction);
        _direction.Normalize();
        _maxDistance = maxDistance;
        _destroy = destroy;
        _isFired = true;
    }

    public void DestroySelf()
    {
        if(_destroy)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
            _moveDistance = 0.0f;
        }
    }
}
