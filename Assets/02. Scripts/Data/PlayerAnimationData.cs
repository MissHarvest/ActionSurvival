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

    [SerializeField] private string isEquipTwoHandedToolIdleParameterName = "IsEquipTwoHandedTool";
    [SerializeField] private string isEquipTwinToolIdleParameterName = "IsEquipTwinTool";
    [SerializeField] private string BlendEquipDefaultToolParameterName = "BlendEquipDefaultTool";
    [SerializeField] private string BlendEquipTwoHandedToolParameterName = "BlendEquipTwoHandedTool";
    [SerializeField] private string BlendEquipTwinToolParameterName = "BlendEquipTwinTool";

    [SerializeField] private string airParameterName = "@Air";
    [SerializeField] private string jumpParameterName = "Jump";
    [SerializeField] private string fallParameterName = "Fall";

    [SerializeField] private string attackParameterName = "@Attack";
    [SerializeField] private string comboAttackParameterName = "ComboAttack";
    [SerializeField] private string interactParameterName = "Interact";

    public int GroundParameterHash { get; private set; }
    public int IdleParameterHash { get; private set; }
    public int WalkParameterHash { get; private set; }
    public int RunParameterHash { get; private set; }

    public int EquipTwoHandedToolParameterHash { get; private set; }
    public int EquipTwinToolParameterHash { get; private set; }
    public int BlendEquipDefaultToolParameterHash { get; private set; }
    public int BlendEquipTwoHandedToolParameterHash { get; private set; }
    public int BlendEquipTwinToolParameterHash { get; private set; }

    public int AirParameterHash { get; private set; }
    public int JumpParameterHash { get; private set; }
    public int FallParameterHash { get; private set; }

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

        EquipTwoHandedToolParameterHash = Animator.StringToHash(isEquipTwoHandedToolIdleParameterName);
        EquipTwinToolParameterHash = Animator.StringToHash(isEquipTwinToolIdleParameterName);
        BlendEquipDefaultToolParameterHash = Animator.StringToHash(BlendEquipDefaultToolParameterName);
        BlendEquipTwoHandedToolParameterHash = Animator.StringToHash(BlendEquipTwoHandedToolParameterName);
        BlendEquipTwinToolParameterHash = Animator.StringToHash(BlendEquipTwinToolParameterName);

        AirParameterHash = Animator.StringToHash(airParameterName);
        JumpParameterHash = Animator.StringToHash(jumpParameterName);
        FallParameterHash = Animator.StringToHash(fallParameterName);

        AttackParameterHash = Animator.StringToHash(attackParameterName);
        ComboAttackParameterHash = Animator.StringToHash(comboAttackParameterName);
        
        BaseAttackParameterHash = Animator.StringToHash(comboAttackParameterName);

        InteractParameterHash = Animator.StringToHash(interactParameterName);
    }
}