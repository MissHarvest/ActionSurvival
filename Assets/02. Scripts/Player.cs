using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [field: Header("Animations")]
    [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; }

    public Rigidbody Rigidbody { get; private set; }
    public Animator Animator { get; private set; }
    public PlayerInput Input { get; private set; }
    public CharacterController Controller { get; private set; }
    public ForceReceiver ForceReceiver { get; private set; }
    public InventorySystem Inventory { get; private set; }
    public Transform ViewPoint { get; private set; }
    public ToolSystem ToolSystem { get; private set; }
    public QuickSlotSystem QuickSlot { get; private set; }
    public ItemSlot EquippedItem => ToolSystem.ItemInUse;
    public PlayerConditionHandler ConditionHandler { get; private set; }

    [field: Header("References")]
    [field: SerializeField] public PlayerSO Data { get; private set; }

    private PlayerStateMachine _stateMachine;

    private void Awake()
    {
        Managers.Game.Player = this;

        AnimationData.Initialize();

        Rigidbody = GetComponent<Rigidbody>();
        Animator = GetComponentInChildren<Animator>();
        Input = GetComponent<PlayerInput>();
        Controller = GetComponent<CharacterController>();
        ForceReceiver = GetComponent<ForceReceiver>();
        Inventory = GetComponentInChildren<InventorySystem>();
        ToolSystem = GetComponentInChildren<ToolSystem>();
        ConditionHandler = GetComponent<PlayerConditionHandler>();
        QuickSlot = GetComponentInChildren<QuickSlotSystem>();

        ViewPoint = Utility.FindChild<Transform>(gameObject, "ViewPoint");

        _stateMachine = new PlayerStateMachine(this);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _stateMachine.ChangeState(_stateMachine.IdleState);
    }

    private void Update()
    {
        _stateMachine.HandleInput();
        _stateMachine.Update();
    }

    private void FixedUpdate()
    {
        _stateMachine.PhysicsUpdate();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        if(EquippedItem != null)
        {
            Gizmos.DrawWireSphere(transform.position, ((ToolItemData)EquippedItem.itemData).range);
        }        
    }
}
