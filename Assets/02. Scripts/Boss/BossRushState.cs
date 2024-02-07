using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRushState : BossAttackState
{
    private GameObject indicator;
    public BossRushState(BossStateMachine stateMachine) : base(stateMachine)
    {
        _reach = 30.0f;
        cooltime = 10.0f;

        var indicatorprefab = Managers.Resource.GetCache<GameObject>("RectAttackIndicator.prefab");
        indicator = Object.Instantiate(indicatorprefab, _stateMachine.Boss.transform.position, Quaternion.identity);

        var width = indicator.GetComponentInChildren<SpriteRenderer>().sprite.bounds.size.x;
        //Debug.Log($"[Widht] {width}");
        indicator.transform.localScale = new Vector3(indicator.transform.localScale.x, indicator.transform.localScale.y, 40.0f / width);

        indicator.SetActive(false);
    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 3.0f;
        _stateMachine.Boss.BodyCollider.isTrigger = true;
        _stateMachine.Boss.GetMonsterWeapon(BossMonster.Parts.Body).ActivateWeapon();
        base.Enter();
        var direction = _stateMachine.Target.transform.position - _stateMachine.Boss.transform.position;
        direction.y = 0;
        direction.Normalize();
        var destination = _stateMachine.Boss.transform.position + direction * 40.0f;
        _stateMachine.Boss.NavMeshAgent.SetDestination(destination);

        indicator.transform.position = _stateMachine.Boss.transform.position;
        indicator.transform.rotation = Quaternion.LookRotation(direction);
        indicator.SetActive(true);
        StartAnimation(_stateMachine.Boss.AnimationData.RushParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        _stateMachine.Boss.GetMonsterWeapon(BossMonster.Parts.Body).InactivateWeapon();
        _stateMachine.Boss.BodyCollider.isTrigger = false;
        indicator.SetActive(false);
        StopAnimation(_stateMachine.Boss.AnimationData.RushParameterHash);
    }

    public override void Update()
    {
        if(_stateMachine.Boss.NavMeshAgent.remainingDistance < 0.1f)
        {
            _stateMachine.ChangeState(_stateMachine.BattleState);
        }
    }
}
