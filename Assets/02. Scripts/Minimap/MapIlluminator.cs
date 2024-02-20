using System;
using System.Collections.Generic;
using UnityEngine;

// 2024. 02. 07 Byun Jeongmin
[Serializable]
public struct VerticesArray
{
    public Vector3[] Verticearray;
}

[Serializable]
public struct ColorsArray
{
    [SerializeField]
    public Color[] Colorarray;
}

public class MapIlluminator : MonoBehaviour
{
    private GameObject _shadowPlanePrefab;
    private GameObject[] _shadowPlanes;
    private Transform _player;
    private Mesh[] _meshes;
    private LayerMask _shadowLayer;
    private VerticesArray[] _verticesArray;
    [SerializeField] private ColorsArray[] _colorsArray;

    private float _shadowRadius = 250f;
    private int _numPlanesX = 11;
    private int _numPlanesZ = 3;
    private float _radiusCircle { get { return _shadowRadius; } }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _shadowPlanePrefab = Managers.Resource.GetCache<GameObject>("ShadowPlane.prefab");
        _shadowPlanes = new GameObject[_numPlanesX * _numPlanesZ];
        _meshes = new Mesh[_shadowPlanes.Length];
        _shadowLayer = LayerMask.GetMask("ShadowLayer");
        _verticesArray = new VerticesArray[_shadowPlanes.Length];
        _colorsArray = new ColorsArray[_shadowPlanes.Length];

        GameObject shadowPlanesParent = new GameObject("ShadowPlanes");

        for (int i = 0; i < _shadowPlanes.Length; i++)
        {
            int row = i / _numPlanesZ;
            int col = i % _numPlanesZ;

            Vector3 spawnPosition = new Vector3(row * 100 - 500, 40, col * -100 + 100);

            _shadowPlanes[i] = Instantiate(_shadowPlanePrefab, spawnPosition, Quaternion.Euler(90, 0, 0));
            _shadowPlanes[i].transform.parent = shadowPlanesParent.transform;

            _meshes[i] = _shadowPlanes[i].GetComponent<MeshFilter>().mesh;
            _verticesArray[i] = new VerticesArray { Verticearray = _meshes[i].vertices };
            _colorsArray[i] = new ColorsArray { Colorarray = new Color[_verticesArray[i].Verticearray.Length] };

            for (int k = 0; k < _colorsArray[i].Colorarray.Length; k++)
            {
                _colorsArray[i].Colorarray[k] = Color.black;
            }
        }
    }

    private void Start()
    {
        _player = Managers.Game.Player.transform;
        Load();
        Managers.Game.OnSaveCallback += Save;
    }

    private void Update()
    {
        Ray ray = new Ray(_player.position, Vector3.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, _shadowLayer, QueryTriggerInteraction.Collide))
        {
            int planeIndex = FindPlaneIndex(hit.collider.gameObject);

            if (planeIndex != -1)
            {
                ProcessNeighborPlanes(hit.point, planeIndex);
            }
        }
    }

    private int FindPlaneIndex(GameObject plane)
    {
        for (int i = 0; i < _shadowPlanes.Length; i++)
        {
            if (_shadowPlanes[i] == plane)
            {
                return i;
            }
        }
        return -1; // 해당하는 shadowplane을 못찾았을 때
    }

    private void ProcessNeighborPlanes(Vector3 start, int planeIndex)
    {
        int[] neighbors = GetNeighbors(planeIndex);

        foreach (int neighbor in neighbors)
        {
            ProcessPlane(neighbor, start);
        }
    }

    // 알파값 업데이트하는 기존 코드
    private void ProcessPlane(int planeIndex, Vector3 hitPoint)
    {
        for (int j = 0; j < _verticesArray[planeIndex].Verticearray.Length; j++)
        {
            Vector3 vector3 = _shadowPlanes[planeIndex].transform.TransformPoint(_verticesArray[planeIndex].Verticearray[j]);
            var temp = vector3 - hitPoint;
            temp.y = 0;
            float distance = Vector3.SqrMagnitude(temp);

            if (distance < _radiusCircle * _radiusCircle)
            {
                float alpha = Mathf.Min(_colorsArray[planeIndex].Colorarray[j].a, distance / _radiusCircle);
                _colorsArray[planeIndex].Colorarray[j].a = alpha;
            }
        }

        UpdateColors(planeIndex);
    }

    // 상하좌우, 대각선으로 인접한 플레인 찾기
    private int[] GetNeighbors(int planeIndex)
    {
        List<int> neighbors = new List<int>();

        int row = planeIndex / _numPlanesZ;
        int col = planeIndex % _numPlanesZ;
        //Debug.Log($"row: {row}, col: {col}");

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int neighborRow = row + i;
                int neighborCol = col + j;

                if (neighborRow >= 0 && neighborRow < _numPlanesX && neighborCol >= 0 && neighborCol < _numPlanesZ)
                {
                    int neighborIndex = neighborRow * _numPlanesZ + neighborCol;
                    MeshRenderer neighborRenderer = _shadowPlanes[neighborIndex].GetComponent<MeshRenderer>();

                    // 중심점과 플레이어의 현재 위치 사이의 거리
                    Vector3 neighborCenter = _shadowPlanes[neighborIndex].transform.position;
                    float distance = Vector3.Distance(neighborCenter, _player.position);

                    float planeWidth = neighborRenderer.bounds.size.x;

                    if (distance < (_radiusCircle / 5) + (planeWidth / 2))
                    {
                        neighbors.Add(neighborIndex);
                    }
                }
            }
        }
        //Debug.Log("인접 플레인 개수 : "+neighbors.ToArray().Length);
        return neighbors.ToArray();
    }

    private void UpdateColors(int planeIndex)
    {
        Color32[] colors32 = new Color32[_colorsArray[planeIndex].Colorarray.Length];
        for (int i = 0; i < colors32.Length; i++)
        {
            colors32[i] = _colorsArray[planeIndex].Colorarray[i];
        }
        _meshes[planeIndex].colors32 = colors32;
    }

    private void UpdateColors()
    {
        for (int i = 0; i < _shadowPlanes.Length; i++)
        {
            UpdateColors(i);
        }
    }

    public virtual void Load()
    {
        if (SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Runtime, "Minimap"))
        {
            UpdateColors();
        }
    }

    protected virtual void Save()
    {
        var json = JsonUtility.ToJson(this);
        SaveGame.CreateJsonFile("Minimap", json, SaveGame.SaveType.Runtime);
    }

    //private void OnDrawGizmos()
    //{
    //    if (_shadowPlanes != null && _shadowPlanes.Length > 0)
    //    {
    //        foreach (var plane in _shadowPlanes)
    //        {
    //            Gizmos.color = Color.blue;
    //            Gizmos.DrawSphere(plane.transform.position, 0.1f);
    //        }

    //        if (_player != null)
    //        {
    //            Gizmos.color = Color.red;
    //            Gizmos.DrawSphere(_player.position, 0.1f);

    //            foreach (var plane in _shadowPlanes)
    //            {
    //                Gizmos.color = Color.magenta;
    //                Gizmos.DrawLine(plane.transform.position, _player.position);
    //            }

    //            Gizmos.color = Color.red;
    //            Gizmos.DrawWireSphere(_player.position, _radiusCircle / 5);
    //        }
    //    }
    //}
}
