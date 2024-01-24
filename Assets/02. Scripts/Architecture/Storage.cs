using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 24 Park jun Uk
public class Storage : MonoBehaviour, IInteractable
{
    public InventorySystem InventorySystem { get; private set; }
    public int maxCapacity = 20;
    private void Awake()
    {
        InventorySystem = GetComponent<InventorySystem>();
        InventorySystem.SetCapacity(maxCapacity);
    }

    public void Interact(Player player)
    {
        var ui = Managers.UI.ShowPopupUI<UIStorage>();
        ui.SetStorage(this);
    }
}
