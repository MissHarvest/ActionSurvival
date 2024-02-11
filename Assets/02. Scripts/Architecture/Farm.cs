using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FarmState
{
    Idle,
    Grow,
    Harvest,
}

public class Farm : MonoBehaviour, IInteractable
{
    private FarmStateMachine _stateMachine;
    public GameObject[] stateObject;

    public int MaxTime { get; } = 1;
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

    public void Interact(Player player) // 인벤토리에 씨앗 확인
    {
        if (stateObject[0].activeSelf == true)
        {
            if (SeedsInInventory())
            {
                _stateMachine.Interact(player);
            }
        }
        else if (stateObject[2].activeSelf == true)
        {
            _stateMachine.Interact(player);
        }
        
    }

    public void ChangeObject(int state)
    {
        for (int i = 0; i < 3; ++i)
        {
            stateObject[i].SetActive(i == state);
        }
    }

    private bool SeedsInInventory()
    {
        for (int i = 0; i < Managers.Game.Player.Inventory.slots.Length; i++)
        {
            if (Managers.Game.Player.Inventory.slots[i].itemData != null && Managers.Game.Player.Inventory.slots[i].itemData.name == "SeedItemData")
            {
                Managers.Game.Player.Inventory.RemoveItem(Managers.Game.Player.Inventory.slots[i].itemData, 1);
                return true;
            }
        }
        return false;
    }
}
