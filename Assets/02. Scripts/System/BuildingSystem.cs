using System;
using UnityEditor;
using UnityEngine;
//using static UnityEditor.FilePathAttribute; 24.01.29 lgs

// 2024. 01. 24 Byun Jeongmin
public class BuildingSystem : MonoBehaviour
{
    private GameObject _rayPointer;

    [SerializeField] private LayerMask _buildableLayer;

    private float _raycastRange = 20.0f;
    private float gridSize = 1.0f;
    private int _rotationAngle = 45;
    private int _inventoryIndex;

    private BuildableObject _buildableObject;

    public event Action<int> OnBuildRequested;

    public Player Owner { get; private set; }

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
        _buildableObject.canBuild = 
            ( _buildableLayer == (_buildableLayer | 1 << RaycastHit().collider.gameObject.layer) )
            && _buildableObject.isOverlap;
        return _buildableObject.canBuild;
    }

    public void CancelBuilding()
    {
        if (_buildableObject != null) _buildableObject.DestroyObject();
        _rayPointer.SetActive(false);
    }

    private RaycastHit RaycastHit() // 매개변수를 transform.positon으로 받아서 플레이어/울타리 위치 나누기
    {
        RaycastHit hit;

        Vector3 playerPosition = _rayPointer.transform.position;

        Ray ray = new Ray(playerPosition, Vector3.down);

        Physics.BoxCast(_rayPointer.transform.position, Vector3.one * 0.5f, Vector3.down, out hit, Quaternion.identity, _raycastRange);

        return hit;
    }

    public void SetObjPosition()
    {
        Vector3 _location = new Vector3(
            Mathf.Floor(_rayPointer.transform.position.x / gridSize) * gridSize,
            _buildableObject.gameObject.transform.position.y,
            Mathf.Floor(_rayPointer.transform.position.z / gridSize) * gridSize
            );
        CanBuild();
        _buildableObject.gameObject.transform.position = _location;
    }

    public void SetObjPositionWithJoystick(Vector2 joystickInput) //조이스틱인풋을 raycast할 지점 바꾸기
    {
        if (_buildableObject == null) return;
        float movementSpeed = 2.0f;
        
        // 울타리 이동 및 위치 조정
        _rayPointer.transform.position += new Vector3(joystickInput.x * movementSpeed * Time.deltaTime, 0, joystickInput.y * movementSpeed * Time.deltaTime);

        SetObjPosition();
    }

    #region Player Input Action Handle

    public void HandleRotateArchitectureLeft()
    {
        if(_buildableObject == null) return;
        _buildableObject.gameObject.transform.Rotate(Vector3.up, -_rotationAngle);

    }

    public void HandleRotateArchitectureRight()
    {
        if (_buildableObject == null) return;
        _buildableObject.gameObject.transform.Rotate(Vector3.up, _rotationAngle);
    }
    #endregion
}
