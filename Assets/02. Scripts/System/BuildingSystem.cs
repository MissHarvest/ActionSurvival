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

    private float _raycastRange = 20.0f;
    private float _gridSize = 1.0f;
    private int _rotationAngle = 45;

    [SerializeField] private float _maxBuildDistanceFront = 4.0f; // 플레이어 앞쪽 최대 건축 가능 거리
    //[SerializeField] private float _maxBuildDistanceBack = 3.0f;
    [SerializeField] private float _maxBuildDistanceSideways = 5.0f;

    private float _leftEdgeHitY;  // 좌측 끝에서의 레이캐스트 충돌 지점 y값
    private float _rightEdgeHitY;  // 우측 끝에서의 레이캐스트 충돌 지점 y값

    private int _inventoryIndex;

    private Vector3 _lastValidHitPoint; // 마지막으로 유효한 충돌 지점을 저장할 변수
    private Vector3 _lastValidPosition;

    private BuildableObject _buildableObject;

    public event Action<int> OnBuildRequested;

    public Player Owner { get; private set; }

    public float LeftEdgeHitY
    {
        get { return _leftEdgeHitY; }
        private set { _leftEdgeHitY = value; }
    }

    public float RightEdgeHitY
    {
        get { return _rightEdgeHitY; }
        private set { _rightEdgeHitY = value; }
    }

    private void Awake()
    {
        Owner = Managers.Game.Player;
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
        if (_buildableObject != null)
            return;

        var indexInUse = Managers.Game.Player.QuickSlot.IndexInUse;
        var handItemData = Managers.Game.Player.QuickSlot.slots[indexInUse].itemSlot.itemData as ToolItemData;
        _inventoryIndex = Managers.Game.Player.QuickSlot.slots[indexInUse].targetIndex;

        if (handItemData.isArchitecture)
        {
            _rayPointer.transform.position = transform.position + Vector3.up * 2;
            // "아이템명"+ItemData에서 "ItemData" 부분 제거
            string itemNameWithoutItemData = handItemData.name.Replace("ItemData", "");

            string prefabName = "Architecture_" + itemNameWithoutItemData + ".prefab";
            var prefab = Managers.Resource.GetCache<GameObject>(prefabName);
            _buildableObject = Instantiate(prefab).GetComponent<BuildableObject>();
            _buildableObject.Create();
        }
    }

    public void CreateArchitectureByIndex(int index)
    {
        var handItemData = Managers.Game.Player.Inventory.slots[index].itemData as ToolItemData;
        _inventoryIndex = index;

        _rayPointer.transform.position = transform.position + Vector3.up * 2;
        // "아이템명"+ItemData에서 "ItemData" 부분 제거
        string itemNameWithoutItemData = handItemData.name.Replace("ItemData", "");

        string prefabName = "Architecture_" + itemNameWithoutItemData + ".prefab";
        var prefab = Managers.Resource.GetCache<GameObject>(prefabName);
        _buildableObject = Instantiate(prefab).GetComponent<BuildableObject>();
        _buildableObject.Create();
        OnBuildRequested?.Invoke(index);
    }

    public bool BuildArchitecture()
    {
        if (_buildableObject.canBuild == false) return false;
        if (CanBuild() == false) return false;

        _buildableObject.Build();
        _rayPointer.SetActive(false);
        _buildableObject = null;
        // 인벤토리에서 제거      
        Managers.Game.Player.Inventory.UseArchitectureItem(_inventoryIndex);
        return true;
    }


    private bool CanBuild()
    {
        RaycastHit hit = RaycastHit();
        if (hit.collider == null)
            return false;

        bool isOnBuildableLayer = (hit.collider != null) && (_buildableLayer == (_buildableLayer | 1 << hit.collider.gameObject.layer));
        bool isCloseToGround = Mathf.Abs(hit.point.y - transform.position.y) < 0.1f;

        _buildableObject.canBuild = _buildableObject.CanBuild(_buildableLayer) && isOnBuildableLayer && _buildableObject.isOverlap && isCloseToGround && IsWithinBuildZone(hit.point);

        return _buildableObject.canBuild;
    }

    public void CancelBuilding()
    {
        if (_buildableObject != null) _buildableObject.DestroyObject();
        _rayPointer.SetActive(false);
    }

    private RaycastHit RaycastHit()
    {
        RaycastHit hit;

        Vector3 playerPosition = _rayPointer.transform.position;

        // _buildableObject의 박스 콜라이더 크기만큼 BoxCast 크기 설정
        Collider buildableObjectCollider = _buildableObject.GetComponent<Collider>();

        if (Physics.BoxCast(_rayPointer.transform.position, buildableObjectCollider.bounds.size / 2.0f, Vector3.down, out hit, Quaternion.identity, _raycastRange))
        {
            _lastValidHitPoint = hit.point; // 유효한 충돌이 있을 때만 기억
        }

        return hit;
    }

    public void SetObjPosition()
    {
        Vector3 _location = new Vector3(
            Mathf.Floor(_rayPointer.transform.position.x / _gridSize) * _gridSize,
            transform.position.y,
            Mathf.Floor(_rayPointer.transform.position.z / _gridSize) * _gridSize
            );
        CanBuild();
        _buildableObject.gameObject.transform.position = _location;
    }

    public void SetObjPositionWithJoystick(Vector2 joystickInput)
    {
        if (_buildableObject == null) return;
        float movementSpeed = 2.0f;

        // 현재 위치 저장
        Vector3 currentPosition = _rayPointer.transform.position;

        _rayPointer.transform.position += new Vector3(joystickInput.x * movementSpeed * Time.deltaTime, 0, joystickInput.y * movementSpeed * Time.deltaTime);

        SetObjPosition();

        // 이동 후 위치
        Vector3 newPosition = _rayPointer.transform.position;

        // 건축 가능 직사각형 영역을 넘어가면 마지막으로 영역 안에 있던 위치로 도르마무
        if (!IsWithinBuildZone(newPosition))
        {
            _rayPointer.transform.position = _lastValidPosition;
            SetObjPosition();
        }
        else
        {
            // 유효한 위치 갱신
            _lastValidPosition = newPosition;
        }
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

        DrawRaycastGizmo(buildableObjectCollider.bounds.min, ref _leftEdgeHitY);
        DrawRaycastGizmo(buildableObjectCollider.bounds.max, ref _rightEdgeHitY);
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

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;

    //    Vector3 center = transform.position;
    //    float halfSideways = _maxBuildDistanceSideways / 2.0f;
    //    float halfFront = _maxBuildDistanceFront / 2.0f;

    //    Vector3 frontLeft = center + new Vector3(-halfSideways, 0, halfFront);
    //    Vector3 frontRight = center + new Vector3(halfSideways, 0, halfFront);
    //    Vector3 backLeft = center + new Vector3(-halfSideways, 0, -halfFront);
    //    Vector3 backRight = center + new Vector3(halfSideways, 0, -halfFront);

    //    Gizmos.DrawLine(frontLeft, frontRight);
    //    Gizmos.DrawLine(frontRight, backRight);
    //    Gizmos.DrawLine(backRight, backLeft);
    //    Gizmos.DrawLine(backLeft, frontLeft);
    //}
}
