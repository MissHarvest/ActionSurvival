using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IAttack, IHit
{
    #region Components
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
    public ItemUsageHelper ItemUsageHelper { get; private set; }
    public PlayerConditionHandler ConditionHandler { get; private set; }
    #endregion

    public QuickSlot EquippedItem => ToolSystem.EquippedTool;

    [field: Header("References")]
    public PlayerSO Data { get; private set; }
    public event Action OnHit;

    private PlayerStateMachine _stateMachine;

    private GameObject _listener;

    [field: SerializeField] public string StandingIslandName { get; set; } = "CenterIsland";

    private void Awake()
    {
        GameManager.Instance.Player = this;

        AnimationData.Initialize();

        SetComponents();

        Data = Managers.Resource.GetCache<PlayerSO>("PlayerSO.data");

        ViewPoint = Utility.FindChild<Transform>(gameObject, "ViewPoint");

        var listener = Managers.Resource.GetCache<GameObject>("@PlayerAudioListener.prefab");
        _listener = Instantiate(listener);

        _stateMachine = new PlayerStateMachine(this);

        GameManager.Instance.OnSaveCallback += Save;

        if (PlayerPrefs.HasKey("IslandName"))
        {
            StandingIslandName = PlayerPrefs.GetString("IslandName");
        }
    }

    private void SetComponents()
    {
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
        ItemUsageHelper = GetComponentInChildren<ItemUsageHelper>();
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

    public void Attack(float damage)
    {
        var hits = Physics.BoxCastAll(transform.position + transform.forward * 0.5f,
            Vector3.one * 0.5f, 
            Vector3.up,
            transform.rotation,
            0,
            1 << 7);

        Debug.Log($"[BoxCast] {hits.Length}");
        for(int i = 0; i < hits.Length; ++i)
        {
            var target = hits[i].collider.GetComponent<IHit>();
            if (target == null) continue;
            AttackInfo attackData = new AttackInfo(target, damage);
            Attack(attackData);
        }
    }

    public void Attack(AttackInfo attack)
    {
        if (attack.target == null) return;
        attack.target.Hit(this, attack.damage);
        Inventory.TrySubtractDurability(EquippedItem.targetIndex, 1.0f);
    }

    public void Hit(IAttack attacker, float damage)
    {
        int armorDefense = ArmorSystem.GetDefense();

        OnHit.Invoke();
        float blockedDamage = damage - armorDefense;

        if (blockedDamage > 0)
        {
            ConditionHandler.HP.Subtract(blockedDamage);
        }
        else
        {
            ConditionHandler.HP.Subtract(1f);
        }

        Managers.Sound.PlayEffectSound(transform.position, "Hit", 1.0f, false);
        Debug.Log($"[ Attacked by ] {attacker}");
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + transform.forward * 0.5f, Vector3.one * 0.5f);
    }
}
