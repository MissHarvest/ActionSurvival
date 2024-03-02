using UnityEngine;

public class FrameCounter : MonoBehaviour
{
    private float _deltaTime = 0.0f;

    private int _size = 25;

    private void Update()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();

        //Rect rect = new Rect(30, 30, Screen.width, Screen.height);
        Rect rect = new Rect(0, Screen.height - _size - 10, Screen.width, _size + 10);
        style.alignment = TextAnchor.MiddleLeft;
        style.fontSize = _size;
        style.normal.textColor = Color.black;

        float ms = _deltaTime * 1000.0f;
        float fps = 1.0f / _deltaTime;
        string text = string.Format("{0:0.} FPS ({1:0.0} ms)", fps, ms);
        GUI.Label(rect, text, style);
    }
}
