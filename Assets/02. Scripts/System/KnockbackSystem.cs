using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KnockbackSystem : MonoBehaviour
{
    //lgs 24.01.26
    private Rigidbody _rigidbody;
    private float _knockbackTime = 0f;
    private Monster _monster;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _monster = GetComponent<Monster>();
        _monster.OnHit += OnHit;
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

    private void OnHit(IAttack attacker)
    {
        _rigidbody.isKinematic = false;
        var other = attacker as MonoBehaviour;
        var dir = (transform.position - other.transform.position);
        dir.y = 0;
        dir.Normalize();
        _rigidbody.AddForce(dir * 2f, ForceMode.Impulse);
        _knockbackTime = 0.5f;
        Debug.Log("충돌");
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other == null)
    //    {
    //        return;
    //    }

    //    if (other.gameObject.layer == 8 && _rigidbody.isKinematic == true)
    //    {
    //        _rigidbody.isKinematic = false;
    //        var dir = (transform.position - other.transform.position);
    //        dir.y = 0;
    //        dir.Normalize();
    //        _rigidbody.AddForce(dir * 2f, ForceMode.Impulse);
    //        _knockbackTime = 0.5f;
    //        Debug.Log("충돌");
    //    }
    //}
}
