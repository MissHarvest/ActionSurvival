using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
// 24.01.30 Lee gyuseong
public class ChangeMaterials : MonoBehaviour
{
    public Material[] fadeMaterials = new Material[2];
    public Material[] originMaterials = new Material[2];
    private Material[] _materials;
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _materials = _meshRenderer.materials;
    }
    public void ChangeFadeMaterials()
    {
        for (int i = 0; i < _materials.Length; i++)
        {
            _materials[i] = fadeMaterials[i];
        }
        _meshRenderer.materials = _materials;
    }

    public void ReturnMaterials()
    {
        for (int i = 0; i < _materials.Length; i++)
        {
            _materials[i] = originMaterials[i];
        }
        _meshRenderer.materials = _materials;
    }
}
