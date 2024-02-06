using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FarmState
{
    Idle,
    Grow,
    Harvest,
}

public class Farm : MonoBehaviour ,IInteractable
{
    private FarmStateMachine _stateMachine;
    public GameObject[] stateObject;

    public int MaxTime { get; } = 100;
    [field: SerializeField] public int RemainingTime { get; set; } = 0;
    [SerializeField] public int State { get; set; } = 0;

    public ItemDropTable looting;

    private void Awake()
    {
        _stateMachine = new FarmStateMachine(this);
    }

    private void Start()
    {
        SetInfo(State, RemainingTime);
    }

    private void Update()
    {
        _stateMachine.Update();
    }

    public void SetInfo(int state, int time)
    {
        State = state;
        RemainingTime = time;
        _stateMachine.ChangeState((FarmState)state);
    }

    public void Interact(Player player)
    {
        _stateMachine.Interact(player);
    }

    public void ChangeObject(int state)
    {
        for(int i = 0; i < 3; ++i)
        {
            stateObject[i].SetActive(i == state);
        }
    }
}
