using UnityEngine;

[System.Serializable]
public class ItemLooting
{
    [SerializeField] private ItemData _item;
    [SerializeField] private AnimationCurve _distribution;

    public (ItemData item, int quantity) Looting(float weight = 0)
    {
        float value = Mathf.Clamp01(Random.value + weight);
        int quantity = Mathf.RoundToInt(_distribution.Evaluate(value));
        return (_item, quantity);
    }

    public void AddInventory(InventorySystem inventory, float weight = 0)
    {
        (var item, var quantity) = Looting(weight);
        if (item != null && quantity != 0)
            inventory.AddItem(item, quantity);
    }
}

[System.Serializable]
public class ItemDropTable
{
    [SerializeField] private ItemLooting[] _lootings;

    public void AddInventory(InventorySystem inventory, float weight = 0)
    {
        foreach (var loot in _lootings)
            loot.AddInventory(inventory, weight);
    }
}