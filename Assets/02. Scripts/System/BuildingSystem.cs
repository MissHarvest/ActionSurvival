using System;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private GameObject _lastHitObjectForBreakMode;
    private BuildableObject _buildableObject;

    private bool _isHold = false;
    private bool _isBreakMode = false;
    private bool _canCreateObject = true;
    private int buildingLayer = 11; // deleteLayer , 적용중인 레이어(Architecture 레이어 번호)

    private bool validHIt = false;

    public Player Owner { get; private set; }

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
        OnInstallArchitectureAction += DestroyArchitecture;
        OnRotateArchitectureLeftAction += HandleRotateArchitectureLeft;
        OnRotateArchitectureRightAction += HandleRotateArchitectureRight;
        OnCancelBuildModeAction += HandleCancelBuildMode;
        OnBreakModeAction += HandleBreakMode;

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

        if (_isBreakMode)
        {
            HandleBreakArchitecture();
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

        float movementSpeed = 0.4f;

        Vector3 newPosition = _obj.transform.position + new Vector3(joystickInput.x * movementSpeed * Time.deltaTime, 0, joystickInput.y * movementSpeed * Time.deltaTime);

        float distance = Vector3.Distance(_obj.transform.position, newPosition);

        float smoothingFactor = 10f;
        float smoothing = Mathf.Clamp01(smoothingFactor / distance);

        // 새로운 위치로 보간
        _obj.transform.position = Vector3.Lerp(_obj.transform.position, newPosition, smoothing);

        _obj.transform.position = new Vector3(
            Mathf.Round(_obj.transform.position.x),
            Mathf.Round(_obj.transform.position.y / _yGridSize) * _yGridSize,
            Mathf.Round(_obj.transform.position.z)
        );
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

    private void SetMaterialForDeletion()
    {
        RaycastHit hit = RaycastHit();
        if (hit.collider != null && hit.collider.gameObject.layer == buildingLayer)
        {
            if (_lastHitObjectForBreakMode != null && _lastHitObjectForBreakMode != hit.collider.gameObject)
                _lastHitObjectForBreakMode.GetComponentInParent<BuildableObject>().SetOriginMaterial();

            _lastHitObjectForBreakMode = hit.collider.gameObject;
            _lastHitObjectForBreakMode.GetComponentInParent<BuildableObject>().SetMaterial(_nonBuildableMat);
        }
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
        if (_isHold && _canCreateObject && !_isBreakMode)
        {
            _isHold = false;
            _obj.GetComponentInChildren<BoxCollider>().isTrigger = false;

            _buildableObject.SetInitialObject();
            _buildableObject.DestroyColliderManager();
        }
    }

    private void HandleBreakMode()
    {
        if (_isHold)
        {
            HandleCancelBuildMode();
        }
        _currentLayer = _destroyLayer;
        _isBreakMode = _isBreakMode ? false : true;
    }

    private void HandleBreakArchitecture()
    {
        RaycastHit();

        if (!validHIt)
        {
            if (_obj)
            {
                VacateObj();
            }
            return;
        }

        if (_obj)
        {
            // 같은 놈이면 리턴
            if (_obj == RaycastHit().collider.gameObject) return;

            // 다른 놈이면
            VacateObj();
            GetObjToRay();
        }
        else
        {
            // 등록된 놈이 없으면
            GetObjToRay();
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
    //생성
    private void CreateAndSetArchitecture()
    {
        if (_isHold)
        {
            // Set            
            HandleInstallArchitecture();
        }
        else if (_isBreakMode == false)
        {
            // Create            
            //var handItemData = Managers.Game.Player.EquippedItem.itemData as ToolItemData;
            if (Managers.Game.Player.QuickSlot.IndexInUse > -1)
            {
                var handItemData = Managers.Game.Player.QuickSlot.slots[Managers.Game.Player.QuickSlot.IndexInUse].itemSlot.itemData as ToolItemData;
                if (handItemData.isArchitecture && handItemData.displayName == "울타리")
                {
                    _tempPrefab = Managers.Resource.GetCache<GameObject>("Handable_Fence.prefab");
                    HandleCreateBluePrint();
                }
            }

        }
    }

    private void DestroyArchitecture()
    {
        if (_isBreakMode && _obj)
        {
            Destroy(_obj.transform.parent.gameObject);
        }
    }

    private void VacateObj()
    {
        _obj.GetComponentInParent<BuildableObject>().SetMaterial(_origionmat);
        //_playerInputs.gameObject.GetComponent<interactionManager>().promptText.gameObject.SetActive(false);
        _obj = null;
    }

    private void GetObjToRay()
    {
        GameObject toBeDestroyedObject = RaycastHit().collider.gameObject;
        _obj = toBeDestroyedObject;
        _origionmat = _obj.GetComponentInParent<BuildableObject>().GetMaterial();
        toBeDestroyedObject.GetComponentInParent<BuildableObject>().SetMaterial(_nonBuildableMat);

        //var promptText = _playerInputs.gameObject.GetComponent<interactionManager>().promptText;
        //promptText.gameObject.SetActive(true);
        //promptText.text = "파괴하기";
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

    public void OnBreakMode()
    {
        OnBreakModeAction?.Invoke();
    }

    public void OnBreakArchitecture()
    {
        OnBreakArchitectureAction?.Invoke();
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
