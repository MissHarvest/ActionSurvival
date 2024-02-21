using UnityEngine;

public class InstanceBlock : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private Transform _model;

    public Material SideMaterial { get => GetMaterials()[0]; set { GetMaterials()[0] = value; } }
    public Material FrontMaterial { get => GetMaterials()[1]; set { GetMaterials()[1] = value; } }

    public Vector3 Forward { get => GetModel().forward; set => GetModel().forward = value; }
    public Mesh Mesh => GetMesh();
    public Matrix4x4 TransformMatrix => GetModel().GetChild(0).transform.localToWorldMatrix;
    public MeshRenderer MeshRenderer => GetMeshRenderer();

    private Material[] GetMaterials()
    {
        return GetMeshRenderer().sharedMaterials;
    }

    private Transform GetModel()
    {
        if (_model == null)
            _model = transform.GetChild(0);
        return _model;
    }

    private Mesh GetMesh()
    {
        if (_meshFilter == null)
            _meshFilter = GetModel().GetChild(0).GetComponent<MeshFilter>();
        return _meshFilter.sharedMesh;
    }

    private MeshRenderer GetMeshRenderer()
    {
        if (_meshRenderer == null)
            _meshRenderer = GetModel().GetChild(0).GetComponent<MeshRenderer>();
        return _meshRenderer;
    }
}