using System;
using UnityEngine;

// 2024-02-28 WJY
public class CustomOutlineDrawer : MonoBehaviour
{
    private GameObject _outlineObject;

    private void Awake() => Initialize();

    private void OnEnable() => Enable();

    private void OnDisable() => Disable();

    public void OnDestroy() => Destroy();

    private void Initialize()
    {
        _outlineObject = new($"[Outline] {gameObject.name}");
        _outlineObject.transform.SetParent(transform);
        _outlineObject.transform.localPosition = Vector3.zero;
        _outlineObject.transform.localRotation = Quaternion.identity;
        _outlineObject.transform.localScale = Vector3.one;

        var outlineMesh = Instantiate(GetComponent<MeshFilter>().sharedMesh);
        outlineMesh.normals = outlineMesh.CalcaultateSmoothNormal();
        _outlineObject.AddComponent<MeshFilter>().sharedMesh = outlineMesh;

        var outlineRenderer = _outlineObject.AddComponent<MeshRenderer>();
        outlineRenderer.material = Managers.Resource.GetCache<Material>("OutlineMaterial.mat");
    }

    public void Enable()
    {
        if (_outlineObject != null)
            _outlineObject.SetActive(true);
    }

    public void Disable()
    {
        if (_outlineObject != null)
            _outlineObject.SetActive(false);
    }

    public void Destroy()
    {
        if (this != null)
            Destroy(this);

        Destroy(_outlineObject);
    }
}