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

    private RaycastHit[] _hits = new RaycastHit[10];

    private HashSet<ChangeMaterials> _prevChangeMaterials = new HashSet<ChangeMaterials>();
    private HashSet<ChangeMaterials> _currentChangeMaterials = new HashSet<ChangeMaterials>();

    private void Start()
    {
        StartCoroutine(CheckPlayer());
    }

    private IEnumerator CheckPlayer()
    {
        while (GameManager.Instance.Player == null)
        {
            yield return null;
        }
        _player = GameManager.Instance.Player.ViewPoint;
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
        //이전 프레임에 담기는 리스트, 이번 프레임에 담기는 리스트의 차이가 있는 오브젝트에 ReturnMaterials 호출
        _prevChangeMaterials = _currentChangeMaterials;
        _currentChangeMaterials = new();
        
        // 플레이어에서 카메라 방향으로 ray 쏘게 수정(BJM)
        int hits = Physics.RaycastNonAlloc(_player.transform.position, (_camera.transform.position - _player.transform.position).normalized, _hits,
            Vector3.Distance(_camera.transform.position, _player.transform.position), _layerMask, QueryTriggerInteraction.Collide);

        if (hits >= 0)
        {
            // hashset
            for (int i = 0; i < hits; i++)
            {
                ChangeMaterials changeMaterials = _hits[i].transform.gameObject.GetComponentInParent<ChangeMaterials>();
                _currentChangeMaterials.Add(changeMaterials);
                
                changeMaterials.ChangeFadeMaterials();
            }

            _prevChangeMaterials.ExceptWith(_currentChangeMaterials);

            foreach (var hit in _prevChangeMaterials)
                hit.ReturnMaterials();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(_camera.transform.position, _camera.transform.forward * 50f);
    }
}
