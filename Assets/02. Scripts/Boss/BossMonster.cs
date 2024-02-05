using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMonster : MonoBehaviour
{
    [field: SerializeField] public MonsterAnimationData AnimationData { get; private set; }
    public Animator Animator { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }

    public ItemDropTable looting;
    public bool Dead { get; private set; }

    public event Action<IAttack> OnHit;
    [field: SerializeField] public Condition HP { get; private set; }
    [field: SerializeField] public MonsterSO Data { get; private set; }

    protected BossStateMachine _stateMachine;

    private void Awake()
    {
        gameObject.layer = 7;
        AnimationData.Initialize();
        Animator = GetComponentInChildren<Animator>();
        NavMeshAgent = Utility.GetOrAddComponent<NavMeshAgent>(gameObject);

        HP = new Condition(Data.MaxHP);
        HP.OnUpdated += OnHpUpdated;
        HP.OnBelowedToZero += Die;

        _stateMachine = new BossStateMachine(this);
    }

    private void Start()
    {
        //RespawnPosition = transform.position;
        //_stateMachine.ChangeState(_stateMachine.IdleState);
        //SetManagementedObject();
    }

    private void OnHpUpdated(float percent)
    {

    }

    private void Die()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _stateMachine.DetectionDist * _stateMachine.DetectionDistModifier);
    }
}
