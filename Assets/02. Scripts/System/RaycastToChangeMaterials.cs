using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastToChangeMaterials : MonoBehaviour
{
    [SerializeField]
    private LayerMask _layerMask;
    [SerializeField]
    private Camera _camera;

    private Transform _player;

    private RaycastHit[] _hits = new RaycastHit[10];

    private IEnumerable Start() // 코루틴, 디렉션은 캐릭터 뷰포인트에
    {

        if (_player != null)
        {
            _player = Managers.Game.Player.GetComponent<Transform>();
        }
        yield return null;
    }


    private void FixedUpdate()
    {
        CheckForObjects();
    }

    private void CheckForObjects()
    {
        int hits = Physics.RaycastNonAlloc(_camera.transform.position, (_player.transform.position - _camera.transform.position).normalized, _hits,
            Vector3.Distance(_camera.transform.position, _player.transform.position), _layerMask);
        if (hits > 0)
        {
            for (int i = 0; i < hits; i++)
            {
                Debug.Log(_hits[i].collider.gameObject.name);
                //Debug.Log(hits);
            }
        }
    } 
}
