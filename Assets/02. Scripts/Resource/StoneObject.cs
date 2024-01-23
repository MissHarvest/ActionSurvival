using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneObject : ResourceObject
{
    public override void Initialize()
    {
        InitLootingItem("StoneItemData.data");
    }

    public override void Interact(Player player)
    {
        player.Inventory.AddItem(_lootingItem, 1);
        Destroy(gameObject);
    }

    public override void Respawn()
    {
    }
}
