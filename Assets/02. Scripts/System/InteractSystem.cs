using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

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

    public event Action<IHit, Vector3> OnWeaponInteract;
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

        if (TryWeaponInteract())
        {
            return;
        }
        if (GetCurrentTool() != _emptyHand)
        {
            if (TryToolDestruct(GetCurrentTool()))
            {
                return;
            }
            if (TryToolInteract(GetCurrentTool()))
            {
                return;
            }
        }
        if (TryArchitectureInteract())
        {
            return;
        }
        if (TryToolInteract())
        {
            return;
        }
        if (TryWeaponInteract(true))
        {
            return;
        }
    }

    public bool TryWeaponInteract(bool force = false)
    {
        var hand = GetCurrentTool();
        if (hand is not WeaponItemData)
            return false;

        if (force)
        {
            OnWeaponInteract?.Invoke(null, _transform.forward);
            return true;
        }

        foreach (var target in targets)
        {
            if (!target.CompareTag("Monster") || (1 << target.gameObject.layer & hand.targetLayers) == 0)
                continue;

            if (Vector3.SqrMagnitude(target.transform.position - _transform.position) > hand.range * hand.range)
                break;

            if (target.TryGetComponent<IHit>(out var hit))
            {
                OnWeaponInteract?.Invoke(hit, target.transform.position);
                return true;
            }
        }
        return false;
    }

    public bool TryToolInteract() => TryToolInteract(_emptyHand);

    public bool TryToolInteract(ToolItemData tool)
    {
        foreach (var target in targets)
        {
            Debug.Log(1 << target.gameObject.layer);
            Debug.Log((int)tool.targetLayers);
            Debug.Log((1 << target.gameObject.layer & tool.targetLayers) == 0);
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

    public bool TryArchitectureInteract()
    {
        foreach (var target in targets)
        {
            Debug.Log(1 << target.gameObject.layer);
            Debug.Log((int)_architectureLayerMask);
            Debug.Log((1 << target.gameObject.layer & _architectureLayerMask) == 0);
            if ((1 << target.gameObject.layer & _architectureLayerMask) == 0)
                continue;

            if (Vector3.SqrMagnitude(target.transform.position - _transform.position) > _emptyHand.range * _emptyHand.range)
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