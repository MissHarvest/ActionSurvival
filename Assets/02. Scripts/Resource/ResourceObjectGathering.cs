using UnityEngine;

public class ResourceObjectGathering : ResourceObjectBase, IInteractable
{
    [SerializeField] private ItemData _lootingItem;

    public void Interact(Player player)
    {
        Managers.Game.Player.Inventory.AddItem(_lootingItem, 1);
        _parent.SwitchObject(_toObjectID);
    }

    public override void Initialize()
    {
        base.Initialize();
    }
}
