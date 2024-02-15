using System;
using System.Collections;
using UnityEngine;


// 2024. 02. 07 Byun Jeongmin
[Serializable]
public struct VerticesArray
{
    [SerializeField]
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
    [SerializeField] private LayerMask _shadowLayer;

    private VerticesArray[] _verticesArray;
    [SerializeField] private ColorsArray[] _colorsArray;

    private float _shadowRadius = 250f;

    // 맵 크기에 맞는 가로, 세로의 ShadowPlane프리팹 개수
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
        _verticesArray = new VerticesArray[_shadowPlanes.Length];
        _colorsArray = new ColorsArray[_shadowPlanes.Length];

        GameObject shadowPlanesParent = new GameObject("ShadowPlanes");

        // 프리팹을 이용하여 ShadowPlane 생성
        for (int i = 0; i < _numPlanesX; i++)
        {
            for (int j = 0; j < _numPlanesZ; j++)
            {
                Vector3 spawnPosition = new Vector3(i * 100 - 500, 40, j * -100 + 100);

                _shadowPlanes[i * _numPlanesZ + j] = Instantiate(_shadowPlanePrefab, spawnPosition, Quaternion.Euler(90, 0, 0));
                _shadowPlanes[i * _numPlanesZ + j].transform.parent = shadowPlanesParent.transform;

                _meshes[i * _numPlanesZ + j] = _shadowPlanes[i * _numPlanesZ + j].GetComponent<MeshFilter>().mesh;
                _verticesArray[i * _numPlanesZ + j] = new VerticesArray { Verticearray = _meshes[i * _numPlanesZ + j].vertices };
                _colorsArray[i * _numPlanesZ + j] = new ColorsArray { Colorarray = new Color[_verticesArray[i * _numPlanesZ + j].Verticearray.Length] };

                for (int k = 0; k < _colorsArray[i * _numPlanesZ + j].Colorarray.Length; k++)
                {
                    _colorsArray[i * _numPlanesZ + j].Colorarray[k] = Color.black;
                }
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
        // 각 ShadowPlane에 레이캐스트
        for (int i = 0; i < _shadowPlanes.Length; i++)
        {
            Ray ray = new Ray(_player.position, Vector3.up);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000, _shadowLayer, QueryTriggerInteraction.Collide))
            {
                for (int j = 0; j < _verticesArray[i].Verticearray.Length; j++)
                {
                    Vector3 vector3 = _shadowPlanes[i].transform.TransformPoint(_verticesArray[i].Verticearray[j]);
                    var temp = vector3 - hit.point;
                    temp.y = 0;
                    float distance = Vector3.SqrMagnitude(temp);

                    if (distance < _radiusCircle * _radiusCircle)
                    {
                        float alpha = Mathf.Min(_colorsArray[i].Colorarray[j].a, distance / _radiusCircle);
                        _colorsArray[i].Colorarray[j].a = alpha;
                    }
                }

                UpdateColors(i);
            }
        }
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
        SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Runtime, "Minimap");
        UpdateColors();
    }

    protected virtual void Save()
    {
        var json = JsonUtility.ToJson(this);
        SaveGame.CreateJsonFile("Minimap", json, SaveGame.SaveType.Runtime);
    }
}