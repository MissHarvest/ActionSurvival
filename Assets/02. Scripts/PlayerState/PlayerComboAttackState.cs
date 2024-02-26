using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerComboAttackState : PlayerAttackState
{
    private bool _alreadyAppliedForce;
    private bool _alreadyApplyCombo;
    private bool _hit;
    protected GameObject target;
    protected string targetTag;
    
    protected Vector3 _targetPosition;
    protected string _targetTag;

    private WeaponItemData _weaponData;
    private AttackInfoData _attackInfoData;

    public PlayerComboAttackState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {

    }    

    public override void Enter()
    {
        base.Enter();
        StartAnimation(_stateMachine.Player.AnimationData.ComboAttackParameterHash);
        _weaponData = _stateMachine.Player.EquippedItem.itemSlot.itemData as WeaponItemData;

        _alreadyAppliedForce = false;
        _alreadyApplyCombo = false;
        _hit = false;

        int comboIndex = _stateMachine.ComboIndex;
        _attackInfoData = _weaponData.WeaponAttackData.GetAttackInfo(comboIndex);
        _stateMachine.Player.Animator.SetInteger("Combo", comboIndex);

        RotateOfTarget();
    }

    public override void Exit()
    {
        base.Exit();
        target = null;
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

        _stateMachine.Player.ForceReceiver.Reset();

        _stateMachine.Player.ForceReceiver.AddForce(_stateMachine.Player.transform.forward * _attackInfoData.Force);
    }

    protected override void Rotate(Vector3 movementDirection)
    {
        // Player의 조이스틱 입력으로 인한 방향 전환을 배제한다.
    }

    private void RotateOfTarget()
    {
        if (_targetPosition.Equals(Vector3.zero))
            return;

        var look = _targetPosition - _stateMachine.Player.transform.position;
        look.y = 0;

        var targetRotation = Quaternion.LookRotation(look);

        _stateMachine.Player.transform.rotation = targetRotation;
    }

    public override void Update()
    {
        base.Update();

        ForceMove();

        float normalizedTime = GetNormalizedTime(_stateMachine.Player.Animator, "Attack");
        if (normalizedTime < 1f)
        {
            if (normalizedTime >= _attackInfoData.ForceTransitionTime)
                TryApplyForce();

            if (normalizedTime >= _attackInfoData.ComboTransitionTime)
            {
                if (!_hit)
                {
                    _hit = true;
                    _stateMachine.Player.Attack(_weaponData.damage * _attackInfoData.Damage);
                }
                TryComboAttack();
                if (_alreadyApplyCombo)
                {
                    _stateMachine.ComboIndex = _attackInfoData.ComboStateIndex;
                    _stateMachine.InteractSystem.TryWeaponInteract();
                }
            }
        }
        else
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
        }
    }

    protected override void OnInteractStarted(InputAction.CallbackContext context)
    {
        _stateMachine.IsAttacking = true;
    }

    protected override void OnQuickUseStarted(InputAction.CallbackContext context)
    {
        
    }

    public void SetTarget(Vector3 position)
    {
        _targetPosition = position;
    }
}