
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager
{
    #region Member Variables

    // Sort Order
    private int _order = 10;
    
    // Popup Management
    private Stack<UIPopup> _activatedPopups = new Stack<UIPopup>();
    private Dictionary<string, GameObject> _popups = new Dictionary<string, GameObject>();

    // Scenes Overlay
    private UIScene _scene;
    #endregion



    #region Properties => Set Root UI

    public GameObject Root
    {
        get
        {
            var root = GameObject.Find("@UI_Root");
            
            if(root == null)
            {
                root = new GameObject { name = "@UI_Root" };
            }

            return root;
        }
    }

    #endregion



    public void LoadPopupUIs()
    {
        //var popups = Managers.Resource.GetPrefabs(Literals.PATH_POPUPUI);
        var popups = Managers.Resource.GetCacheGroup<GameObject>("UIPopUp_");
        for (int i = 0; i < popups.Length; ++i)
        {
            var obj = Object.Instantiate(popups[i], Root.transform);
            if (_popups.TryAdd(popups[i].name, obj))
            {
                Debug.Log($"{popups[i].name} is Added");
            }
        }
    }

    #region Scene UI

    public T ShowSceneUI<T>(string name = null) where T : UIScene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        //var gameObject = Managers.Resource.Instantiate(name, Literals.PATH_UI);
        var gameObject = Managers.Resource.GetCache<GameObject>($"{name}.prefab");
        gameObject = Object.Instantiate(gameObject);
        var sceneUI = Utility.GetOrAddComponent<T>(gameObject);
        
        gameObject.transform.SetParent(Root.transform);

        if (_scene != null)
            Object.Destroy(_scene.gameObject); // [WJY]: UILoadingScene -> UIMainScene 교체
        _scene = sceneUI;

        return sceneUI;
    }

    // [WJY]: 현재 씬 UI에 접근하기 위해 작성
    public bool TryGetSceneUI<T>(out T sceneUI) where T : UIScene
    {
        sceneUI = _scene as T;
        return sceneUI != null;
    }

    #endregion



    #region Popup UI

    public T ShowPopupUI<T>(string name = null) where T : UIPopup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        if (_popups.TryGetValue(name, out GameObject go))
        {
            var canvas = go.GetOrAddComponent<Canvas>();
            SetOrder(canvas);
            go.SetActive(true);
        }

        var popupUI = Utility.GetOrAddComponent<T>(go);
        _activatedPopups.Push(popupUI);

        Cursor.lockState = CursorLockMode.None;
        return popupUI;
    }

    public void ClosePopupUI(UIPopup popup)
    {
        if (_activatedPopups.Count == 0) return;

        if (_activatedPopups.Peek() != popup)
        {
            Debug.LogWarning("Close Popup failed");
            return;
        }
        
        ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        if (_activatedPopups.Count == 0) return;

        var popup = _activatedPopups.Pop();

        popup.gameObject.SetActive(false);

        _order -= 1;

        if (_order == 0)
            Cursor.lockState = CursorLockMode.Locked;
    }

    public void CloseAllPopupUI()
    {
        while(_activatedPopups.Count > 0)
            ClosePopupUI();
    }

    public bool Check(UIPopup popup)
    {
        return false;// _popups.Contains(popup);
    }

    #endregion



    #region Setup Canvas

    public void SetCanvas(GameObject go, bool sorting = true)
    {
        var canvas = Utility.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        var uiScales = Utility.GetOrAddComponent<CanvasScaler>(go);
        uiScales.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        var resolution = new Vector2(Literals.SCREEN_X, Literals.SCREEN_Y);
        uiScales.referenceResolution = resolution;
        uiScales.matchWidthOrHeight = Literals.MATCH_WIDTH;

        if(sorting)
        {
            SetOrder(canvas);
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    private void SetOrder(Canvas canvas)
    {
        ++_order;
        canvas.sortingOrder = _order;
    }

    #endregion
}
