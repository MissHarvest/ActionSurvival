using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIArmorSlot : MonoBehaviour
{
    [SerializeField] private GameObject _armorSlotPrefab;
    [SerializeField] private Transform _position;
    private List<GameObject> _armorSlots = new List<GameObject>();

    private ArmorSlot _armorSlot;

    private void Awake()
    {
        CreatArmorSlot();
    }

    private void CreatArmorSlot()
    {
        for (int i = 0; i < 2; i++)
        {
            var slots = Instantiate(_armorSlotPrefab, _position);
            _armorSlot = GetComponentInChildren<ArmorSlot>();
            SetParts(i);
            _armorSlots.Add(slots);
        }
    }

    private void SetParts(int index)
    {  
        _armorSlot.part = (ItemParts)index;
    }
}
