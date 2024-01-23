using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public Player Player { get; }
    public PlayerIdleState IdleState { get; }
    public PlayerRunState RunState { get; }

    public PlayerTwoHandedToolIdleState TwoHandedToolIdleState { get; } // lgs
    
    public PlayerTwoHandedToolRunState TwoHandedToolRunState { get; }

    public PlayerTwinToolIdleState TwinToolIdleState { get; }
    
    public PlayerTwinToolRunState TwinToolRunState { get; }

    public PlayerInteractState InteractState { get; }
    public PlayerBuildState BuildState { get; }
    public PlayerMakeState MakeState { get; }
        
    public PlayerComboAttackState ComboAttackState { get; }

    public Vector2 MovementInput { get; set; }
    public float MovementSpeed { get; private set; }
    public float RotationDamping { get; private set; }
    public float MovementSpeedModifier { get; set; } = 1f;
    public bool IsAttacking { get; set; }
    public int ComboIndex { get; set; }


    public float JumpForce { get; set; }
    
    public Transform MainCameraTransform { get; set; }

    public PlayerStateMachine(Player player)
    {
        this.Player = player;

        IdleState = new PlayerIdleState(this);
        RunState = new PlayerRunState(this);

        TwoHandedToolIdleState = new PlayerTwoHandedToolIdleState(this); // lgs        
        TwoHandedToolRunState = new PlayerTwoHandedToolRunState(this);

        TwinToolIdleState = new PlayerTwinToolIdleState(this);
        TwinToolRunState = new PlayerTwinToolRunState(this);

        InteractState = new PlayerInteractState(this);
        BuildState = new PlayerBuildState(this);
        MakeState = new PlayerMakeState(this);
        ComboAttackState = new PlayerComboAttackState(this);

        MainCameraTransform = Camera.main.transform;

        MovementSpeed = player.Data.GroundedData.BaseSpeed;
        RotationDamping = player.Data.GroundedData.BaseRotationDamping;
    }
}
