using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BossBreathState : BossAttackState
{
    private Coroutine _breathCoroutine;
    private float _normalizedTime;
    private bool _alreadyAttack;

    private int _prefabCount = 7;
    public List<Projectile> _fireObjects = new();

    private GameObject _meteorPrefab;
    private GameObject _indicatorPrefab;
    private IObjectPool<MeteorObject> _meteors;
    private IObjectPool<RectAttackIndicator> _indicators;

    public BossBreathState(BossStateMachine stateMachine) : base(stateMachine)
    {
        _reach = 50.0f;
        cooltime = 10.0f;// 30.0f;
        weight = 20.0f;

        _meteorPrefab = Managers.Resource.GetCache<GameObject>("TerrorBringerProjectile.prefab");
        _indicatorPrefab = Managers.Resource.GetCache<GameObject>("RectAttackIndicator.prefab");

        _meteors = new ObjectPool<MeteorObject>(CreateMeteor, OnGetMeteor, OnReleaseMeteor, OnDestroyMeteor, maxSize: 10);
        _indicators = new ObjectPool<RectAttackIndicator>(CreateIndicator, OnGetIndicator, OnReleaseIndicator, OnDestroyIndicator, maxSize: 10);
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

            var meteor = _meteors.Get();
            var headPosition = _stateMachine.Boss.GetMonsterWeapon(BossMonster.Parts.Head).transform.position;
            var direction = headPosition - _stateMachine.Boss.transform.position;
            direction.y = 0;
            direction.Normalize();

            Managers.Sound.PlayEffectSound(headPosition, "DragonBreath", 0.6f);
            meteor.Fire(headPosition, direction, 25.0f, _reach);

            var indicator = _indicators.Get();
            var pos = new Vector3(headPosition.x, _stateMachine.Boss.transform.position.y, headPosition.z);
            indicator.Activate(pos, direction, _reach, 0.2f);
        }
    }

    #region Object Pooling
    private MeteorObject CreateMeteor()
    {
        MeteorObject meteor = Object.Instantiate(_meteorPrefab, _stateMachine.ObjectPoolContainer).GetComponent<MeteorObject>();
        meteor.SetManagedPool(_meteors);
        meteor.Owner = _stateMachine.Boss;
        return meteor;
    }

    private void OnGetMeteor(MeteorObject meteor)
    {
        meteor.gameObject.SetActive(true);
    }

    private void OnReleaseMeteor(MeteorObject meteor)
    {
        meteor.gameObject.SetActive(false);
    }

    private void OnDestroyMeteor(MeteorObject meteor)
    {
        Object.Destroy(meteor.gameObject);
    }

    private RectAttackIndicator CreateIndicator()
    {
        RectAttackIndicator indicator =
            Object.Instantiate(_indicatorPrefab, _stateMachine.ObjectPoolContainer).GetComponent<RectAttackIndicator>();
        indicator.SetManagedPool(_indicators);
        return indicator;
    }

    private void OnGetIndicator(RectAttackIndicator indicator)
    {
        indicator.gameObject.SetActive(true);
    }

    private void OnReleaseIndicator(RectAttackIndicator indicator)
    {
        indicator.gameObject.SetActive(false);
    }

    private void OnDestroyIndicator(RectAttackIndicator indicator)
    {
        Object.Destroy(indicator.gameObject);
    }
    #endregion
}
