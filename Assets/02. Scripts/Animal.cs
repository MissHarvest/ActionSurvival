using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 2024. 01. 15 Park Jun Uk
public class Animal : MonoBehaviour, IHit
{
    [field : SerializeField] public AnimalAnimationData AnimationData { get; private set; }
    protected AnimalStateMachine _stateMachine;

    [field: SerializeField] public AnimalSO Data { get; private set; }

    public Animator Animator { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }

    public Vector3 RespawnPosition { get; set; }

    public ItemData[] looting;

    [field: SerializeField] public Condition HP { get; private set; }

    public bool Dead { get; private set; } = false;

    private void Awake()
    {
        AnimationData.Initialize();
        Animator = GetComponentInChildren<Animator>();
        _stateMachine = new AnimalStateMachine(this);

        NavMeshAgent = Utility.GetOrAddComponent<NavMeshAgent>(gameObject);

        HP = new Condition(Data.MaxHP);
        HP.OnBelowedToZero += Die;
    }

    private void Start()
    {
        RespawnPosition = transform.position;
        _stateMachine.ChangeState(_stateMachine.IdleState);
    }

    private void Update()
    {
        _stateMachine.Update();
    }

    public void Hit(IAttack attacker, float damage)
    {
        HP.Subtract(damage);
        _stateMachine.Attacker = (attacker as MonoBehaviour).gameObject;
        _stateMachine.ChangeState(_stateMachine.FleeState);
    }

    public void Die()
    {
        Dead = true;
        _stateMachine.ChangeState(_stateMachine.DieState);

        // [ TEMP ] Get Looting //
        foreach (var loot in looting)
        {
            //Managers.Game.Player.Inventory.AddItem(loot, 1);
        }
    }

    public void Respawn()
    {
        Dead = false;
        _stateMachine.ChangeState(_stateMachine.IdleState);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(RespawnPosition, Data.MovementData.PatrolRadius);
    }
}
