using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotSystem : MonoBehaviour
{
    public static int capacity = 4;
    private ItemSlot[] slots = new ItemSlot[capacity];

    private void Awake()
    {
        var emptyHandData = Managers.Resource.GetScriptableObject("EmptyHand") as ItemData;
        for(int i = 0; i < slots.Length; ++i)
        {
            slots[i] = new ItemSlot(emptyHandData, 1);
        }
    }

    public void Registe()
    {

    }

    public void UnRegiste()
    {

    }

    public void Use(int index) // Change , Consume
    {

    }
}
