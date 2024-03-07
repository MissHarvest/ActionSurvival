using System;
using UnityEngine;

// 2024-02-28 WJY
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class OutlineDrawer : MonoBehaviour
{
    [HideInInspector] [SerializeField] private Material _outlineMaterial;
    [SerializeField] private Mesh _originMesh;
    private MeshRenderer _outlineRenderer;
    private MeshFilter _meshFilter;

#if UNITY_EDITOR
    private void Reset()
    {
        GetComponent<MeshRenderer>().sharedMaterial = _outlineMaterial;
        GetComponent<MeshFilter>().sharedMesh = null;
    }
#endif

    private void Awake() => Initialize();

    private void Initialize()
    {
        _outlineRenderer = GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();

        if (!Managers.Data.smoothNormalMeshDictionary.TryGetValue(_originMesh, out var outlineMesh))
        {
            outlineMesh = Instantiate(_originMesh);
            outlineMesh.normals = outlineMesh.CalcaultateSmoothNormal();
            Managers.Data.smoothNormalMeshDictionary.Add(_originMesh, outlineMesh);
        }
        _meshFilter.mesh = outlineMesh;
        _outlineRenderer.sharedMaterial = _outlineMaterial;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}