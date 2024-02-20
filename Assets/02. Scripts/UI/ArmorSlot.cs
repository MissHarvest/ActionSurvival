using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Lee gyuseong 24.02.15

public class ArmorSlot : MonoBehaviour
{
    //GetComponent 중복으로 사용이 짜친다. 바인딩을 하자.
    [SerializeField] private GameObject _armorSlotPrefab;
    [SerializeField] private Transform _position;

    private UIArmorSlot _armorSlot;
    private List<GameObject> _armorSlots = new List<GameObject>();

    private void Awake()
    {
        CreatArmorSlot();

        var armorItemSlots = Managers.Game.Player.ArmorSystem._equippedArmor;

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
