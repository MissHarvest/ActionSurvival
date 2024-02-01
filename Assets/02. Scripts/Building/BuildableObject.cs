using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

// 2024. 01. 24 Byun Jeongmin
public class BuildableObject : MonoBehaviour, IHit
{
    [SerializeField] private Renderer _renderer;
    private NavMeshObstacle _navMeshObstacle;
    private Collider _collider;
    private Rigidbody _rigidbody;
    private Material _originMat;
    public Material redMat;
    public Material blueMat;

    public bool canBuild { get; set; } = true;
    public bool isOverlap { get; private set; } = false;

    public event Action OnRenamed;
    public event Action<float> OnHit;

    [field:SerializeField] public Condition HP { get; private set; }

    private void Awake()
    {
        _renderer= GetComponentInChildren<MeshRenderer>();
        _originMat = new Material(_renderer.material);
        _collider = GetComponent<Collider>();
        _navMeshObstacle = GetComponent<NavMeshObstacle>();
        _rigidbody = GetComponent<Rigidbody>();

        var architectureName = name.Replace("Architecture_", "");
        architectureName = architectureName.Replace("(Clone)", "");

        var data = Managers.Resource.GetCache<ItemData>($"{architectureName}ItemData.data");
        if (data is ToolItemData tool)
        {
            HP = new Condition(tool.maxDurability);
        }

        HP.OnBelowedToZero += DestroyObject;
    }

    public void Create()
    {
        SetMaterial(blueMat);
        _rigidbody.useGravity = false;
        _collider.isTrigger = true;
        StartCoroutine(StartBuild());
    }

    IEnumerator StartBuild()
    {
        while(true)
        {
            yield return null;
            var mat = canBuild ? blueMat : redMat;
            if(_collider.isTrigger) SetMaterial(mat);
        }
    }

    public void Build()
    {
        _collider.isTrigger = false;
        _rigidbody.useGravity = true;
        StopCoroutine(StartBuild());
        SetMaterial(_originMat);
        _navMeshObstacle.enabled = true;
    }

    private void SetMaterial(Material material)
    {
        _renderer.material = material;
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

    public void Hit(IAttack attacker, float damage)
    {
        HP.Subtract(damage);
        OnHit?.Invoke(HP.GetPercentage());
    }
}
