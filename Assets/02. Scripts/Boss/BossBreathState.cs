using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBreathState : BossAttackState
{
    private GameObject _projectilePrefab;
    private Coroutine _breathCoroutine;
    private float _normalizedTime;
    private bool _alreadyAttack;

    public BossBreathState(BossStateMachine stateMachine) : base(stateMachine)
    {
        _reach = 100.0f;
        _projectilePrefab = Managers.Resource.GetCache<GameObject>("TerrorBringerProjectile.prefab");
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(_stateMachine.Boss.AnimationData.BreathParamterHash);
        
    }

    public override void Exit()
    {
        base.Exit();
        CoroutineManagement.Instance.StopCoroutine(_breathCoroutine);
        _normalizedTime = 0.0f;
        _alreadyAttack = false;
        StopAnimation(_stateMachine.Boss.AnimationData.BreathParamterHash);
    }

    public override void Update()
    {
        _normalizedTime = GetNormalizedTime(_stateMachine.Boss.Animator, "Attack");
        if (_normalizedTime >= 1.0f)
        {            
            _stateMachine.ChangeState(_stateMachine.BattleState);
        }
        else if(_normalizedTime >= 0.3f && _alreadyAttack == false)
        {
            _alreadyAttack = true;
            _breathCoroutine = CoroutineManagement.Instance.StartCoroutine(Breath());
        }
    }

    IEnumerator Breath()
    {
        while(_normalizedTime < 0.75f)
        {
            yield return new WaitForSeconds(0.15f);
            var go = Object.Instantiate(_projectilePrefab,
                _stateMachine.Boss.GetMonsterWeapon(BossMonster.Parts.Head).transform.position,
                Quaternion.identity);
            go.GetComponent<MonsterWeapon>().Owner = _stateMachine.Boss;
            var projectile = go.GetComponent<Projectile>();

            if(projectile != null)
            {
                var direction = _stateMachine.Boss.GetMonsterWeapon(BossMonster.Parts.Head).transform.position - _stateMachine.Boss.transform.position;
                direction.y = 0;
                direction.Normalize();
                var destination = _stateMachine.Boss.transform.position + direction * _reach;
                destination.y = 0;
                projectile.Fire(destination, _reach);
            }
        }
    }
}
