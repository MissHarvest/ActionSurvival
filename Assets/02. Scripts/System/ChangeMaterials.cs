using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
// 24.01.30 Lee gyuseong
public class ChangeMaterials : MonoBehaviour
{
    public Material[] fadeMaterials = new Material[1];
    public Material[] originMaterials = new Material[1];
    private Material[] _materials;
    public GameObject target; // Raycast로 쏴서 맞춘 오브젝트면 되지 않을까나?
    [SerializeField]
    private RaycastToChangeMaterials raycastToChangeMaterials;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
        raycastToChangeMaterials = _camera.GetComponent<RaycastToChangeMaterials>();
        raycastToChangeMaterials.OnRaycastHit += ChangeFadeMaterials;

    }

    public void ChangeFadeMaterials()
    {
        _materials = target.GetComponentInChildren<MeshRenderer>().materials;
        _materials[0] = fadeMaterials[0];
        _materials[1] = fadeMaterials[1];
        target.GetComponentInChildren<MeshRenderer>().materials = _materials;
    }

    public void ReturnMaterials()
    {
        _materials = target.GetComponentInChildren<MeshRenderer>().materials;
        _materials[0] = originMaterials[0];
        _materials[1] = originMaterials[1];
        target.GetComponentInChildren<MeshRenderer>().materials = _materials;
    }
}
