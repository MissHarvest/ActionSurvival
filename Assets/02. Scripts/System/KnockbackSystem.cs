using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KnockbackSystem : MonoBehaviour
{
    //lgs 24.01.26
    private Rigidbody _rigidbody;
    private float _knockbackTime = 0f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        _rigidbody.isKinematic = true;
    }
    private void FixedUpdate()
    {
        _knockbackTime -= Time.deltaTime;
        if (_knockbackTime <= 0f)
        {
            _rigidbody.isKinematic = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null)
        {
            return;
        }

        if (other.gameObject.layer == 8 && _rigidbody.isKinematic == true)
        {
            _rigidbody.isKinematic = false;
            _rigidbody.AddForce((transform.position - other.transform.position).normalized, ForceMode.Impulse);
            _knockbackTime = 0.5f;
            Debug.Log("충돌");
        }
    }
}
