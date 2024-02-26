using UnityEngine;

// 2024. 01. 16 Byun Jeongmin
public class BonFire : MonoBehaviour, IInteractable
{
    [SerializeField] private int _cookingLevel = 0;
    public string clipName = "BonFire";

    private void Start()
    {
        Managers.Sound.PlayEffectSound(
            transform.position,
            clipName,
            0.7f,
            true);
    }

    public void Interact(Player player)
    {
        var ui = Managers.UI.ShowPopupUI<UICooking>();
        ui.SetAdvancedRecipeUIActive(_cookingLevel);
    }

    public float GetInteractTime()
    {
        return 0f;
    }
}
