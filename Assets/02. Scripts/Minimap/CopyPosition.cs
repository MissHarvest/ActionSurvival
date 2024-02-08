using System.Collections;
using UnityEngine;

// 2024. 02. 07 Byun Jeongmin
public class CopyPosition : MonoBehaviour
{
    [SerializeField] private bool _positionX, _positionY, _positionZ;
    [SerializeField] private Transform _target;

    private void Start()
    {
        StartCoroutine(WaitForPlayer());
    }

    private IEnumerator WaitForPlayer()
    {
        while (Managers.Game.Player == null)
        {
            yield return null;
        }
        _target = Managers.Game.Player.transform;
        StartCoroutine(UpdatePosition());
    }

    private IEnumerator UpdatePosition()
    {
        while (true)
        {
            if (_target != null)
            {
                transform.position = new Vector3(
                    (_positionX ? _target.position.x : transform.position.x),
                    (_positionY ? _target.position.y : transform.position.y),
                    (_positionZ ? _target.position.z : transform.position.z));
            }
            yield return null;
        }
    }
}
