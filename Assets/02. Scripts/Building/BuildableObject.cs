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
    private LayerMask _buildableLayer;
    public Collider otherCollider;

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
        if (data is ArchitectureItemData tool)
        {
            HP = new Condition(tool.MaxDurability);
        }

        HP.OnBelowedToZero += DestroyObject;
    }

    public void Create(LayerMask layermask)
    {
        _buildableLayer = layermask;
        SetMaterial(blueMat);
        _rigidbody.useGravity = false;
        _navMeshObstacle.enabled = false;
        _collider.isTrigger = true;
        StartCoroutine(StartBuild());
    }

    IEnumerator StartBuild()
    {
        while(true)
        {
            yield return null;
            var mat = CanBuild() ? blueMat : redMat;
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
        otherCollider = other;
        isOverlap = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isOverlap = false;
        otherCollider = null;
    }

    public bool CanBuild()
    {
        if (isOverlap) return false;

        bool isLeftEdgeOnGround = Physics.Raycast(_collider.bounds.min, Vector3.down, out RaycastHit leftHit, Mathf.Infinity, _buildableLayer);
        bool isCenterOnGround = Physics.Raycast(_collider.bounds.center, Vector3.down, out RaycastHit centerHit, Mathf.Infinity, _buildableLayer);
        bool isRightEdgeOnGround = Physics.Raycast(_collider.bounds.max, Vector3.down, out RaycastHit rightHit, Mathf.Infinity, _buildableLayer);
        
        // 한 부분이라도 충돌이 안되면,,
        if (!isLeftEdgeOnGround || !isCenterOnGround || !isRightEdgeOnGround) return false;

        // 바닥과의 거리가 멀 때 ,,
        if (Mathf.Abs(_collider.bounds.center.y - transform.position.y - centerHit.distance) > 0.01f) return false;
        
        // 좌우 양 끝에서의 충돌 지점의 y 값이 서로 다르면 건축 불가능
        // >> 완전히 같냐고 물어도 되나?
        return (leftHit.point.y == centerHit.point.y) && (centerHit.point.y == rightHit.point.y);
    }

    public void Hit(IAttack attacker, float damage)
    {
        HP.Subtract(damage);
        OnHit?.Invoke(HP.GetPercentage());
    }
}
