using System.Collections;
using UnityEngine;

public class KnockbackSystem : MonoBehaviour
{
    //lgs 24.01.26
    [SerializeField] private float _knockbackRatio = 1f;
    private Monster _monster;
    private Coroutine _coroutine;
    private WaitForFixedUpdate _wait;

    private void Awake()
    {
        _monster = GetComponent<Monster>();
        _monster.OnHit += OnHit;
        _wait = new WaitForFixedUpdate();
    }

    private void OnHit(IAttack attacker)
    {
        var other = attacker as MonoBehaviour;
        var dir = transform.position - other.transform.position;
        dir.y = 0;
        dir.Normalize();
        Knockback(dir);
        Debug.Log("충돌");
    }

    public void Knockback(Vector3 force)
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(CoKnockback(force));
    }

    private IEnumerator CoKnockback(Vector3 force)
    {
        for (float t = 0; t < 0.5f; t += Time.fixedDeltaTime)
        {
            Vector3 velocity = Vector3.Lerp(force, Vector3.zero, t / 0.5f);
            transform.position += _knockbackRatio * Time.fixedDeltaTime * velocity;
            yield return _wait;
        }
    }
}