using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmBaseState : IState
{
    protected FarmStateMachine _stateMachine;

    public FarmBaseState(FarmStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }

    public virtual void TimeLapse()
    {

    }

    public virtual void HandleInput()
    {

    }

    public virtual void PhysicsUpdate()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void Interact(Player player)
    {

    }
}