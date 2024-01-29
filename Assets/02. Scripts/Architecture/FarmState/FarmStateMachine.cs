using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class FarmStateMachine : StateMachine
{
    public Farm Farm {get;}
    public FarmEmptyState EmptyState { get; }
    public FarmGrowState GrowState { get; }
    public FarmHarvestState HarvestState { get; }

    private FarmBaseState[] _farmStates = new FarmBaseState[3];

    public FarmStateMachine(Farm farm)
    {
        this.Farm = farm;

        EmptyState = new FarmEmptyState(this);
        _farmStates[0] = EmptyState;

        GrowState = new FarmGrowState(this);
        _farmStates[1] = GrowState;

        HarvestState = new FarmHarvestState(this);
        _farmStates[2] = HarvestState;
    }

    public void Interact(Player player)
    {
        (currentState as FarmBaseState).Interact(player);
    }

    public void ChangeState(FarmState state)
    {
        ChangeState(_farmStates[(int)state]);
    }
}
