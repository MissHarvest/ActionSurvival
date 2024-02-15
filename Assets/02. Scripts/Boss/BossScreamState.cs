using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

public class BossScreamState : BossAttackState
{
    private bool _canScream = true;
    private float _sfxVolume = 0.7f;
    private Dictionary<int, GameObject> _monsterPrefab = new();
    private Vector3[] _summonPositions;
    private List<GameObject> _monsterList = new();

    public BossScreamState(BossStateMachine stateMachine) : base(stateMachine)
    {
        _reach = 100.0f;
        cooltime = 60.0f;
        weight = 1.0f;

        _monsterPrefab.TryAdd(1, Managers.Resource.GetCache<GameObject>("FireSwarm.prefab"));
        _monsterPrefab.TryAdd(2, Managers.Resource.GetCache<GameObject>("RedMetalon.prefab"));
        _monsterPrefab.TryAdd(3, Managers.Resource.GetCache<GameObject>("RedSoulEater.prefab"));

        // Set Position //
        CoroutineManagement.Instance.StartCoroutine(CaculateSummonPosition());
    }

    IEnumerator CaculateSummonPosition()
    {
        SummonPositionJob job = new SummonPositionJob(
            _stateMachine.Boss.transform.forward,
            Vector3.up,
            5.0f,
            20.0f,
            5);

        var handle = job.Schedule();
        while (!handle.IsCompleted)
            yield return null;

        handle.Complete();
        _summonPositions = job.GetResult();
    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0.0f;
        base.Enter();        
        StartAnimation(_stateMachine.Boss.AnimationData.ScreamParamterHash);
        
    }

    public override void Exit()
    {
        base.Exit();
        _canScream = true;
        StopAnimation(_stateMachine.Boss.AnimationData.ScreamParamterHash);
    }

    public override void Update()
    {
        float normalizedTime = GetNormalizedTime(_stateMachine.Boss.Animator, "Scream");
        if (normalizedTime >= 1.0f)
        {
            _stateMachine.ChangeState(_stateMachine.BattleState);
        }
        else if(normalizedTime > 0.2f && _canScream)
        {
            _canScream = false;
            Managers.Sound.PlayEffectSound(_stateMachine.Boss.transform.position, "Scream", _sfxVolume);
            SummonMonster();
        }       
    }

    private void SummonMonster()
    {
        if (_stateMachine.isBattaleState == false)
        {
            _stateMachine.isBattaleState = true;
            return;
        }

        if (_monsterPrefab.TryGetValue(_stateMachine.Phase, out GameObject prefab))
        {
            for (int i = 0; i < _summonPositions.Length; ++i)
            {
                var look = _stateMachine.Target.transform.position - _stateMachine.Boss.transform.position - _summonPositions[i];
                var monster = Object.Instantiate(prefab,
                    _stateMachine.Boss.transform.position + _summonPositions[i],
                    Quaternion.LookRotation(look));
                monster.GetComponent<Monster>().SetBerserkMode();
                _monsterList.Add(monster);
            }
        }
    }

    public override void Cancel()
    {
        for(int i = 0; i < _monsterList.Count; ++i)
        {
            Object.Destroy(_monsterList[i]);
        }
        _monsterList.Clear();
    }
}
