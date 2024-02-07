//using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class BossMonster : MonoBehaviour, IAttack, IHit
{
    public enum Parts
    {
        Body,
        Head,        
        LeftWing,
        RightWing,
    }

    [field: SerializeField] public BossAnimationData AnimationData { get; private set; }
    public Animator Animator { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }
    public Collider BodyCollider { get; private set; }
    public ItemDropTable looting;
    public bool Dead { get; private set; }

    public event System.Action<IAttack> OnHit;
    [field: SerializeField] public Condition HP { get; private set; }
    [field: SerializeField] public MonsterSO Data { get; private set; }

    protected BossStateMachine _stateMachine;

    [SerializeField] private MonsterWeapon[] _weapons;

    private void Awake()
    {
        gameObject.layer = 7;
        AnimationData.Initialize();
        Animator = GetComponentInChildren<Animator>();
        NavMeshAgent = Utility.GetOrAddComponent<NavMeshAgent>(gameObject);
        BodyCollider = GetComponent<Collider>();
        HP = new Condition(Data.MaxHP);
        HP.OnUpdated += OnHpUpdated;
        HP.OnBelowedToZero += Die;

        _weapons = GetComponentsInChildren<MonsterWeapon>();
        foreach(var weapon in _weapons)
        {
            weapon.Owner = this;
        }

        _stateMachine = new BossStateMachine(this);
    }

    private void Start()
    {
        //RespawnPosition = transform.position;
        _stateMachine.ChangeState(_stateMachine.SleepState);
        //SetManagementedObject();
    }

    private void Update()
    {
        _stateMachine.Update();        
    }

    private void OnHpUpdated(float percent)
    {

    }

    private void Die()
    {

    }

    public void Attack(IHit target)
    {
        target.Hit(this, 10);
    }

    public void Hit(IAttack attacker, float damage)
    {
        
    }

    public MonsterWeapon GetMonsterWeapon(Parts part)
    {
        return _weapons[(int)part];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _stateMachine.DetectionDist * _stateMachine.DetectionDistModifier);
    }
}
