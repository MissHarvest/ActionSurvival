using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerComboAttackState : PlayerAttackState
{
    private bool _alreadyAppliedForce;
    private bool _alreadyApplyCombo;
    private bool _hit;
    //private Weapon _weapon;
    protected GameObject target;
    protected string targetTag;
    
    private Weapon _weapon;
    protected Vector3 _targetPosition;
    protected string _targetTag;

    private AttackInfoData _attackInfoData;

    public PlayerComboAttackState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {

    }    

    public override void Enter()
    {
        base.Enter();
        StartAnimation(_stateMachine.Player.AnimationData.ComboAttackParameterHash);
        //_weapon = _stateMachine.Player.GetComponentInChildren<Weapon>();

        _alreadyAppliedForce = false;
        _alreadyApplyCombo = false;
        _hit = false;

        int comboIndex = _stateMachine.ComboIndex;

        var weapon = _stateMachine.Player.EquippedItem.itemSlot.itemData as WeaponItemData;
        if(weapon == null)
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
            return;
        }

        _attackInfoData = weapon.WeaponAttackData.GetAttackInfo(comboIndex);
        _stateMachine.Player.Animator.SetInteger("Combo", comboIndex);

        var targets = Physics.OverlapSphere(_stateMachine.Player.transform.position, weapon.range * 1.5f, weapon.targetLayers);
        if (targets.Length == 0)
        {
            return;
        }

        if (targets[0].CompareTag(weapon.targetTagName))
        {
            target = targets[0].gameObject;
            targetTag = target.tag;
            RotateOfTarget();
            return;
        }
    }
        RotateOfTarget();
    }

    public override void Exit()
    {
        base.Exit();
        //_weapon.gameObject.GetComponentInChildren<BoxCollider>().enabled = false;
        //_weapon = null;
        target = null;
        _weapon.gameObject.GetComponentInChildren<BoxCollider>().enabled = false;
        _weapon = null;
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
        WeaponItemData weaponItem = _stateMachine.Player.ToolSystem.EquippedTool.itemSlot.itemData as WeaponItemData;
        if (weaponItem == null)
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
            return;
        }

        base.Update();

        ForceMove();

        float normalizedTime = GetNormalizedTime(_stateMachine.Player.Animator, "Attack");
        if (normalizedTime < 1f)
        {
            if (normalizedTime >= _attackInfoData.ForceTransitionTime)
                TryApplyForce();

            if (_attackInfoData.ForceTransitionTime < normalizedTime && normalizedTime <= _attackInfoData.ComboTransitionTime)
                _weapon.gameObject.GetComponentInChildren<BoxCollider>().enabled = true;

            if (normalizedTime >= _attackInfoData.ComboTransitionTime)
            {
                if(!_hit)
                {
                    _hit = true;
                    _stateMachine.Player.Attack(weaponItem.damage * _attackInfoData.Damage);
                }
            {
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
                //_weapon.gameObject.GetComponentInChildren<BoxCollider>().enabled = false;//
                ExitState(weaponItem);
            }
        }
    }

    private void ExitState(WeaponItemData weaponItem)
    {
        if (weaponItem.isTwoHandedTool == true)
        {
            _stateMachine.ChangeState(_stateMachine.TwoHandedToolIdleState);
        }
        else if (weaponItem.isTwinTool == true)
        {
            _stateMachine.ChangeState(_stateMachine.TwinToolIdleState);
                if (_alreadyApplyCombo)
                {
                    _stateMachine.ComboIndex = _attackInfoData.ComboStateIndex;
                    _stateMachine.InteractSystem.TryWeaponInteract();
                }
            }
        }
        else
        {
            _weapon.gameObject.GetComponentInChildren<BoxCollider>().enabled = false;
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
