using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuickSlot
{
    public ItemSlot itemSlot = new ItemSlot();
    public int targetIndex = -1;

    public void Set(int target, ItemSlot itemSlot)
    {
        this.targetIndex = target;
        this.itemSlot.Copy(itemSlot);
    }

    public void Clear()
    {
        itemSlot.Clear();
        this.targetIndex = -1;
    }
}
