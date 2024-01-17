using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerAnimationData
{
    [SerializeField] private string groundParameterName = "@Ground";
    [SerializeField] private string idleParameterName = "Idle";
    [SerializeField] private string walkParameterName = "Walk";
    [SerializeField] private string runParameterName = "Run";

    [SerializeField] private string equipTwoHandedToolIdleParameterName = "EquipTwoHandedToolIdle";
    [SerializeField] private string equipTwoHandedToolWalkParameterName = "EquipTwoHandedToolWalk";
    [SerializeField] private string equipTwoHandedToolRunParameterName = "EquipTwoHandedToolRun";

    [SerializeField] private string equipTwinToolIdleParameterName = "EquipTwinToolIdle";
    [SerializeField] private string equipTwinToolWalkParameterName = "EquipTwinToolWalk";
    [SerializeField] private string equipTwinToolRunParameterName = "EquipTwinToolRun";

    [SerializeField] private string airParameterName = "@Air";
    [SerializeField] private string jumpParameterName = "Jump";
    [SerializeField] private string fallParameterName = "Fall";

    [SerializeField] private string attackParameterName = "@Attack";
    [SerializeField] private string comboAttackParameterName = "ComboAttack";
    [SerializeField] private string baseAttackParameterName = "BaseAttack";
    [SerializeField] private string interactParameterName = "Interact";

    public int GroundParameterHash { get; private set; }
    public int IdleParameterHash { get; private set; }
    public int WalkParameterHash { get; private set; }
    public int RunParameterHash { get; private set; }

    public int EquipTwoHandedToolIdleParameterHash { get; private set; }
    public int EquipTwoHandedToolWalkParameterHash { get; private set; }
    public int EquipTwoHandedToolRunParameterHash { get; private set; }

    public int EquipTwinToolIdleParameterHash {  get; private set; }
    public int EquipTwinToolWalkParameterHash { get; private set; }
    public int EquipTwinToolRunParameterHash { get; private set; }

    public int AirParameterHash { get; private set; }
    public int JumpParameterHash { get; private set; }
    public int fallParameterHash { get; private set; }

    public int AttackParameterHash { get; private set; }
    public int ComboAttackParameterHash { get; private set; }
    public int BaseAttackParameterHash { get; private set; }

    public int InteractParameterHash { get; private set; }

    public void Initialize()
    {
        GroundParameterHash = Animator.StringToHash(groundParameterName);
        IdleParameterHash = Animator.StringToHash(idleParameterName);
        WalkParameterHash = Animator.StringToHash(walkParameterName);
        RunParameterHash = Animator.StringToHash(runParameterName);

        EquipTwoHandedToolIdleParameterHash = Animator.StringToHash(equipTwoHandedToolIdleParameterName);
        EquipTwoHandedToolWalkParameterHash = Animator.StringToHash(equipTwoHandedToolWalkParameterName);
        EquipTwoHandedToolRunParameterHash = Animator.StringToHash(equipTwoHandedToolRunParameterName);

        EquipTwinToolIdleParameterHash = Animator.StringToHash(equipTwinToolIdleParameterName);
        EquipTwinToolWalkParameterHash = Animator.StringToHash(equipTwinToolWalkParameterName);
        EquipTwinToolRunParameterHash = Animator.StringToHash(equipTwinToolRunParameterName);

        AirParameterHash = Animator.StringToHash(airParameterName);
        JumpParameterHash = Animator.StringToHash(jumpParameterName);
        fallParameterHash = Animator.StringToHash(fallParameterName);

        AttackParameterHash = Animator.StringToHash(attackParameterName);
        ComboAttackParameterHash = Animator.StringToHash(comboAttackParameterName);
        
        BaseAttackParameterHash = Animator.StringToHash(comboAttackParameterName);

        InteractParameterHash = Animator.StringToHash(interactParameterName);
    }
}