using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UIItemUsageHelper : UIItemHelper
{
    public enum Functions
    {
        Regist,
        UnRegist,
        Use,
        Build,
        Equip,
        UnEquip,
        Destroy
    }

    private string[] _functions = new string[] { "등록하기", "해제하기", "사용하기", "건축하기", "장착하기", "해제하기", "버리기" };
    
    public override void Initialize()
    {
        base.Initialize();
    }

    protected override void CreateButtons()
    {
        CreateButtonByEnum(Functions.Regist);
        CreateButtonByEnum(Functions.UnRegist);
        CreateButtonByEnum(Functions.Use);
        CreateButtonByEnum(Functions.Build);
        CreateButtonByEnum(Functions.Equip);
        CreateButtonByEnum(Functions.UnEquip);
        CreateButtonByEnum(Functions.Destroy);
    }

    public override void ShowOption(ItemSlot selectedSlot, Vector3 position)
    {
        Clear();
        gameObject.SetActive(true);
        SetItemName(selectedSlot.itemData.displayName);

        Container.transform.position = position;
        
        switch (selectedSlot.itemData)
        {
            case ToolItemData _:
                ShowRegistButton(selectedSlot.registed);
                break;

            case ArchitectureItemData _: // ToolItemData이면서 건축 아이템인 경우
                ShowRegistButton(selectedSlot.registed);
                ShowButtonByEnum(Functions.Build);
                break;

            case FoodItemData _:
                ShowRegistButton(selectedSlot.registed);
                ShowButtonByEnum(Functions.Use);
                break;

            case EquipItemData:
                ShowEquipButton(selectedSlot.equipped);
                break;
        }
        ShowButtonByEnum(Functions.Destroy);

        // Set My Size
    }

    public void HighLight(Functions function)
    {
        var go = _buttons[function.ToString()].gameObject;
        var rect = go.GetComponent<RectTransform>();
        var xOffset = rect.sizeDelta.x * 0.5f;

        var arrowUI = Managers.UI.ShowPopupUI<UITutorialArrow>();
        var pos = go.transform.position;

        arrowUI.ActivateArrow(pos, new Vector2(xOffset, 0));
        go.BindEvent((x) =>
        {
            Managers.UI.ClosePopupUI(arrowUI);

            var evtHandler = Utility.GetOrAddComponent<UIEventHandler>(go);
            evtHandler.OnPointerDownEvent = null;
        }, UIEvents.PointerDown);
    }

    #region ShowButton

    private void ShowEquipButton(bool equip)
    {
        if (equip)
        {
            ShowButtonByEnum(Functions.UnEquip);
        }
        else
        {
            ShowButtonByEnum(Functions.Equip);
        }
    }

    private void ShowRegistButton(bool regist)
    {
        if(regist)
        {
            ShowButtonByEnum(Functions.UnRegist);            
        }
        else
        {
            ShowButtonByEnum(Functions.Regist);
        }
    }

    private void CreateButtonByEnum(Functions label)
    {
        CreateButton(label.ToString(), _functions[(int)label]);
    }

    private void ShowButtonByEnum(Functions function)
    {
        ShowButton(function.ToString());
    }

    public void BindEventOfButton(Functions functions, UnityAction action)
    {
        string name = functions.ToString();
        if(_buttons.TryGetValue(name, out GameObject button))
        {
            button.GetComponent<UIOptionButton>().Bind(action);
        }
    }

    #endregion
}
