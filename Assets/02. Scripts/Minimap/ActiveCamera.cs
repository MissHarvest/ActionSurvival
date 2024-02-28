using UnityEngine;

// 2024. 02. 28 Byun Jeongmin
public class ActiveCamera : MonoBehaviour
{
    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        var ui = Managers.UI.GetPopupUI<UIMinimap>();

        if (ui != null && ui.gameObject.activeSelf)
            _camera.enabled = true;
        else
            _camera.enabled = false;
    }
}
