using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBreathState : BossAttackState
{
    private Coroutine _breathCoroutine;
    private float _normalizedTime;
    private bool _alreadyAttack;

    private List<RectAttackIndicator> _indicatores = new();
    private int _prefabCount = 7;
    public List<Projectile> _fireObjects = new();

    public BossBreathState(BossStateMachine stateMachine) : base(stateMachine)
    {
        _reach = 50.0f;
        cooltime = 30.0f;
        weight = 20.0f;

        var projectilePrefab = Managers.Resource.GetCache<GameObject>("TerrorBringerProjectile.prefab");
        for(int i = 0; i < _prefabCount; ++i)
        {
            var projectile = Object.Instantiate(projectilePrefab, _stateMachine.Boss.transform).GetComponent<Projectile>();
            projectile.GetComponent<MonsterWeapon>().Owner = _stateMachine.Boss;
            projectile.gameObject.SetActive(false);
            _fireObjects.Add(projectile);
            
        }

        var indicatorPrefab = Managers.Resource.GetCache<GameObject>("RectAttackIndicator.prefab");
        var width = indicatorPrefab.GetComponentInChildren<SpriteRenderer>().sprite.bounds.size.x;
        indicatorPrefab.transform.localScale = new Vector3(0.6f, indicatorPrefab.transform.localPosition.y, _reach/width);
        for(int i = 0; i < _prefabCount; ++i)
        {
            var indicator = Object.Instantiate(indicatorPrefab, _stateMachine.Boss.transform).GetComponent<RectAttackIndicator>();
            indicator.gameObject.SetActive(false);
            _indicatores.Add(indicator);
        }
    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0.0f;
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
        for(int i = 0; i < _prefabCount; ++i)
        {
            yield return new WaitForSeconds(0.15f);
            var headPosition = _stateMachine.Boss.GetMonsterWeapon(BossMonster.Parts.Head).transform.position;
            var direction = headPosition - _stateMachine.Boss.transform.position;
            direction.y = 0;
            direction.Normalize();

            var pos = new Vector3(headPosition.x, _stateMachine.Boss.transform.position.y, headPosition.z);
            _indicatores[i].Activate(pos, direction, _reach, 0.2f);

            _fireObjects[i].transform.position = headPosition;
            var destination = headPosition + direction * _reach;
            _fireObjects[i].Fire(destination, _reach, false);
        }
        //while(_normalizedTime < 0.75f)
        //{
        //    yield return new WaitForSeconds(0.15f);
        //    var go = Object.Instantiate(_projectilePrefab,
        //        _stateMachine.Boss.GetMonsterWeapon(BossMonster.Parts.Head).transform.position,
        //        Quaternion.identity);
        //    go.GetComponent<MonsterWeapon>().Owner = _stateMachine.Boss;
        //    var projectile = go.GetComponent<Projectile>();

        //    var pos = go.transform.position;
        //    pos.y = _stateMachine.Boss.transform.position.y;

        //    if(projectile != null)
        //    {
        //        var direction = _stateMachine.Boss.GetMonsterWeapon(BossMonster.Parts.Head).transform.position - _stateMachine.Boss.transform.position;
        //        direction.y = 0;
        //        direction.Normalize();

        //        var indicator = Object.Instantiate(_indicatorPrefab, pos, Quaternion.LookRotation(direction));
        //        _indicatores.Add(indicator);

        //        var destination = _stateMachine.Boss.transform.position + direction * _reach;
        //        destination.y = 0;
        //        projectile.Fire(destination, _reach);
        //    }
        //}
    }
}
