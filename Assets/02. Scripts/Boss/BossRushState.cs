using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRushState : BossAttackState
{
    private RectAttackIndicator _indicator;

    public BossRushState(BossStateMachine stateMachine) : base(stateMachine)
    {
        _reach = 30.0f;
        cooltime = 15.0f;
        weight = 15.0f;

        var indicatorprefab = Managers.Resource.GetCache<GameObject>("RectAttackIndicator.prefab");
        var go = Object.Instantiate(indicatorprefab, _stateMachine.Boss.transform.position, Quaternion.identity);

        _indicator = go.GetComponentInChildren<RectAttackIndicator>();
        _indicator.gameObject.SetActive(false);
    }

    public override void Enter()
    {
        _stateMachine.Boss.NavMeshAgent.isStopped = true;
        StartAnimation(_stateMachine.Boss.AnimationData.DelayParameterHash);

        var direction = _stateMachine.Target.transform.position - _stateMachine.Boss.transform.position;
        direction.y = 0;
        direction.Normalize();

        _indicator.Activate(_stateMachine.Boss.transform.position, direction, _reach, 1.0f);

        // 1.0 초 대기 후  && 대기 하면서 indicator 확장
        CoroutineManagement.Instance.StartCoroutine(Stay(1.0f,
            ()=> 
            {
                StopAnimation(_stateMachine.Boss.AnimationData.DelayParameterHash);
                _stateMachine.Boss.NavMeshAgent.isStopped = false;
                _stateMachine.MovementSpeedModifier = 3.0f;
                _stateMachine.Boss.BodyCollider.isTrigger = true;
                _stateMachine.Boss.GetMonsterWeapon(BossMonster.Parts.Body).ActivateWeapon();
                
                base.Enter();
                
                var destination = _stateMachine.Boss.transform.position + direction * _reach;
                _stateMachine.Boss.NavMeshAgent.SetDestination(destination);
                
                StartAnimation(_stateMachine.Boss.AnimationData.RushParameterHash);
            }));
    }

    public override void Exit()
    {
        base.Exit();
        _stateMachine.Boss.GetMonsterWeapon(BossMonster.Parts.Body).InactivateWeapon();
        _stateMachine.Boss.BodyCollider.isTrigger = false;

        _stateMachine.Boss.NavMeshAgent.velocity = Vector3.zero;

        StopAnimation(_stateMachine.Boss.AnimationData.RushParameterHash);
    }

    public override void Update()
    {
        if(_stateMachine.Boss.NavMeshAgent.remainingDistance < 1.0f)
        {
            _stateMachine.ChangeState(_stateMachine.BattleState);
        }
    }
}
