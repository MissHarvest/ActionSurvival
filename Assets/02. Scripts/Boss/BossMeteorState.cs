using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class BossMeteorState : BossAttackState
{
    private enum State
    {
        TakeOff,
        Fly,
        Land,
    }

    private GameObject _meteorPrefab;
    private GameObject _indicatorPrefab;
    private IObjectPool<MeteorObject> _meteores;
    private IObjectPool<AttackIndicatorCircle> _indicatores;
    
    private State _currentState = State.TakeOff;
    private List<Coroutine> _coroutines = new List<Coroutine>();

    public BossMeteorState(BossStateMachine stateMachine) : base(stateMachine)
    {
        _reach = 50.0f;
        weight = 9999.0f;

        _meteorPrefab = Managers.Resource.GetCache<GameObject>("TerrorBringerMeteor.prefab");
        _meteores = new ObjectPool<MeteorObject>(CreateMeteor, OnGetMeteor, OnReleaseMeteor, OnDestroyMeteor, maxSize: 30);

        _indicatorPrefab = Managers.Resource.GetCache<GameObject>("CircleAttackIndicator.prefab");
        _indicatores = new ObjectPool<AttackIndicatorCircle>(CreateIndicator, OnGetIndicator, OnReleaseIndicator, OnDestroyIndicator, maxSize: 30);

        _stateMachine.Boss.HP.OnBelowedToZero += Cancel;
    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0.0f;
        _stateMachine.Skills.Remove(this);
        base.Enter();
        _currentState = State.TakeOff;
        StartAnimation(_stateMachine.Boss.AnimationData.FlyParameterHash);
    }

    public override void Update()
    {
        float normalizedTime;
        switch (_currentState)
        {
            case State.TakeOff:
                normalizedTime = GetNormalizedTime(_stateMachine.Boss.Animator, "TakeOff");
                _stateMachine.Boss.NavMeshAgent.baseOffset = normalizedTime * 2.0f;
                if(normalizedTime >= 0.8f)
                {
                    ChangeState(State.Fly);
                }
                break;

            case State.Land:
                normalizedTime = GetNormalizedTime(_stateMachine.Boss.Animator, "Land");
                _stateMachine.Boss.NavMeshAgent.baseOffset = 2.0f * (1 - normalizedTime);
                if (normalizedTime >= 0.8f)
                {
                    _stateMachine.Boss.NavMeshAgent.baseOffset = 0.0f;
                    _stateMachine.ChangeState(_stateMachine.BattleState);
                }
                break;
        }
    }

    private void ChangeState(State state)
    {
        _currentState = state;

        switch (_currentState)
        {
            case State.TakeOff:
                
                break;

            case State.Fly:
                _stateMachine.Boss.NavMeshAgent.baseOffset = 2.0f;

                // Meteor - Coroutine //
                var coroutine = CoroutineManagement.Instance.StartCoroutine(FallMeteor());
                _coroutines.Add(coroutine);
                CoroutineManagement.Instance.StartCoroutine(FireToPlayer());                
                break;

            case State.Land:
                StopAnimation(_stateMachine.Boss.AnimationData.FlyParameterHash);
                break;
        }
    }

    IEnumerator FallMeteor()
    {
        Managers.Sound.PlayEffectSound(_stateMachine.Target.transform.position, "Falling", 0.15f);
        while (true)
        {
            var sec = Random.Range(0.1f, 1.0f);
            yield return new WaitForSeconds(sec);
            RandomMeteor();
        }
    }

    IEnumerator FireToPlayer()
    {
        int time = 0;
        while (time != 40)
        {
            yield return new WaitForSeconds(0.25f);
            ++time;

            FireMeteorToTarget();            
        }

        ChangeState(State.Land);
    }

    private void RandomMeteor()
    {
        List<Vector3> list = new List<Vector3>();

        for(int i = 0; i < 2; ++i)
        {
            var position = new Vector3(
                    _stateMachine.Boss.transform.position.x + Random.Range(-30.0f, 30.0f),
                    _stateMachine.Boss.transform.position.y + Random.Range(5.0f, 15.0f),
                    _stateMachine.Boss.transform.position.z + Random.Range(-30.0f, 30.0f));
            list.Add(position);
        }

        for(int i = 0; i < list.Count; ++i)
        {
            var meteor = _meteores.Get();
            meteor.Fall(list[i], 10.0f);

            var indicator = _indicatores.Get();
            if (meteor.Collider is SphereCollider collider)
            {
                indicator.Activate(meteor.Destination, meteor.Speed, meteor.MaxDistance, collider);
            }
        }
    }

    private void FireMeteorToTarget()
    {
        var meteor = _meteores.Get();
        
        // 타겟의 위치로 부터 랜덤한 자리의 상공
        var position = _stateMachine.Target.transform.position
            + new Vector3(Random.Range(-5.0f, 5.0f), 0, Random.Range(-5.0f, 5.0f))
            + Vector3.up * 30.0f;

        meteor.Fall(position , 10.0f);

        var indicator = _indicatores.Get();
        if(meteor.Collider is SphereCollider collider)
        {
            indicator.Activate(meteor.Destination, meteor.Speed, meteor.MaxDistance, collider);
        }
    }

    public override void Cancel()
    {
        for (int i = 0; i < _coroutines.Count; ++i)
        {
            CoroutineManagement.Instance.StopCoroutine(_coroutines[i]);
        }
        _coroutines.Clear();
    }

    #region Object Pooling
    private MeteorObject CreateMeteor()
    {
        MeteorObject meteor = Object.Instantiate(_meteorPrefab, _stateMachine.ObjectPoolContainer).GetComponent<MeteorObject>();
        meteor.SetManagedPool(_meteores);
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

    private AttackIndicatorCircle CreateIndicator()
    {
        AttackIndicatorCircle indicator =
            Object.Instantiate(_indicatorPrefab, _stateMachine.ObjectPoolContainer).GetComponent<AttackIndicatorCircle>();
        indicator.SetManagedPool(_indicatores);
        return indicator;
    }

    private void OnGetIndicator(AttackIndicatorCircle indicator)
    {
        indicator.gameObject.SetActive(true);
    }

    private void OnReleaseIndicator(AttackIndicatorCircle indicator)
    {
        indicator.gameObject.SetActive(false);
    }

    private void OnDestroyIndicator(AttackIndicatorCircle indicator)
    {
        Object.Destroy(indicator.gameObject);
    }
    #endregion
}
