using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComboAttackState : PlayerAttackState
{
    private bool _alreadyAppliedForce;
    private bool _alreadyApplyCombo;
    private Weapon _weapon;

    private AttackInfoData _attackInfoData;

    public PlayerComboAttackState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {

    }    

    public override void Enter()
    {
        base.Enter();
        StartAnimation(_stateMachine.Player.AnimationData.ComboAttackParameterHash);

        _alreadyAppliedForce = false;
        _alreadyApplyCombo = false;

        int comboIndex = _stateMachine.ComboIndex;
        _attackInfoData = _stateMachine.Player.Data.AttackData.GetAttackInfo(comboIndex);
        _stateMachine.Player.Animator.SetInteger("Combo", comboIndex);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(_stateMachine.Player.AnimationData.ComboAttackParameterHash);

        if (!_alreadyApplyCombo)
            _stateMachine.ComboIndex = 0;
    }

    private void TryComboAttack()
    {
        if (_alreadyApplyCombo) return;

        if (_attackInfoData.ComboStateIndex == -1) return;

        if (!_stateMachine.IsAttacking) return;

        _alreadyApplyCombo = true;
    }

    private void TryApplyForce()
    {
        if (_alreadyAppliedForce) return;
        _alreadyAppliedForce = true;

        //_stateMachine.Player.ForceReceiver.Reset();

        _stateMachine.Player.ForceReceiver.AddForce(_stateMachine.Player.transform.forward * _attackInfoData.Force);
    }

    public override void Rotate(Vector3 movementDirection)
    {

    }

    public override void Update()
    {
        ToolItemData toolItemDate = (ToolItemData)Managers.Game.Player.ToolSystem.ItemInUse.itemData;
        base.Update();

        ForceMove();

        float normalizedTime = GetNormalizedTime(_stateMachine.Player.Animator, "Attack");
        if (normalizedTime < 1f)
        {
            if (normalizedTime >= _attackInfoData.ForceTransitionTime)
            {
                TryApplyForce();
            }

            if (normalizedTime >= _attackInfoData.ComboTransitionTime)
            {
                _weapon = Managers.Game.Player.GetComponentInChildren<Weapon>(); // lgs 24.01.22 애니메이션 재생 중에 콜라이더가 켜지고 꺼지게 수정함.
                _weapon.gameObject.GetComponentInChildren<BoxCollider>().enabled = true;
                TryComboAttack();
            }
        }
        else
        {
            if(_alreadyApplyCombo)
            {
                _stateMachine.ComboIndex = _attackInfoData.ComboStateIndex;
                _stateMachine.ChangeState(_stateMachine.ComboAttackState);
            }
            else
            {
                _weapon.gameObject.GetComponentInChildren<BoxCollider>().enabled = false; // lgs 24.01.22
                if (toolItemDate.isTwoHandedTool == true)
                {
                    _stateMachine.ChangeState(_stateMachine.TwoHandedToolIdleState);
                }
                else if (toolItemDate.isTwinTool == true)
                {
                    _stateMachine.ChangeState(_stateMachine.TwinToolIdleState);
                }
                else
                {
                    _stateMachine.ChangeState(_stateMachine.IdleState);
                }
            }
        }
    }

}
