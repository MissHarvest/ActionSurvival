using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSystem : MonoBehaviour
{
    public ItemSlot ItemInUse { get; private set; }
    public Transform handPosition;
    public GameObject ItemObject { get; private set; }

    private ItemSlot EmptyHand;

    private void Awake()
    {
        EmptyHand = new ItemSlot((ItemData)Resources.Load<ScriptableObject>("SO/EmptyHandItemData"));
    }

    private void Start()
    {
        if(handPosition == null)
        {
            Debug.LogWarning("handPosition is not allocated. Do Find [Hand R] / ToolSystem");
            this.enabled = false;
        }
        Managers.Game.Player.QuickSlot.OnUnRegisted += OnQuickSlotUpdated;
    }

    public void Equip(ItemSlot itemSlot)
    {
        UnEquip();

        ItemInUse = itemSlot;
        var prefabName = itemSlot.itemData.name.Replace("ItemData", "");
        ItemObject = Managers.Resource.Instantiate(prefabName, Literals.PATH_HANDABLE, handPosition);
    }

    public void UnEquip()
    {
        ItemInUse = EmptyHand;
        if (ItemObject != null)
        {
            Destroy(ItemObject);
        }        
    }

    public void ClearHand()
    {
        Equip(EmptyHand);
    }

    private void OnQuickSlotUpdated(QuickSlot slot)
    {
        if(ItemInUse == slot.itemSlot)
        {
            UnEquip();
        }
    }
}
