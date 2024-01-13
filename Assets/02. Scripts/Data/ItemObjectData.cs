using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemObjectData : MonoBehaviour
{
    // lg
    public ToolItemData itemData;
    public bool onEquipTwoHandedTool = false;

    public void OnEquipTwoHandedTool()
    {
        // ���⼭ ĳ���Ͱ� ���� ������ �˻��ϸ� ���� �� ������..
        if (itemData.isTwoHandedTool == true)
        {
            Debug.Log("�� �� ������ �����Ͽ����ϴ�.");
            onEquipTwoHandedTool = true;
        }
        else if (itemData.isTwoHandedTool == false)
        {
            Debug.Log("�� �� ������ �����Ͽ����ϴ�.");            
        }
    }
    public void OnUnEquipTwoHandedTool()
    {
        if (itemData.isTwoHandedTool == true)
        {
            Debug.Log("�� �� ������ �����Ͽ����ϴ�.");
            onEquipTwoHandedTool = false;
        }
        else if (itemData.isTwoHandedTool == false)
        {
            Debug.Log("�� �� ������ �����Ͽ����ϴ�.");
        }
    }
}
