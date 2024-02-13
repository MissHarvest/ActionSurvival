using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITutorialArrow : UIPopup
{
    enum GameObjects
    {
        Arrow,
    }

    private Canvas _canvas;
    private GraphicRaycaster _graphicRaycaster;
    private PointerEventData _eventData;

    public GameObject[] temp;

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(GameObjects));
    }

    private void Awake()
    {
        Initialize();
        _canvas = GetComponent<Canvas>();
        _graphicRaycaster = GetComponent<GraphicRaycaster>();

        _eventData = new PointerEventData(null);

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Debug.Log("Arrow UI Activate");
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;
        Get<GameObject>((int)GameObjects.Arrow).transform.Rotate(Vector3.up, 1.0f);
    }

    public void ActivateArrow(Vector3 pos)
    {
        var arrow = Get<GameObject>((int)GameObjects.Arrow);
        arrow.transform.position = new Vector3(pos.x, pos.y, arrow.transform.position.z);
    }

    public void ActivateArrow(Vector3 pos, Vector2 offset)
    {
        var arrow = Get<GameObject>((int)GameObjects.Arrow);
        arrow.transform.position = pos;
        Debug.Log($"[Position] {pos} , [Offset] {offset}");
        arrow.GetComponent<RectTransform>().anchoredPosition += offset;
    }
}
