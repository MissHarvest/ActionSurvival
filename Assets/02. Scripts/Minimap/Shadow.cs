using System.Collections;
using UnityEngine;

// 2024. 02. 07 Byun Jeongmin
public class Shadow : MonoBehaviour
{
    [SerializeField] private GameObject _shadowPlane;
    [SerializeField] private Transform _player;
    [SerializeField] private LayerMask _shadowLayer;
    [SerializeField] private float _shadowRadius = 20f;

    private float _radiusCircle { get { return _shadowRadius; } }

    private Mesh _mesh;
    private Vector3[] _verctices;
    private Color[] _colors;

    private void Initialize()
    {
        _mesh = _shadowPlane.GetComponent<MeshFilter>().mesh;
        _verctices = _mesh.vertices;
        _colors = new Color[_verctices.Length];
        for (int i = 0; i < _colors.Length; i++)
        {
            _colors[i] = Color.black;
        }
        UpdateColors();
    }

    private void Start()
    {
        StartCoroutine(WaitForPlayer());
        Initialize();
    }

    private IEnumerator WaitForPlayer()
    {
        while (Managers.Game.Player == null)
        {
            yield return null;
        }
        _player = Managers.Game.Player.transform;
        StartCoroutine(UpdateColor());
    }

    private IEnumerator UpdateColor()
    {
        while (true)
        {
            Ray ray = new Ray(_player.position, Vector3.up); //플레이어 방향 위로 쏘는 게 맞을듯
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000, _shadowLayer, QueryTriggerInteraction.Collide))
            {
                for (int i = 0; i < _verctices.Length; i++) //for문안쓰고?
                {
                    Vector3 vector3 = _shadowPlane.transform.TransformPoint(_verctices[i]);
                    //Debug.Log("벡터값: "+vector3);
                    var temp = vector3 - hit.point;
                    temp.y = 0;
                    float distance = Vector3.SqrMagnitude(temp);
                    if (distance < _radiusCircle * _radiusCircle)
                    {
                        float alpha = Mathf.Min(_colors[i].a, distance / _radiusCircle);
                        _colors[i].a = alpha;
                    }
                }

                UpdateColors();
            }
            yield return null;
        }
    }

    private void UpdateColors()
    {
        _mesh.colors = _colors;
    }
}
