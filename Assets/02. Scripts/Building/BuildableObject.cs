using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// 2024. 01. 24 Byun Jeongmin
public class BuildableObject : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    private NavMeshObstacle _navMeshObstacle;
    private Collider _collider;
    private Material _originMat;
    public Material redMat;
    public Material blueMat;

    private int buildingLayer = 11; // Architecture 레이어 번호
    public bool canBuild { get; set; } = true;
    public bool isOverlap { get; private set; } = false;
    public event Action OnRenamed;

    private void Awake()
    {
        _renderer= GetComponentInChildren<Renderer>();
        _originMat = new Material(_renderer.material);
        _renderer.material = _originMat;
        _collider = GetComponent<Collider>();
        _navMeshObstacle = GetComponent<NavMeshObstacle>();
    }

    public void Create()
    {
        _renderer.material = blueMat;
        _collider.isTrigger = true;
        StartCoroutine(StartBuild());
    }

    IEnumerator StartBuild()
    {
        while(true)
        {
            yield return null;
            _renderer.material = canBuild ? blueMat : redMat;
        }
    }

    public void Build()
    {
        _renderer.material = _originMat;
        _collider.isTrigger = false;
        _navMeshObstacle.enabled = true;
        StopCoroutine(StartBuild());
    }

    private void Start()
    {
        if (gameObject.name.Contains("(Clone)"))
        {
            Managers.Game.Architecture.Add(this);
        }
        OnRenamed?.Invoke();
    }

    public void DestroyObject()
    {
        Managers.Game.Architecture.Remove(this);
        StopCoroutine(StartBuild());
        Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        isOverlap = false;
    }

    private void OnTriggerExit(Collider other)
    {
        isOverlap = true;
    }
}
