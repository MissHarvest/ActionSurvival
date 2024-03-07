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
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void ChangeFadeMaterials()
    {
        _meshRenderer.sharedMaterials = fadeMaterials;
    }

    public void ReturnMaterials()
    {
        _meshRenderer.sharedMaterials = originMaterials;
    }
}
