using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 12 Byun Jeongmin
public class CraftingTable : MonoBehaviour, IInteractable
{
    // Level
    public void Interact(Player player)
    {
        var ui = Managers.UI.ShowPopupUI<UIRecipe>();    
        // ui.Level 넘겨주기
    }
}

