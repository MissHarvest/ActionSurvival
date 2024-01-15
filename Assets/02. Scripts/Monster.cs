// �ۼ� ��¥ : 2024. 01. 12
// �ۼ��� : Park Jun Uk 

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public abstract class Monster : MonoBehaviour
{
    [field : SerializeField] public MonsterAnimationData AnimationData { get; private set; }
    protected MonsterStateMachine _stateMachine;
    public Animator Animator { get; private set; }
    [field :SerializeField] public MonsterSO Data { get; private set; }

    public Vector3 RespawnPosition { get; private set; }

    public NavMeshAgent NavMeshAgent { get; private set; }

    public ItemData[] looting;

    public bool Dead { get; private set; }

    private void Awake()
    {
        AnimationData.Initialize();
        Animator = GetComponentInChildren<Animator>();
        Data = Managers.Resource.GetCache<MonsterSO>($"{this.GetType().Name}.data");
        _stateMachine = new MonsterStateMachine(this);

        NavMeshAgent = Utility.GetOrAddComponent<NavMeshAgent>(gameObject);        
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

    public void Respawn()
    {
        Dead = false;
        _stateMachine.ChangeState(_stateMachine.IdleState);

        // HP 복구
    }

    public void Die()
    {
        Dead = true;
        _stateMachine.ChangeState(_stateMachine.DieState);

        // TEMP //
        foreach(var loot in looting)
        {
            Managers.Game.Player.Inventory.AddItem(loot, 1);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(RespawnPosition, Data.MovementData.PatrolRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _stateMachine.DetectionDist * _stateMachine.DetectionDistModifier);
    }

    public abstract void Attack();
}
