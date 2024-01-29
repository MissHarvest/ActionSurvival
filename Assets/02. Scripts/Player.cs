using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IHit
{
    [field: Header("Animations")]
    public PlayerAnimationData AnimationData { get; private set; } = new PlayerAnimationData();

    public Rigidbody Rigidbody { get; private set; }
    public Animator Animator { get; private set; }
    public PlayerInput Input { get; private set; }
    public CharacterController Controller { get; private set; }
    public ForceReceiver ForceReceiver { get; private set; }
    public PlayerInventorySystem Inventory { get; private set; }
    public Transform ViewPoint { get; private set; }
    public ToolSystem ToolSystem { get; private set; }
    public QuickSlotSystem QuickSlot { get; private set; }
    public Recipe Recipe { get; private set; }
    public Cooking Cooking { get; private set; }
    public BuildingSystem Building { get; private set; }
    public ItemSlot EquippedItem => ToolSystem.ItemInUse;
    public PlayerConditionHandler ConditionHandler { get; private set; }

    [field: Header("References")]
    public PlayerSO Data { get; private set; }

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
        Inventory = GetComponentInChildren<PlayerInventorySystem>();
        ToolSystem = GetComponentInChildren<ToolSystem>();
        ConditionHandler = GetComponent<PlayerConditionHandler>();
        QuickSlot = GetComponentInChildren<QuickSlotSystem>();
        Recipe = GetComponentInChildren<Recipe>();
        Cooking = GetComponentInChildren<Cooking>();
        Building = GetComponentInChildren<BuildingSystem>();

        Data = Managers.Resource.GetCache<PlayerSO>("PlayerSO.data");

        ViewPoint = Utility.FindChild<Transform>(gameObject, "ViewPoint");

        _stateMachine = new PlayerStateMachine(this);

        // [ Save Test ] //
        Managers.Game.OnSaveCallback += Save;
        if(SaveGame.TryLoadJsonFile<Vector3>(SaveGame.SaveType.Runtime, "PlayerPosition", out Vector3 pos))
        {
            transform.position = pos;
        }
    }

    private void Start()
    {
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

    public void Hit(IAttack attacker, float damage)
    {
        ConditionHandler.HP.Subtract(damage);
        Debug.Log($"[ Attacked by ] {attacker}");
    }
       
    public void Save()
    {
        var json = JsonUtility.ToJson(transform.position);
        SaveGame.CreateJsonFile("PlayerPosition", json, SaveGame.SaveType.Runtime);
    }
}
