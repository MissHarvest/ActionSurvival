using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InventorySystem), true)]
public class InventoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        InventorySystem inventory = (InventorySystem)target;
        if (GUILayout.Button("Add Cheat Item"))
        {
            inventory.TestAddItem();
        }

        if (GUILayout.Button("Remove Cheat Item"))
        {
            inventory.TestRemoveItem();
        }

        if (GUILayout.Button("Clear Inventory"))
        {
            inventory.TestClearInventory();
        }

        if (GUILayout.Button("Save"))
        {
            inventory.TestSave();
        }
    }
}
