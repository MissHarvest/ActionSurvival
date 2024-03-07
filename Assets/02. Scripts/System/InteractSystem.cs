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
    private InteractableTarget[] _targets;
    private Action _onInteract;
    private InteractableTarget? _currentHighlightTarget;

    public readonly struct InteractableTarget
    {
        public readonly Collider targetCollider;
        public readonly float distance;

        public readonly GameObject gameObject
        {
            get
            {
                if (targetCollider == null) return null;
                else return targetCollider.gameObject; 
            }
        }

        public readonly Transform transform
        {
            get
            {
                if (targetCollider == null) return null;
                else return targetCollider.transform;
            }
        }
        public readonly string tag
        {
            get
            {
                if (targetCollider == null) return null;
                else return targetCollider.tag;
            }
        }

        public InteractableTarget(Collider targetCollider, Vector3 searchOrigin)
        {
            this.targetCollider = targetCollider;
            distance = Vector3.SqrMagnitude(targetCollider.transform.position - targetCollider.ClosestPoint(searchOrigin));
        }

        public readonly bool CompareTag(string tag) => targetCollider.CompareTag(tag);
        public readonly bool TryGetComponent<T>(out T component) => targetCollider.TryGetComponent(out component);
    }

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

    public void TryInteract()
    {
        _onInteract?.Invoke();
    }

    public void SearchInteractableObjectsSequence()
    {
        var tool = GetCurrentTool();
        var isWeapon = tool is WeaponItemData;

        SearchAllObject(_transform.position);

        // # 1. 타게팅 공격
        if (isWeapon)
        {
            if (SearchWeaponInteract(tool))
                return;
        }

        // # 2. 도구 상호작용
        if (tool != _emptyHand)
        {
            // # 2-1. 파괴
            if (SearchToolDestruct(tool))
                return;

            // # 2-2. 벌목, 채광
            if (SearchToolInteract(tool))
                return;
        }

        // # 3. 구조물 상호작용
        if (SearchArchitectureInteract(_emptyHand))
            return;

        // # 4. 채집 상호작용
        if (SearchToolInteract(_emptyHand))
            return;

        // # 5. 허공에 공격
        if (isWeapon)
        {
            if (SearchWeaponInteract(tool, true))
                return;
        }

        _onInteract = null;
        HighlightTarget(null);
    }

    public void SearchWeaponInteract()
    {
        var tool = GetCurrentTool();
        var isWeapon = tool is WeaponItemData;

        SearchObject(_transform.position, tool.range, _monsterLayerMask);

        // # 1. 타게팅 공격
        if (isWeapon)
        {
            if (SearchWeaponInteract(tool))
                return;
        }

        // # 2. 허공에 공격
        if (isWeapon)
        {
            if (SearchWeaponInteract(tool, true))
                return;
        }
    }

    private bool SearchWeaponInteract(ToolItemData tool, bool force = false)
    {
        if (force)
        {
            _onInteract = () => OnWeaponInteract?.Invoke(Vector3.zero);
            //OnWeaponInteract?.Invoke(Vector3.zero);
            return true;
        }

        foreach (var target in _targets)
        {
            if (!target.CompareTag("Monster") || (1 << target.gameObject.layer & tool.targetLayers) == 0)
                continue;

            if (target.distance > tool.range * tool.range)
                break;

            if (target.TryGetComponent<IHit>(out var hit))
            {
                _onInteract = () => OnWeaponInteract?.Invoke(target.transform.position);
                HighlightTarget(target);
                //OnWeaponInteract?.Invoke(target.transform.position);
                return true;
            }
        }
        return false;
    }

    private bool SearchToolInteract(ToolItemData tool)
    {
        foreach (var target in _targets)
        {
            if (!target.CompareTag(tool.targetTagName) || (1 << target.gameObject.layer & tool.targetLayers) == 0)
                continue;

            if (target.distance > tool.range * tool.range)
                break;

            if (target.TryGetComponent<IInteractable>(out var interactable))
            {
                _onInteract = () => OnToolInteract?.Invoke(interactable, target.tag, target.transform.position);
                HighlightTarget(target);
                //OnToolInteract?.Invoke(interactable, target.tag, target.transform.position);
                return true;
            }
        }
        return false;
    }

    private bool SearchToolDestruct(ToolItemData tool)
    {
        foreach (var target in _targets)
        {
            if ((1 << target.gameObject.layer & tool.targetLayers) == 0)
                continue;

            if (target.distance > tool.range * tool.range)
                break;

            if (target.TryGetComponent<IDestructible>(out var IDestructible))
            {
                _onInteract = () => OnToolDestruct?.Invoke(IDestructible, tool.targetTagName, target.transform.position);
                HighlightTarget(target);
                //OnToolDestruct?.Invoke(IDestructible, tool.targetTagName, target.transform.position);
                return true;
            }
        }
        return false;
    }

    private bool SearchArchitectureInteract(ToolItemData tool)
    {
        foreach (var target in _targets)
        {
            if ((1 << target.gameObject.layer & _architectureLayerMask) == 0)
                continue;

            if (target.distance > tool.range * tool.range)
                break;

            if (target.TryGetComponent<IInteractable>(out var interactable))
            {
                _onInteract = () => OnArchitectureInteract?.Invoke(interactable, target.tag, target.transform.position);
                HighlightTarget(target);
                //OnArchitectureInteract?.Invoke(interactable, target.tag, target.transform.position);
                return true;
            }
        }
        return false;
    }

    // 한 번에 모든 오브젝트를 서칭하고, 거리 순으로 정렬
    public void SearchAllObject(Vector3 position) => SearchObject(position, GetMaxRange(), _interactableAllLayerMask);

    public void SearchObject(Vector3 position, float maxRange, LayerMask targetLayers)
    {
        var colliders = Physics.OverlapSphere(position, maxRange, targetLayers, QueryTriggerInteraction.Collide);
        _targets = colliders.Select(x => new InteractableTarget(x, position)).OrderBy(x => x.distance).ToArray();
    }

    private float GetMaxRange()
    {
        return Mathf.Max(GetCurrentTool().range, _emptyHand.range);
    }

    private ToolItemData GetCurrentTool()
    {
        return _toolSystem.EquippedTool.itemSlot.itemData as ToolItemData;
    }

    private void HighlightTarget(InteractableTarget? target)
    {
        if (_currentHighlightTarget?.targetCollider != null)
        {
            if (target?.targetCollider != _currentHighlightTarget?.targetCollider)
                _currentHighlightTarget?.targetCollider?.GetComponentInChildren<OutlineDrawer>(true)?.SetActive(false);
        }
        target?.targetCollider?.GetComponentInChildren<OutlineDrawer>(true)?.SetActive(true);
        _currentHighlightTarget = target;
    }
}