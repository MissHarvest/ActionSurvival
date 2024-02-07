using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossMeteorState : BossAttackState
{
    private enum State
    {
        TakeOff,
        Fly,
        Land,
    }
    private GameObject _projectilePrefab;
    private GameObject _indicatorPrefab;
    private State _currentState = State.TakeOff;

    public BossMeteorState(BossStateMachine stateMachine) : base(stateMachine)
    {
        _reach = 50.0f;
        _projectilePrefab = Managers.Resource.GetCache<GameObject>("TerrorBringerMeteor.prefab");
        _indicatorPrefab = Managers.Resource.GetCache<GameObject>("CircleAttackIndicator.prefab");
    }

    public override void Enter()
    {
        base.Enter();
        _currentState = State.TakeOff;
        StartAnimation(_stateMachine.Boss.AnimationData.FlyParameterHash);
    }

    public override void Exit()
    {
        //base.Exit();        
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
                CoroutineManagement.Instance.StartCoroutine(FallMeteor());
                //
                Debug.Log("Fly");
                
                break;

            case State.Land:
                StopAnimation(_stateMachine.Boss.AnimationData.FlyParameterHash);
                break;
        }
    }

    IEnumerator FallMeteor()
    {
        int time = 0;
        while (time != 100)
        {
            yield return new WaitForSeconds(0.1f);
            ++time;

            RandomMeteor();
            //FireMeteorToTarget();
        }
        
        ChangeState(State.Land);
    }

    private void RandomMeteor()
    {
        List<Vector3> list = new List<Vector3>();

        for(int i = 0; i < 20; ++i)
        {
            var position = new Vector3(
                    _stateMachine.Boss.transform.position.x + Random.Range(-30.0f, 30.0f),
                    _stateMachine.Boss.transform.position.y + Random.Range(2.0f, 5.0f),
                    _stateMachine.Boss.transform.position.z + Random.Range(-30.0f, 30.0f));
            list.Add(position);
        }

        var direction = new Vector3(-1, -2, 0);

        for(int i = 0; i < list.Count; ++i)
        {
            if (Physics.Raycast(list[i], direction, out RaycastHit hit, 100, 1 << 12))
            {
                //hit.distance
                var go = Object.Instantiate(_projectilePrefab,
                list[i],
                Quaternion.identity);
                go.GetComponent<MonsterWeapon>().Owner = _stateMachine.Boss;
                var projectile = go.GetComponent<Projectile>();

                var indicatorGo = Object.Instantiate(_indicatorPrefab, hit.point + Vector3.up * 0.1f, _indicatorPrefab.transform.rotation);
                var indicator = indicatorGo.GetComponent<AttackIndicatorCircle>();

                if (projectile != null)
                {
                    var sphereCollider = projectile.GetComponent<SphereCollider>();
                    projectile.Fire(hit.point, hit.distance);
                    indicator.Set(hit.distance, projectile.Speed, sphereCollider.radius);
                }
            }
        }
    }

    private void FireMeteorToTarget()
    {
        var position = _stateMachine.Target.transform.position + Vector3.up * 25.0f;

        //var direction = _stateMachine.Target.transform.position - position + Vector3.up * 0.5f;
        var direction = Vector3.down;

        if (Physics.Raycast(position, direction, out RaycastHit hit, 100, 1 << 12))
        {
            var destination = hit.point + new Vector3(Random.Range(-5.0f, 5.0f), 0, Random.Range(-5.0f, 5.0f));
            var dist = Vector3.Distance(position, destination);

            var go = Object.Instantiate(_projectilePrefab,
            position,
            Quaternion.identity);
            go.GetComponent<MonsterWeapon>().Owner = _stateMachine.Boss;
            var projectile = go.GetComponent<Projectile>();

            var indicatorGo = Object.Instantiate(_indicatorPrefab, destination + Vector3.up * 0.1f, _indicatorPrefab.transform.rotation);
            var indicator = indicatorGo.GetComponent<AttackIndicatorCircle>();

            if (projectile != null)
            {
                var sphereCollider = projectile.GetComponent<SphereCollider>();
                projectile.Fire(destination, dist);
                indicator.Set(dist, projectile.Speed, sphereCollider.radius);
            }
        }
    }
}
