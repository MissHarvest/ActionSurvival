using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
//using static UnityEditor.FilePathAttribute; 24.01.29 lgs

// 2024. 01. 24 Byun Jeongmin
public class BuildingSystem : MonoBehaviour
{
    private GameObject _rayPointer;

    [SerializeField] private LayerMask _buildableLayer;

    private float _gridSize = 1.0f;
    private int _rotationAngle = 45;

    [SerializeField] private float _maxBuildDistanceFront = 4.0f; // 플레이어 앞쪽 최대 건축 가능 거리
    [SerializeField] private float _maxBuildDistanceSideways = 5.0f;

    private int _inventoryIndex;
    private BuildableObject _buildableObject;

    public event Action<int> OnBuildRequested;
    public event Action<int> OnBuildCompleted;

    public Player Owner { get; private set; }


    private void Awake()
    {
        Owner = GetComponentInParent<Player>();
        CreateRayPointer();
    }

    public void CreateRayPointer()
    {
        var pos = transform.position + Vector3.up * 2;
        _rayPointer = Instantiate(Managers.Resource.GetCache<GameObject>("RayPointer.prefab"), pos, Quaternion.identity);
        _rayPointer.name = "RayPointer";
        _rayPointer.SetActive(false);
    }

    public void CreateArchitecture()
    {
        if (_buildableObject != null) return;

        var indexInUse = Managers.Game.Player.QuickSlot.IndexInUse;
        var index = _inventoryIndex = Managers.Game.Player.QuickSlot.slots[indexInUse].targetIndex;

        CreateArchitectureByIndex(index);
    }

    public void CreateArchitectureByIndex(int index)
    {
        _rayPointer.transform.position = transform.position + Vector3.up * 2;
        var handItemData = Owner.Inventory.Get(index).itemData as ToolItemData;
        if (!handItemData.isArchitecture) return;

        if (Physics.Raycast(_rayPointer.transform.position, Vector3.down, out RaycastHit hit, 100, _buildableLayer))
        {            
            _inventoryIndex = index;

            string itemNameWithoutItemData = handItemData.name.Replace("ItemData", "");

            string prefabName = "Architecture_" + itemNameWithoutItemData + ".prefab";
            var prefab = Managers.Resource.GetCache<GameObject>(prefabName);
            _buildableObject = Instantiate(prefab, hit.point, Quaternion.identity).GetComponent<BuildableObject>();
            _buildableObject.Create(_buildableLayer);
            OnBuildRequested?.Invoke(index);
        }
    }

    public bool BuildArchitecture()
    {
        if (_buildableObject.CanBuild() == false) return false;

        _buildableObject.Build();
        OnBuildCompleted?.Invoke(_inventoryIndex);
        _rayPointer.SetActive(false);
        _buildableObject = null;
        
        // 인벤토리에서 제거
        // Managers.Game.Player.Inventory.UseArchitectureItem(_inventoryIndex);
        return true;
    }

    public void CancelBuilding()
    {
        if (_buildableObject != null) _buildableObject.DestroyObject();
        _rayPointer.SetActive(false);
    }

    public void SetObjPosition()
    {
        Vector3 _location = new Vector3(
            Mathf.Round(_rayPointer.transform.position.x / _gridSize) * _gridSize,
            _buildableObject.gameObject.transform.position.y,
            Mathf.Round(_rayPointer.transform.position.z / _gridSize) * _gridSize
            );

        _buildableObject.gameObject.transform.position = _location;
    }

    public void MoveRayPointer(Vector2 joystickInput)
    {
        if (_buildableObject == null) return;
        float movementSpeed = 2.0f;

        // 현재 위치 저장
        Vector3 currentPosition = _rayPointer.transform.position;
        var nextPosition = currentPosition + new Vector3(joystickInput.x * movementSpeed * Time.deltaTime, 0, joystickInput.y * movementSpeed * Time.deltaTime);

        if(IsWithinBuildZone(nextPosition))
        {
            _rayPointer.transform.position = nextPosition;
        }

        SetObjPosition();
    }

    //청사진 위치를 기준으로 직사각형 영역 내에 있는지 확인
    private bool IsWithinBuildZone(Vector3 position)
    {
        float xDistance = Mathf.Abs(position.x - transform.position.x);
        float zDistance = Mathf.Abs(position.z - transform.position.z);

        
        return xDistance < _maxBuildDistanceSideways &&
               zDistance < _maxBuildDistanceFront;
    }


    #region Player Input Action Handle

    public void HandleRotateArchitectureLeft()
    {
        if (_buildableObject == null) return;
        _buildableObject.gameObject.transform.Rotate(Vector3.up, -_rotationAngle);

    }

    public void HandleRotateArchitectureRight()
    {
        if (_buildableObject == null) return;
        _buildableObject.gameObject.transform.Rotate(Vector3.up, _rotationAngle);
    }
    #endregion


    private void OnDrawGizmos()
    {
        if (_buildableObject == null)
            return;
        Collider buildableObjectCollider = _buildableObject.GetComponent<Collider>();

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_rayPointer.transform.position, buildableObjectCollider.bounds.size);

        //DrawRaycastGizmo(buildableObjectCollider.bounds.min, ref _leftEdgeHitY);
        //DrawRaycastGizmo(buildableObjectCollider.bounds.max, ref _rightEdgeHitY);
    }

    private void DrawRaycastGizmo(Vector3 origin, ref float hitY)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity, _buildableLayer))
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(hit.point, 0.1f);
            Gizmos.DrawLine(origin, hit.point);

            hitY = hit.point.y;  // 충돌 지점의 y값을 기억
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(origin, Vector3.down * 10f);
        }
    }
}
