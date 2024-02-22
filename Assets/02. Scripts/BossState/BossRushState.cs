using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRushState : BossAttackState
{
    private GameObject _indicatorPrefab;

    public BossRushState(BossStateMachine stateMachine) : base(stateMachine)
    {
        _reach = 30.0f;
        cooltime = 15.0f;
        weight = 15.0f;

        _indicatorPrefab = Managers.Resource.GetCache<GameObject>("RectAttackIndicator.prefab");
    }

    public override void Enter()
    {
        _stateMachine.Boss.NavMeshAgent.isStopped = true;
        StartAnimation(_stateMachine.Boss.AnimationData.DelayParameterHash);

        var direction = _stateMachine.Target.transform.position - _stateMachine.Boss.transform.position;
        direction.y = 0;
        direction.Normalize();

        var dist = _reach;
        var destination = _stateMachine.Boss.transform.position + direction * _reach;
        _stateMachine.Boss.NavMeshAgent.SetDestination(destination);

        if (Physics.Raycast(_stateMachine.Boss.transform.position + Vector3.up * 0.5f, direction, out RaycastHit hit, _reach, 1 << 12))
        {
            if(hit.normal.y == 0)
            {
                Debug.Log("[ Normal ] " + hit.normal);
                destination = hit.point - direction * 0.5f;
                dist = hit.distance;
            }
        }

        ShowIndicator(direction, dist, 1.0f);

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

    private void ShowIndicator(Vector3 direction, float dist, float time)
    {
        var go = Object.Instantiate(_indicatorPrefab, _stateMachine.Boss.transform.position, Quaternion.identity);
        var indicator = go.GetComponentInChildren<RectAttackIndicator>();
        indicator.Activate(_stateMachine.Boss.transform.position, direction, dist, 1.0f);
    }
}
