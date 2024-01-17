using UnityEngine;

public class SlideBlock : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private Transform _model;

    public Material SideMaterial { get => GetMaterials()[0]; set { GetMaterials()[0] = value; } }
    public Material FrontMaterial { get => GetMaterials()[1]; set { GetMaterials()[1] = value; } }

    public Vector3 Forward { get => GetModel().forward; set => GetModel().forward = value; }

    private Material[] GetMaterials()
    {
        if (_meshRenderer == null)
            _meshRenderer = GetModel().GetChild(0).GetComponent<MeshRenderer>();
        return _meshRenderer.materials;
    }

    private Transform GetModel()
    {
        if (_model == null)
            _model = transform.GetChild(0);
        return _model;
    }
}