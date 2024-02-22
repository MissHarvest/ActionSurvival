using System.Linq;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

// 2024. 01. 25 Byun Jeongmin
public class PlayerDestroyState : PlayerGroundedState
{
    protected GameObject target;
    protected string targetTag;

    public PlayerDestroyState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0;
        base.Enter();
        StartAnimation(_stateMachine.Player.AnimationData.InteractParameterHash);

        ToolItemData hammer = _stateMachine.Player.EquippedItem.itemSlot.itemData as ToolItemData;

        var targets = Physics.OverlapSphere(_stateMachine.Player.transform.position, hammer.range, hammer.targetLayers);
        if (targets.Length == 0)
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
            return;
        }

        target = targets[0].gameObject;
        targetTag = target.tag;
        
        RotateOfTarget();
        _stateMachine.Player.Animator.SetBool(targetTag, true);
        return;
    }

    public override void Exit()
    {
        base.Exit();
        if (target != null)
        {
            _stateMachine.Player.Animator.SetBool(targetTag, false);
            target = null;
        }
        StopAnimation(_stateMachine.Player.AnimationData.InteractParameterHash);
    }

    public override void Update()
    {
        // exit 조건 설정
        float normalizedTime = GetNormalizedTime(_stateMachine.Player.Animator, "Interact");

        if (normalizedTime >= 3f)
        {
            if (target != null)
            {
                BuildableObject buildableObject = target.GetComponent<BuildableObject>();
                if (buildableObject != null)
                {
                    // "Architecture_아이템명-번호"에서 아이템명 문자열 추출
                    string itemName = ExtractItemName(buildableObject.name);
                    //Debug.Log("아이템명 : " + itemName);

                    RecipeSO recipeData = Managers.Data.GetRecipeDataByItemName(itemName);

                    if (recipeData != null)
                    {
                        ObtainRandomIngredients(recipeData);
                        buildableObject.DestroyObject();
                    }
                    else
                    {
                        Debug.LogError("아이템 재료 정보 없음");
                    }
                }
                _stateMachine.ChangeState(_stateMachine.IdleState);
            }
        }
    }

    //문자열 추출
    private string ExtractItemName(string fullName)
    {
        string itemNameWithNumber = fullName.Split('_')[1];
        string[] itemNameParts = itemNameWithNumber.Split('-');
        return itemNameParts[0];
    }

    // 재료 획득
    private void ObtainRandomIngredients(RecipeSO recipeData)
    {
        // buildableObject를 만드는데 드는 총 재료 개수
        int totalIngredients = recipeData.requiredItems.Sum(ingredient => ingredient.quantity);

        //총 재료 개수 / 4
        int maxIngredientsToObtain = Mathf.CeilToInt(totalIngredients / 4f);

        //재료 목록에서 중복을 허용하여 랜덤하게 획득
        var selectedIngredients = recipeData.requiredItems
            .OrderBy(ingredient => Random.value)
            .ToList();

        // 1부터 (총 재료 개수 / 4)개까지 재료 획득 가능 
        int totalQuantityToObtain = Random.Range(1, maxIngredientsToObtain + 1);

        for (int i = 0; i < selectedIngredients.Count; i++)
        {
            // 재료의 양을 고려해서 획득할 수 있는 최대 개수 설정
            int maxQuantityToObtain = Mathf.CeilToInt(selectedIngredients[i].quantity / 4f);

            //totalQuantityToObtain보다 maxQuantityToObtain가 큰 경우 방지
            int quantityToObtain = Mathf.Min(totalQuantityToObtain, maxQuantityToObtain);

            _stateMachine.Player.Inventory.AddItem_Before(selectedIngredients[i].item, quantityToObtain);
            Debug.Log($"재료 아이템명: {selectedIngredients[i].item.displayName}, 개수: {quantityToObtain}");

            totalQuantityToObtain -= quantityToObtain;
            if (totalQuantityToObtain <= 0)
                break;
        }
    }


    private void RotateOfTarget()
    {
        var look = target.transform.position - _stateMachine.Player.transform.position;
        look.y = 0;

        var targetRotation = Quaternion.LookRotation(look);
        _stateMachine.Player.transform.rotation = targetRotation;
    }
}
