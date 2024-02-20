using System;
using System.Collections.Generic;
using UnityEngine;

// 2024. 02. 20 Byun Jeongmin
[Serializable]
public struct WholeVerticesArray
{
    public Vector3[] Verticearray;
}

[Serializable]
public struct WholeColorsArray
{
    [SerializeField]
    public Color[] Colorarray;
}

public class WholeMapIlluminator : MonoBehaviour
{
    private GameObject _wholeShadowPlanePrefab;
    private Transform _player;
    private Mesh _wholeMesh;
    private LayerMask _shadowLayer;
    private WholeVerticesArray _verticesArray;
    [SerializeField] private WholeColorsArray _colorsArray;

    private Vector3 _hitPoint;

    private float _shadowRadius = 350f;

    private float _radiusCircle { get { return _shadowRadius; } }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _wholeShadowPlanePrefab = Managers.Resource.GetCache<GameObject>("WholeShadowPlane.prefab");
        GameObject wholeShadowPlane = Instantiate(_wholeShadowPlanePrefab, new Vector3(0, 40, 0), Quaternion.Euler(-90, 0, 0));
        _wholeMesh = wholeShadowPlane.GetComponent<MeshFilter>().mesh;
        _shadowLayer = LayerMask.GetMask("ShadowLayer");

        _verticesArray = new WholeVerticesArray { Verticearray = _wholeMesh.vertices };
        _colorsArray = new WholeColorsArray { Colorarray = new Color[_verticesArray.Verticearray.Length] };

        for (int k = 0; k < _colorsArray.Colorarray.Length; k++)
        {
            _colorsArray.Colorarray[k] = Color.black;
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
            _hitPoint = hit.point;
            Debug.Log(hit.point);
            UpdateAlpha(hit.point);
        }
    }

    private void UpdateAlpha(Vector3 hitPoint)
    {
        // 알파값 업데이트하는 기존 코드
        for (int j = 0; j < _verticesArray.Verticearray.Length; j++)
        {
            Vector3 vector3 = transform.TransformPoint(_verticesArray.Verticearray[j]);
            var temp = vector3 - hitPoint;
            temp.y = 0;
            float distance = Vector3.SqrMagnitude(temp);

            if (distance < _radiusCircle * _radiusCircle)
            {
                float alpha = Mathf.Min(_colorsArray.Colorarray[j].a, distance / _radiusCircle);
                _colorsArray.Colorarray[j].a = alpha;
            }
        }

        UpdateColors();
    }

    private void UpdateColors()
    {
        Color32[] colors32 = new Color32[_colorsArray.Colorarray.Length];
        for (int i = 0; i < colors32.Length; i++)
        {
            colors32[i] = _colorsArray.Colorarray[i];
        }
        _wholeMesh.colors32 = colors32;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(_hitPoint, 0.2f);
    }
}