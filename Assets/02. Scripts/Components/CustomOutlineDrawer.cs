using System;
using UnityEngine;

// 2024-02-28 WJY
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CustomOutlineDrawer : MonoBehaviour
{
    [SerializeField] private Material _outlineMaterial;
    [SerializeField] private Mesh _originMesh;
    private MeshRenderer _outlineRenderer;
    private MeshFilter _meshFilter;


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
        _outlineRenderer.material = _outlineMaterial;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}