using System.Collections;
using UnityEngine;

// 2024. 02. 27 Byun Jeongmin
public class CopyRotation : MonoBehaviour
{
    [SerializeField] private bool _isPlayerTarget;
    [SerializeField] private bool _rotationX, _rotationY, _rotationZ;
    [SerializeField] private Transform _target;

    private void Start()
    {
        if (_isPlayerTarget)
        {
            StartCoroutine(WaitForPlayer());
        }
        else
        {
            StartCoroutine(WaitForMinimapCamera());
        }
    }

    private IEnumerator WaitForPlayer()
    {
        while (GameManager.Instance.Player == null)
        {
            yield return null;
        }
        _target = GameManager.Instance.Player.transform;
    }

    private IEnumerator WaitForMinimapCamera()
    {
        var minimapCamGO = GameObject.FindObjectOfType<MapIlluminator>();
        while (minimapCamGO == null)
        {
            yield return null;
        }
        _target = minimapCamGO.transform;
    }

    private void Update()
    {
        if (_target != null)
        {
            transform.rotation = Quaternion.Euler(
                (_rotationX ? _target.rotation.eulerAngles.x : transform.rotation.eulerAngles.x),
                (_rotationY ? _target.rotation.eulerAngles.y : transform.rotation.eulerAngles.y),
                (_rotationZ ? _target.rotation.eulerAngles.z : transform.rotation.eulerAngles.z));
        }
    }
}
