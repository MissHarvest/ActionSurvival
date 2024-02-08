using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBreathState : BossAttackState
{
    private GameObject _projectilePrefab;
    private Coroutine _breathCoroutine;
    private float _normalizedTime;
    private bool _alreadyAttack;
    private GameObject _indicatorPrefab;
    private List<GameObject> _indicatores = new List<GameObject>();

    public BossBreathState(BossStateMachine stateMachine) : base(stateMachine)
    {
        _reach = 50.0f;
        cooltime = 60.0f;
        weight = 20.0f;

        _projectilePrefab = Managers.Resource.GetCache<GameObject>("TerrorBringerProjectile.prefab");
        _indicatorPrefab = Managers.Resource.GetCache<GameObject>("RectAttackIndicator.prefab");
        var width = _indicatorPrefab.GetComponentInChildren<SpriteRenderer>().sprite.bounds.size.x;

        _indicatorPrefab.transform.localScale = new Vector3(0.6f, _indicatorPrefab.transform.localPosition.y, _reach/width);
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
        foreach(var indicator in _indicatores)
        {
            Object.Destroy(indicator);
        }
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

            var pos = go.transform.position;
            pos.y = 0;

            if(projectile != null)
            {
                var direction = _stateMachine.Boss.GetMonsterWeapon(BossMonster.Parts.Head).transform.position - _stateMachine.Boss.transform.position;
                direction.y = 0;
                direction.Normalize();

                var indicator = Object.Instantiate(_indicatorPrefab, pos, Quaternion.LookRotation(direction));
                _indicatores.Add(indicator);

                var destination = _stateMachine.Boss.transform.position + direction * _reach;
                destination.y = 0;
                projectile.Fire(destination, _reach);
            }
        }
    }
}
