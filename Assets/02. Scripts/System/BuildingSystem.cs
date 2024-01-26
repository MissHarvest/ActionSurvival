using System;
using UnityEngine;

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

    private bool _isHold = false;
    private bool _canCreateObject = true;
    private bool validHIt = false;

    public Player Owner { get; private set; }
    public bool IsHold
    {
        get { return _isHold; }
        private set { _isHold = value; }
    }

    // 얘네 없애도 돌아가게
    public event Action OnCreateBluePrintAction;
    public event Action OnInstallArchitectureAction;
    public event Action OnRotateArchitectureLeftAction;
    public event Action OnRotateArchitectureRightAction;
    public event Action OnCancelBuildModeAction;
    public event Action OnBreakModeAction;
    public event Action OnBreakArchitectureAction;

    private void Awake()
    {
        _cam = Camera.main;

        Owner = Managers.Game.Player;
        var input = Owner.Input;
    }

    private void Start()
    {
        OnInstallArchitectureAction += CreateAndSetArchitecture;
        OnRotateArchitectureLeftAction += HandleRotateArchitectureLeft;
        OnRotateArchitectureRightAction += HandleRotateArchitectureRight;
        OnCancelBuildModeAction += HandleCancelBuildMode;

        _virtualJoystick = FindObjectOfType<VirtualJoystick>();
    }

    private void Update()
    {
        if (_isHold)
        {
            if (_virtualJoystick != null)
            {
                Vector2 joystickInput = _virtualJoystick.Handle.anchoredPosition;
                //Vector2 joystickInput = _stateMachine.MovementInput;
                SetObjPositionWithJoystick(joystickInput);
            }
        }
    }

    #region
    private RaycastHit RaycastHit()
    {
        RaycastHit hit;

        // 플레이어의 위치와 방향 받아서 ray 쏘기
        Vector3 playerPosition = transform.position;
        Vector3 raycastStartPoint = playerPosition + transform.forward * 1.0f;

        Ray ray = new Ray(raycastStartPoint, Vector3.down);
        validHIt = Physics.Raycast(ray, out hit, _raycastRange, _currentLayer);

        return hit;
    }

    private void SetObjPosition(Vector3 hitPos)
    {
        Vector3 _location = hitPos;
        _location.Set(Mathf.Round(_location.x), Mathf.Round(_location.y / _yGridSize) * _yGridSize, Mathf.Round(_location.z));

        _obj.transform.position = _location;
    }

    private void SetObjPosition()
    {
        Vector3 _location = RaycastHit().point;
        _location.Set(Mathf.Round(_location.x), Mathf.Round(_location.y / _yGridSize) * _yGridSize, Mathf.Round(_location.z));

        _obj.transform.position = _location;
    }

    private void SetObjPositionWithJoystick(Vector2 joystickInput)
    {
        Vector3 currentPosition = RaycastHit().point;

        // 이동 속도
        //float movementSpeed = 0.3f;
        float movementSpeed = 1.0f;

        // 울타리 이동 및 위치 조정
        _obj.transform.position += new Vector3(joystickInput.x * movementSpeed * Time.deltaTime, 0, joystickInput.y * movementSpeed * Time.deltaTime);


        Vector3 newPosition = _obj.transform.position;
        newPosition.Set(Mathf.Round(newPosition.x), Mathf.Round(newPosition.y / _yGridSize) * _yGridSize, Mathf.Round(newPosition.z));
        _obj.transform.position = newPosition;
    }

    private void CreateBluePrintObject(Vector2 pos)
    {
        //Debug.Log("이건 임시 프리팹 이름이에요 : " + _tempPrefab.name);
        _obj = Instantiate(_tempPrefab);

        SetObjPosition(pos);

        _buildableObject = _obj.GetComponent<BuildableObject>();
        _buildableObject.SetMaterial(_previewMat);

        BuildableObjectColliderManager buildableObject = _obj.GetComponentInChildren<BuildableObjectColliderManager>();
        buildableObject.OnRedMatAction += HandleBuildableObjectTriggerEnter;
        buildableObject.OnBluePrintMatAction += HandleBuildableObjectTriggerExit;
    }

    private void CreateBluePrintObject(GameObject go)
    {
        _obj = go;
        //SetObjPosition(pos);

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
            CreateBluePrintObject(RaycastHit().point);
        }
    }

    private void HandleRotateArchitectureLeft()
    {
        if (_isHold)
            _obj.transform.Rotate(Vector3.up, -_rotationAngle);
    }

    private void HandleRotateArchitectureRight()
    {
        if (_isHold)
            _obj.transform.Rotate(Vector3.up, _rotationAngle);
    }

    private void HandleCancelBuildMode()
    {
        if (_isHold)
        {
            Destroy(_obj);
            _isHold = false;
        }
    }

    private void HandleInstallArchitecture()
    {
        if (_isHold && _canCreateObject)
        {
            _isHold = false;
            _obj.GetComponentInChildren<BoxCollider>().isTrigger = false;

            _buildableObject.SetInitialObject();
            _buildableObject.DestroyColliderManager();
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

    private void CreateAndSetArchitecture()
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

    public void OnCreateBluePrintArchitecture()
    {
        OnCreateBluePrintAction?.Invoke();
    }

    public void OnRotateArchitectureLeft()
    {
        OnRotateArchitectureLeftAction?.Invoke();
    }

    public void OnRotateArchitectureRight()
    {
        OnRotateArchitectureRightAction?.Invoke();
    }

    public void OnCancelBuildMode()
    {
        OnCancelBuildModeAction?.Invoke();
    }

    public void OnInstallArchitecture()
    {
        OnInstallArchitectureAction?.Invoke();
    }

    private void OnDrawGizmos()
    {
        Vector3 playerPosition = transform.position;
        Vector3 raycastStartPoint = playerPosition + transform.forward * 1.0f;

        Gizmos.color = Color.red;

        Vector3 cubeCenter = raycastStartPoint + Vector3.down * _raycastRange * 0.5f;
        Vector3 cubeSize = new Vector3(0.1f, _raycastRange, 0.1f);
        Gizmos.DrawCube(cubeCenter, cubeSize);
    }
}
