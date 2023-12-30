using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlot
{
    public ItemSlot itemSlot = new ItemSlot();
    public int targetIndex = -1;

    public QuickSlot()
    {
        // EmptyHand ItemSlot ���� �� ���� �����ϱ� ����
    }

    public void Set(int target, ItemSlot itemSlot)
    {
        this.targetIndex = target;
        this.itemSlot = itemSlot;
    }

    public void Clear()
    {
        itemSlot = new ItemSlot();
        this.targetIndex = -1;
    }
}
