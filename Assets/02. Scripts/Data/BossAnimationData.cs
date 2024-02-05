using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BossAnimationData
{
    [SerializeField] private string idleParameterName = "Idle";
    [SerializeField] private string attackParameterName = "Attack";
    [SerializeField] private string dieParamterName = "Die";
    public int IdleParameterHash { get; private set; }
    public int AttackParameterHash { get; private set; }
    public int DieParameterHash { get; private set; }

    public void Initialize()
    {
        IdleParameterHash = Animator.StringToHash(idleParameterName);
        AttackParameterHash = Animator.StringToHash(attackParameterName);
        DieParameterHash = Animator.StringToHash(dieParamterName);
    }
}
