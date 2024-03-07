using System;
using UnityEngine;

// 2024-02-28 WJY
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class OutlineDrawer : MonoBehaviour
{
    [HideInInspector] [SerializeField] private Material _outlineMaterial;
    [SerializeField] private Mesh _originMesh;
    [SerializeField] private UnityEngine.Object _managedKeyObject;
    private MeshRenderer _outlineRenderer;
    private MeshFilter _meshFilter;
    private OutlineManager _manager;

#if UNITY_EDITOR
    private void Reset()
    {
        GetComponent<MeshRenderer>().sharedMaterial = _outlineMaterial;
        GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        GetComponent<MeshRenderer>().receiveShadows = false;
        GetComponent<MeshFilter>().sharedMesh = null;
    }
#endif

    private void Awake() => Initialize();
    private void OnDestroy() => Dispose();
    private void Initialize()
    {
        _outlineRenderer = GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();
        _manager = GameManager.OutlineManager;

        if (!Managers.Data.smoothNormalMeshDictionary.TryGetValue(_originMesh, out var outlineMesh))
        {
            outlineMesh = Instantiate(_originMesh);
            outlineMesh.normals = outlineMesh.CalcaultateSmoothNormal();
            Managers.Data.smoothNormalMeshDictionary.Add(_originMesh, outlineMesh);
        }
        _meshFilter.mesh = outlineMesh;
        _outlineRenderer.sharedMaterial = _outlineMaterial;

        _manager.RegistOutlineDrawer(_managedKeyObject, this);
        gameObject.SetActive(false);
    }

    public void Dispose()
    {
        _manager?.ReleaseOutlineDrawer(_managedKeyObject);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}