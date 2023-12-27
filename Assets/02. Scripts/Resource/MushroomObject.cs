using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomObject : MonoBehaviour , IInteractable
{
    public void Interact(Player player)
    {
        var inventory = player.GetComponentInChildren<InventorySystem>();
        var itemData = Resources.Load<ScriptableObject>("SO/MushroomItemData");
        inventory.AddItem(new ItemSlot((ItemData)itemData, 1));
        Destroy(gameObject);
    }
}
