using System;
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
    public Tutorial Tutorial { get; private set; }
    public ArmorSystem ArmorSystem { get; private set; }
    public ItemSlot EquippedItem => ToolSystem.ItemInUse;
    public PlayerConditionHandler ConditionHandler { get; private set; }

    [field: Header("References")]
    public PlayerSO Data { get; private set; }
    public event Action OnHit;

    public int playerDefense = 0;

    private PlayerStateMachine _stateMachine;

    [field: SerializeField] public string StandingIslandName { get; set; } = "CenterIsland";

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
        Tutorial = GetComponentInChildren<Tutorial>();
        ArmorSystem = GetComponentInChildren<ArmorSystem>();

        Data = Managers.Resource.GetCache<PlayerSO>("PlayerSO.data");

        ViewPoint = Utility.FindChild<Transform>(gameObject, "ViewPoint");

        _stateMachine = new PlayerStateMachine(this);

        Managers.Game.OnSaveCallback += Save;

        if (PlayerPrefs.HasKey("IslandName"))
        {
            StandingIslandName = PlayerPrefs.GetString("IslandName");
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

    public void Hit(IAttack attacker, float damage) // 인벤토리의 방어구 idx를 찾아서 UseToolItemByIndex()를 Hit가 실행될 때마다 호출
    {
        //코드 줄이기
        OnHit.Invoke();
        float blockedDamage = damage - playerDefense;

        if (blockedDamage > 0)
        {
            Managers.Sound.PlayEffectSound(transform.position, "Hit");
            ConditionHandler.HP.Subtract(blockedDamage);
            Debug.Log($"[ Attacked by ] {attacker}");
        }
        else
        {
            Managers.Sound.PlayEffectSound(transform.position, "Hit");
            ConditionHandler.HP.Subtract(1f);
            Debug.Log($"[ Attacked by ] {attacker}");
        }
    }

    public void Die()
    {
        _stateMachine.ChangeState(_stateMachine.DieState);
        SaveGame.DeleteAllFiles();
        Managers.UI.ShowPopupUI<UIDeath>();        
    }
       
    public void Save()
    {
        var json = JsonUtility.ToJson(transform.position);
        SaveGame.CreateJsonFile("PlayerPosition", json, SaveGame.SaveType.Runtime);
        PlayerPrefs.SetString("IslandName", StandingIslandName);
    }

    public void Load()
    {
        if (SaveGame.TryLoadJsonFile<Vector3>(SaveGame.SaveType.Runtime, "PlayerPosition", out Vector3 pos))
        {
            Controller.enabled = false;
            transform.position = pos;
            Controller.enabled = true;
        }
        else
        {
            Controller.enabled = false;
            transform.position = new Vector3(-40f, 1f, 22f);
            Controller.enabled = true;
        }
    }
}
