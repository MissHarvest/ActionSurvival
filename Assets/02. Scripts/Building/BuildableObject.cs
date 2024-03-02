using System;
using System.Collections;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

// 2024. 01. 24 Byun Jeongmin
public class BuildableObject : MonoBehaviour, IHit, IDestructible
{
    [Header("Settings")]
    [SerializeField] private float _destructTime = 3f;
    [Header("")]
    [SerializeField] private Renderer _renderer;
    private NavMeshObstacle _navMeshObstacle;
    private Collider _collider;
    private Rigidbody _rigidbody;
    private Material _originMat;
    [HideInInspector] public Material redMat;
    [HideInInspector] public Material blueMat;
    [HideInInspector] public Collider otherCollider;
    private LayerMask _buildableLayer;

    public bool canBuild { get; set; } = true;
    public bool isOverlap { get; private set; } = false;

    public event Action OnRenamed;
    public event Action<float> OnHit;
    public event Action OnDestroyed;

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
        GameManager.Architecture.Add(this);
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
        //if (gameObject.name.Contains("(Clone)"))
        //{
        //    GameManager.Architecture.Add(this);
        //}
        OnRenamed?.Invoke();
    }

    public void DestroyObject()
    {
        GameManager.Architecture.Remove(this);
        StopCoroutine(StartBuild());
        OnDestroyed?.Invoke();
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
        return Mathf.Approximately(leftHit.point.y, centerHit.point.y) && Mathf.Approximately(centerHit.point.y, rightHit.point.y);
    }

    public void Hit(IAttack attacker, float damage)
    {
        if (_collider.isTrigger) return;
        HP.Subtract(damage);
        OnHit?.Invoke(HP.GetPercentage());
    }

    public float GetDestructTime()
    {
        return _destructTime;
    }

    public void Destruct(Player player)
    {
        ObtainRandomIngredients(player);
        DestroyObject();
    }

    private string ExtractItemName(string fullName)
    {
        string itemNameWithNumber = fullName.Split('_')[1];
        string[] itemNameParts = itemNameWithNumber.Split('-');
        Debug.Log(itemNameParts[0]);
        return itemNameParts[0];
    }

    private void ObtainRandomIngredients(Player player)
    {
        // buildableObject를 만드는데 드는 총 재료 개수
        var recipeData = Managers.Data.GetRecipeDataByItemName(ExtractItemName(name));
        int totalIngredients = recipeData.requiredItems.Sum(ingredient => ingredient.quantity);

        //총 재료 개수 / 4
        int maxIngredientsToObtain = Mathf.CeilToInt(totalIngredients / 4f);

        //재료 목록에서 중복을 허용하여 랜덤하게 획득
        var selectedIngredients = recipeData.requiredItems
            .OrderBy(ingredient => UnityEngine.Random.value)
            .ToList();

        // 1부터 (총 재료 개수 / 4)개까지 재료 획득 가능 
        int totalQuantityToObtain = UnityEngine.Random.Range(1, maxIngredientsToObtain + 1);

        for (int i = 0; i < selectedIngredients.Count; i++)
        {
            // 재료의 양을 고려해서 획득할 수 있는 최대 개수 설정
            int maxQuantityToObtain = Mathf.CeilToInt(selectedIngredients[i].quantity / 4f);

            //totalQuantityToObtain보다 maxQuantityToObtain가 큰 경우 방지
            int quantityToObtain = Mathf.Min(totalQuantityToObtain, maxQuantityToObtain);

            player.Inventory.TryAddItem(selectedIngredients[i].item, quantityToObtain);
            Debug.Log($"재료 아이템명: {selectedIngredients[i].item.displayName}, 개수: {quantityToObtain}");

            totalQuantityToObtain -= quantityToObtain;
            if (totalQuantityToObtain <= 0)
                break;
        }
    }
}
