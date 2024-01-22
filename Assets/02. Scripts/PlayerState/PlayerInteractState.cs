using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractState : PlayerBaseState
{
    protected GameObject target;
    private bool hasInteracted = false; // 내구도 소모 확인용

    public PlayerInteractState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0;
        base.Enter();

        ToolItemData tool = _stateMachine.Player.EquippedItem.itemData as ToolItemData;

        var targets = Physics.OverlapSphere(_stateMachine.Player.transform.position, tool.range, tool.targetLayers);
        if (targets.Length != 0 && (targets[0].CompareTag(tool.targetTagName) || targets[0].CompareTag("Gather")))
        {
            target = targets[0].gameObject;
            Debug.Log($"target Name : {target.name}");
            StartAnimation(_stateMachine.Player.AnimationData.InteractParameterHash);

            // 내구도 감소 로직
            if (!hasInteracted)
            {
                int curIndex = Managers.Game.Player.QuickSlot.IndexInUse;
                int inventoryIndex = Managers.Game.Player.QuickSlot.slots[curIndex].targetIndex;

                string currentToolName = Managers.Game.Player.ToolSystem.GetToolName(Managers.Game.Player.ToolSystem.ItemInUse);

                // 빈손이 아닌 경우에만 내구도 감소
                if (currentToolName != "Handable_EmptyHand")
                {
                    float curToolDurability = Managers.Game.Player.Inventory.slots[inventoryIndex].currentDurability;

                    if (curToolDurability > 0)
                    {
                        curToolDurability--;
                        Debug.Log("현재 내구도 : " + curToolDurability);

                        // 내구도 업데이트
                        Managers.Game.Player.Inventory.UseToolItemByIndex(inventoryIndex, 1f);
                    }
                }

                hasInteracted = true;
            }
        }
        else
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(_stateMachine.Player.AnimationData.InteractParameterHash);

        int curIndex = Managers.Game.Player.QuickSlot.IndexInUse;
        int inventoryIndex = Managers.Game.Player.QuickSlot.slots[curIndex].targetIndex;

        string currentToolName = Managers.Game.Player.ToolSystem.GetToolName(Managers.Game.Player.ToolSystem.ItemInUse);

        // 빈손이 아닌 경우에만 내구도 감소
        if (currentToolName != "Handable_EmptyHand")
        {
            float curToolDurability = Managers.Game.Player.Inventory.slots[inventoryIndex].currentDurability;
            if (curToolDurability == 0)
            {
                // 장착 해제
                Managers.Game.Player.Inventory.FindItem(Managers.Game.Player.ToolSystem.ItemInUse.itemData, out int index);
                var item = new QuickSlot();
                item.Set(index, Managers.Game.Player.Inventory.slots[index]);
                Managers.Game.Player.QuickSlot.UnRegist(item);

                // 인벤토리에서 제거
                Managers.Game.Player.Inventory.DestroyItemByIndex(item);
            }
        }
        hasInteracted = false;
    }

    public override void Update()
    {
        // exit 조건 설정
        float normalizedTime = GetNormalizedTime(_stateMachine.Player.Animator, "Interact");

        if (normalizedTime >= 1f)
        {
            if (target != null)
                target.GetComponent<IInteractable>()?.Interact(_stateMachine.Player);
            _stateMachine.ChangeState(_stateMachine.IdleState);
        }
    }

    protected override void OnInteractStarted(InputAction.CallbackContext context)
    {
    }
}
