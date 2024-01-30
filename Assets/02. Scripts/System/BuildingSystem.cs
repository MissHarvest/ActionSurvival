using System;
using UnityEngine;
//using static UnityEditor.FilePathAttribute; 24.01.29 lgs

// 2024. 01. 24 Byun Jeongmin
public class BuildingSystem : MonoBehaviour
{
    private Camera _cam;
    private VirtualJoystick _virtualJoystick;

    [SerializeField] private GameObject _tempPrefab;
    [SerializeField] private Material _previewMat;
    [SerializeField] private Material _nonBuildableMat;
    private Material _origionmat;

    [SerializeField] private LayerMask _buildableLayer; // creatingLayer
    [SerializeField] private LayerMask _destroyLayer;
    private LayerMask _currentLayer;

    [SerializeField] private int _raycastRange = 20;
    [SerializeField] private float _yGridSize = 0.1f;
    [SerializeField] private int _rotationAngle = 45;

    [SerializeField] private GameObject _obj;
    private BuildableObject _buildableObject;

    private GameObject _bird;

    private bool _isHold = false;
    private bool _canCreateObject = true;
    private bool validHIt = false;

    public Player Owner { get; private set; }
    public bool IsHold
    {
        get { return _isHold; }
        private set { _isHold = value; }
    }

    public bool CanCreateObject
    {
        get { return _canCreateObject; }
        private set { _canCreateObject = value; }
    }

    private void Awake()
    {
        _cam = Camera.main;

        Owner = Managers.Game.Player;
        var input = Owner.Input;
    }

    private void Start()
    {
        _virtualJoystick = FindObjectOfType<VirtualJoystick>();
    }

    private void Update()
    {
        if (_isHold)
        {
            if (_virtualJoystick != null)
            {
                //Vector2 joystickInput = _virtualJoystick.Handle.anchoredPosition;
                //Debug.Log("조이스틱 x값 :" + joystickInput.x + ", y값 : " + joystickInput.y);
                //SetObjPositionWithJoystick(joystickInput);
            }
        }
    }

    #region

    public void SetRayBird()
    {
        Vector3 birdPosition = transform.position + Vector3.up * 1f;
        //_bird = Instantiate(Managers.Resource.GetCache<GameObject>("RaycastBird.prefab"), birdPosition, Quaternion.identity, _tempPrefab.transform);
        _bird = Instantiate(Managers.Resource.GetCache<GameObject>("RaycastBird.prefab"), birdPosition, Quaternion.identity);
    }

    private RaycastHit RaycastHit()
    {
        RaycastHit hit;
        Vector3 raycastStartPoint = _bird.transform.position;
        Ray ray = new Ray(raycastStartPoint, Vector3.down);
        //validHIt = Physics.Raycast(ray, out hit, _raycastRange, _currentLayer);
        Physics.Raycast(ray, out hit, _raycastRange, Physics.DefaultRaycastLayers);

        if (hit.collider != null && IsInLayerMask(hit.collider.gameObject.layer, _currentLayer))
        {
            _canCreateObject = true;
            //Debug.Log("건축 가능한 레이어입니다. 레이어: " + hit.collider.gameObject.layer);
            return hit;
        }
        else
        {
            _canCreateObject = false;
            //Debug.Log("건축 불가능한 레이어입니다. 레이어: " + hit.collider.gameObject.layer);
            return hit;
        }
    }

    private RaycastHit PlayerRaycastHit()
    {
        RaycastHit hit;

        Vector3 playerPosition = transform.position;
        Vector3 raycastStartPoint = playerPosition;

        Ray ray = new Ray(raycastStartPoint, Vector3.down);
        validHIt = Physics.Raycast(ray, out hit, _raycastRange, _currentLayer);
        //Debug.Log("현재 레이어 : " + hit.collider.gameObject.layer);
        return hit;
    }

    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }

    //오브젝트의 가로 세로 크기만큼 움직임 -> 1칸씩 움직이게 수정
    public void SetObjPosition(Vector3 hitPos)
    {
        Vector3 location = hitPos;
        location.Set(Mathf.Round(location.x), Mathf.Round(location.y / _yGridSize) * _yGridSize, Mathf.Round(location.z));
        location.y = _obj.transform.position.y;
        _obj.transform.position = location;
    }

    public void SetObjPositionWithJoystick(Vector2 joystickInput)
    {
        // 이동 속도
        float movementSpeed = 4.0f;

        // 울타리 이동 및 위치 조정
        _bird.transform.position += new Vector3(joystickInput.x * movementSpeed * Time.deltaTime, 0, joystickInput.y * movementSpeed * Time.deltaTime);
        //Debug.Log("x값: " + _obj.transform.position.x + " y값: " + _obj.transform.position.y + " z값: " + _obj.transform.position.z);

        SetObjPosition(RaycastHit().point);
    }

    private void CreateBluePrintObject(Vector3 pos)
    {
        SetRayBird();
        Vector3 position = _bird.transform.position - Vector3.up * 1f;
        //Debug.Log("이건 임시 프리팹 이름이에요 : " + _tempPrefab.name);
        _obj = Instantiate(_tempPrefab);

        //SetObjPosition();
        _obj.transform.position = position;
        //_obj.transform.position = pos;
        _buildableObject = _obj.GetComponent<BuildableObject>();
        _buildableObject.SetMaterial(_previewMat);

        BuildableObjectColliderManager buildableObject = _obj.GetComponentInChildren<BuildableObjectColliderManager>();
        buildableObject.OnRedMatAction += HandleBuildableObjectTriggerEnter;
        buildableObject.OnBluePrintMatAction += HandleBuildableObjectTriggerExit;
    }

    #endregion

    #region Player Input Action Handle

    private void HandleCreateBluePrint()
    {
        if (!_isHold)
        {
            _isHold = true;
            _currentLayer = _buildableLayer;
            Vector3 newPosition = PlayerRaycastHit().point;
            CreateBluePrintObject(newPosition);
        }
    }

    public void HandleRotateArchitectureLeft()
    {
        if (_isHold)
            _obj.transform.Rotate(Vector3.up, -_rotationAngle);
    }

    public void HandleRotateArchitectureRight()
    {
        if (_isHold)
            _obj.transform.Rotate(Vector3.up, _rotationAngle);
    }

    public void HandleCancelBuildMode()
    {
        if (_isHold)
        {
            Destroy(_obj);
            Destroy(_bird);
            _isHold = false;
        }
    }

    // 건축물 설치
    private void HandleInstallArchitecture()
    {
        if (_isHold && _canCreateObject)
        {
            _isHold = false;
            _obj.GetComponentInChildren<BoxCollider>().isTrigger = false;

            _buildableObject.SetInitialObject();
            _buildableObject.DestroyColliderManager();
            Destroy(_bird);

            // 건축물 장착 해제
            Managers.Game.Player.Inventory.FindItem(Managers.Game.Player.ToolSystem.ItemInUse.itemData, out int index);
            var item = new QuickSlot();
            item.Set(index, Managers.Game.Player.Inventory.slots[index]);
            Managers.Game.Player.QuickSlot.UnRegist(item);

            // 인벤토리에서 제거
            Managers.Game.Player.Inventory.DestroyItemByIndex(item);
        }
        else if (_isHold && !_canCreateObject)
        {
            HandleCancelBuildMode();
        }
    }

    #endregion

    #region Architecture Collider Action Handle

    private void HandleBuildableObjectTriggerEnter()
    {
        _canCreateObject = false;
    }

    private void HandleBuildableObjectTriggerExit()
    {
        _canCreateObject = true;
    }

    #endregion

    public void CreateAndSetArchitecture()
    {
        if (_isHold)
        {
            // 청사진 위치에 건축물 생성
            HandleInstallArchitecture();
        }
        else
        {
            // 청사진 생성
            if (Managers.Game.Player.QuickSlot.IndexInUse > -1)
            {
                var handItemData = Managers.Game.Player.QuickSlot.slots[Managers.Game.Player.QuickSlot.IndexInUse].itemSlot.itemData as ToolItemData;

                if (handItemData.isArchitecture)
                {
                    // "아이템명"+ItemData에서 "ItemData" 부분 제거
                    string itemNameWithoutItemData = handItemData.name.Replace("ItemData", "");

                    string prefabName = "Architecture_" + itemNameWithoutItemData + ".prefab";
                    _tempPrefab = Managers.Resource.GetCache<GameObject>(prefabName);

                    HandleCreateBluePrint();
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_bird == null) return;

        RaycastHit hit = RaycastHit();

        //Debug.DrawLine(_bird.transform.position, hit.point, Color.red);

        Gizmos.color = Color.red;
        Gizmos.DrawCube(hit.point, new Vector3(0.1f, 0.1f, 0.1f));
    }
}
