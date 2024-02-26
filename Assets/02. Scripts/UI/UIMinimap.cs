using TMPro;
using UnityEngine;

// 2024. 02. 07 Byun Jeongmin
public class UIMinimap : UIPopup
{
    enum GameObjects
    {
        Block,
    }

    enum Texts
    {
        CoordinatesText,
    }

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Get<GameObject>((int)GameObjects.Block).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });
    }

    private void Awake()
    {
        Initialize();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateCoordinatesText();
    }

    private void UpdateCoordinatesText()
    {
        Vector3 playerPosition = GameManager.Instance.Player.transform.position;
        Get<TextMeshProUGUI>((int)Texts.CoordinatesText).text = playerPosition.ToString("F0");
    }
}
