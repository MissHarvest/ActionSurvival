using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomObject : ResourceObject
{
    public override void Initialize()
    {
        InitLootingItem("MushroomItemData.data");
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
