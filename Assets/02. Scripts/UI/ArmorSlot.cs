using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Lee gyuseong 24.02.15

public class ArmorSlot : MonoBehaviour
{
    [SerializeField] private GameObject _armorSlotPrefab;
    [SerializeField] private Transform _position;

    private UIArmorSlot _armorSlot;
    private List<GameObject> _armorSlots = new List<GameObject>();

    private void Awake()
    {
        CreatArmorSlot();

        var armorItemSlots = GameManager.Instance.Player.ArmorSystem._linkedSlots;

        foreach (var armor in armorItemSlots)
        {
            for (int i = 0; i < _armorSlots.Count; i++)
            {
                var armorSlot = _armorSlots[i].GetComponent<UIArmorSlot>();
                armorSlot.EquipArmor(armor);
            }            
        }
    }

    private void CreatArmorSlot()
    {
        for (int i = 0; i < 2; i++)
        {
            var armorSlotPrefab = Instantiate(_armorSlotPrefab, _position);
            _armorSlot = GetComponentInChildren<UIArmorSlot>();
            SetParts(i);
            _armorSlots.Add(armorSlotPrefab);
        }
    }

    private void SetParts(int index)
    {  
        _armorSlot.part = (ItemParts)index;
    }
}
