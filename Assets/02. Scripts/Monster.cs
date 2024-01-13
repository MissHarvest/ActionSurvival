// 작성 날짜 : 2024. 01. 12
// 작성자 : Park Jun Uk

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    [field : SerializeField] public MonsterAnimationData AnimationData { get; private set; }
    protected MonsterStateMachine _stateMachine;
    public Animator Animator { get; private set; }
    [field :SerializeField] public MonsterSO Data { get; private set; }

    public Vector3 RespawnPosition { get; private set; }

    public NavMeshAgent NavMeshAgent { get; private set; }

    private void Awake()
    {   
        AnimationData.Initialize();
        Animator = GetComponentInChildren<Animator>();

        NavMeshAgent = Utility.GetOrAddComponent<NavMeshAgent>(gameObject);
        _stateMachine = new MonsterStateMachine(this);

        RespawnPosition = transform.position;
    }

    private void Start()
    {
        _stateMachine.ChangeState(_stateMachine.IdleState);
    }

    private void Update()
    {
        _stateMachine.Update();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(RespawnPosition, Data.MovementData.PatrolRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _stateMachine.DetectionDist * _stateMachine.DetectionDistModifier);
    }
}
