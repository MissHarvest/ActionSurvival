using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackArea : MonoBehaviour
{
    public Vector3 Center => transform.position;
    public Vector3 BoxSize => transform.localScale * 0.5f;

    private void Awake()
    {
        gameObject.SetActive(false);
    }
}
