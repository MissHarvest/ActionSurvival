using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BossAnimationData
{
    [SerializeField] private string idleParameterName = "Idle";
    [SerializeField] private string sleepParameterName = "Sleep";
    [SerializeField] private string screamParameterName = "Scream";
    [SerializeField] private string rushParameterName = "Rush";
    [SerializeField] private string breathParameterName = "Breath";
    [SerializeField] private string battleParameterName = "@Attack";
    [SerializeField] private string biteParameterName = "Bite";
    [SerializeField] private string stabParameterName = "Stab";
    [SerializeField] private string flyParameterName = "Fly";
    [SerializeField] private string dieParamterName = "Die";

    public int IdleParameterHash { get; private set; }
    public int SleepParameterHash { get; private set; }

    public int ScreamParamterHash { get; private set; }
    public int BreathParamterHash { get; private set; }
    public int RushParameterHash { get; private set; }
    public int BattleParamterHash { get; private set; }
    public int BiteParameterHash { get; private set; }
    public int StabParameterHash { get; private set; }
    public int DieParameterHash { get; private set; }
    public int FlyParameterHash { get; private set; }

    public void Initialize()
    {
        IdleParameterHash = Animator.StringToHash(idleParameterName);
        SleepParameterHash = Animator.StringToHash(sleepParameterName);
        ScreamParamterHash = Animator.StringToHash(screamParameterName);
        BreathParamterHash = Animator.StringToHash(breathParameterName);
        RushParameterHash = Animator.StringToHash(rushParameterName);
        BattleParamterHash = Animator.StringToHash(battleParameterName);
        BiteParameterHash = Animator.StringToHash(biteParameterName);
        StabParameterHash = Animator.StringToHash(stabParameterName);
        FlyParameterHash = Animator.StringToHash(flyParameterName);
        DieParameterHash = Animator.StringToHash(dieParamterName);
    }
}
