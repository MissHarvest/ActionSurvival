using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSystem : MonoBehaviour
{
    public ItemSlot ItemInUse { get; private set; }
    public Transform handPosition;
    public GameObject ItemObject { get; private set; }

    // public GameObject[] Tools;
    private void Start()
    {
        if(handPosition == null)
        {
            Debug.LogWarning("handPosition is not allocated. Do Find [Hand R] / ToolSystem");
            this.enabled = false;
        }
    }

    public void Equip(ItemSlot itemSlot)
    {
        UnEquip();

        ItemInUse = itemSlot;
        var prefabName = itemSlot.itemData.name.Replace("ItemData", "");
        ItemObject = Managers.Resource.Instantiate(prefabName, Literals.PATH_HANDABLE, handPosition);
        // var go = 
        // go.get<class>().SetItemData();
    }

    public void UnEquip()
    {
        ItemInUse = null;
        if (ItemObject != null)
        {
            Destroy(ItemObject);
        }        
    }
}
