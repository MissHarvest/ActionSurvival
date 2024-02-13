using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    private Vector3 _destination = Vector3.zero;

    public void SetDestination(Vector3 destination)
    {
        _destination = destination;
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;

        var look = _destination - transform.position;
        look.y = 0;

        if(look.sqrMagnitude < 1)
        {
            gameObject.SetActive(false);
            return;
        }

        transform.rotation = Quaternion.LookRotation(look);
    }
}
