using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 24.01.30 Lee gyuseong
public class RaycastToChangeMaterials : MonoBehaviour
{
    [SerializeField]
    private LayerMask _layerMask;
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private Transform _player;

    public Action OnRaycastHit;

    private RaycastHit[] _hits = new RaycastHit[10];

    private void Start()
    {
        StartCoroutine(CheckPlayer());
    }

    private IEnumerator CheckPlayer()
    {
        while (Managers.Game.Player == null)
        {
            yield return null;
        }
        _player = Managers.Game.Player.ViewPoint;
    }

    private void FixedUpdate()
    {
        if (_player != null)
        {
            CheckForObjects();
        }
    }

    private void CheckForObjects()
    {
        int hits = Physics.RaycastNonAlloc(_camera.transform.position, (_player.transform.position - _camera.transform.position).normalized, _hits,
            Vector3.Distance(_camera.transform.position, _player.transform.position), _layerMask);
        if (hits > 0)
        {
            for (int i = 0; i < hits; i++)
            {
                //_hits[i].transform.gameObject.GetComponent<ChangeMaterials>().ChangeFadeMaterials();
                OnRaycastHit();
            }
        }
    }
}
