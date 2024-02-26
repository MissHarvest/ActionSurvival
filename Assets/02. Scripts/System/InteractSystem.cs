using System;
using System.Linq;
using UnityEngine;

// 2024-02-23 WJY
public class InteractSystem
{
    private ToolSystem _toolSystem;
    private Transform _transform;
    private ToolItemData _emptyHand;
    private LayerMask _architectureLayerMask;
    private LayerMask _monsterLayerMask;
    private LayerMask _resourcesLayerMask;
    private LayerMask _interactableAllLayerMask;
    private Collider[] targets;

    public event Action<Vector3> OnWeaponInteract;
    public event Action<IInteractable, string, Vector3> OnToolInteract;
    public event Action<IDestructible, string, Vector3> OnToolDestruct;
    public event Action<IInteractable, string, Vector3> OnArchitectureInteract;

    public InteractSystem()
    {
        _toolSystem = GameManager.Instance.Player.ToolSystem;
        _transform = _toolSystem.transform;
        _emptyHand = Managers.Resource.GetCache<ToolItemData>("EmptyHandItemData.data");
        _architectureLayerMask = LayerMask.GetMask("Architecture");
        _monsterLayerMask = LayerMask.GetMask("Monster");
        _resourcesLayerMask = LayerMask.GetMask("Resources");
        _interactableAllLayerMask = _architectureLayerMask | _monsterLayerMask | _resourcesLayerMask;
    }

    public void TryInteractSequence()
    {
        SearchAllObject(_transform.position);

        var tool = GetCurrentTool();
        var isWeapon = tool is WeaponItemData;

        // # 1. 타게팅 공격
        if (isWeapon)
        {
            if (TryWeaponInteract(tool))
                return;
        }

        // # 2. 도구 상호작용
        if (tool != _emptyHand)
        {
            // # 2-1. 파괴
            if (TryToolDestruct(tool))
                return;

            // # 2-2. 벌목, 채광
            if (TryToolInteract(tool))
                return;
        }

        // # 3. 구조물 상호작용
        if (TryArchitectureInteract(_emptyHand))
            return;

        // # 4. 채집 상호작용
        if (TryToolInteract(_emptyHand))
            return;

        // # 5. 허공에 공격
        if (isWeapon)
        {
            if (TryWeaponInteract(tool, true))
                return;
        }
    }

    public bool TryWeaponInteract(ToolItemData tool, bool force = false)
    {
        if (force)
        {
            Debug.Log("?");
            OnWeaponInteract?.Invoke(Vector3.zero);
            return true;
        }

        foreach (var target in targets)
        {
            if (!target.CompareTag("Monster") || (1 << target.gameObject.layer & tool.targetLayers) == 0)
                continue;

            if (Vector3.SqrMagnitude(target.transform.position - _transform.position) > tool.range * tool.range)
                break;

            if (target.TryGetComponent<IHit>(out var hit))
            {
                Debug.Log("!");
                OnWeaponInteract?.Invoke(target.transform.position);
                return true;
            }
        }
        return false;
    }

    public bool TryToolInteract(ToolItemData tool)
    {
        foreach (var target in targets)
        {
            if (!target.CompareTag(tool.targetTagName) || (1 << target.gameObject.layer & tool.targetLayers) == 0)
                continue;

            if (Vector3.SqrMagnitude(target.transform.position - _transform.position) > tool.range * tool.range)
                break;

            if (target.TryGetComponent<IInteractable>(out var interactable))
            {
                OnToolInteract?.Invoke(interactable, target.tag, target.transform.position);
                return true;
            }
        }
        return false;
    }

    public bool TryToolDestruct(ToolItemData tool)
    {
        foreach (var target in targets)
        {
            if ((1 << target.gameObject.layer & tool.targetLayers) == 0)
                continue;

            if (Vector3.SqrMagnitude(target.transform.position - _transform.position) > tool.range * tool.range)
                break;

            if (target.TryGetComponent<IDestructible>(out var IDestructible))
            {
                OnToolDestruct?.Invoke(IDestructible, tool.targetTagName, target.transform.position);
                return true;
            }
        }
        return false;
    }

    public bool TryArchitectureInteract(ToolItemData tool)
    {
        foreach (var target in targets)
        {
            if ((1 << target.gameObject.layer & _architectureLayerMask) == 0)
                continue;

            if (Vector3.SqrMagnitude(target.transform.position - _transform.position) > tool.range * tool.range)
                break;

            if (target.TryGetComponent<IInteractable>(out var interactable))
            {
                OnArchitectureInteract?.Invoke(interactable, "Make", target.transform.position);
                return true;
            }
        }
        return false;
    }

    // 한 번에 모든 오브젝트를 서칭하고, 거리 순으로 정렬
    public void SearchAllObject(Vector3 position) => SearchObject(position, GetMaxRange(), _interactableAllLayerMask);

    public void SearchObject(Vector3 position, float maxRange, LayerMask targetLayers)
    {
        targets = Physics.OverlapSphere(position, maxRange, targetLayers, QueryTriggerInteraction.Collide);
        targets = targets.OrderBy(x => Vector3.SqrMagnitude(position - x.transform.position)).ToArray();
    }

    private float GetMaxRange()
    {
        return Mathf.Max(GetCurrentTool().range, _emptyHand.range);
    }

    private ToolItemData GetCurrentTool()
    {
        return _toolSystem.EquippedTool.itemSlot.itemData as ToolItemData;
    }
}