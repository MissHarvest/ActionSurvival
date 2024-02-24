using System.Collections;
using UnityEngine;

// 2024. 02. 24 Byun Jeongmin
public class ViewPointRotation : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 1f;
    [SerializeField] private RectTransform _rotationArea;
    private string _rotationAreaObjectName = "ViewRotationArea";

    private void Start()
    {
        StartCoroutine(WaitForUIMainScene());
    }

    private void Update()
    {
        if (_rotationArea != null && Input.GetMouseButton(1)) // 마우스 우클릭
        {
            float mouseX = Input.GetAxis("Mouse X");

            if (RectTransformUtility.RectangleContainsScreenPoint(_rotationArea, Input.mousePosition))
            {
                transform.Rotate(Vector3.up, mouseX * _rotationSpeed, Space.World);

                // X축 회전 고정
                float currentXRotation = transform.eulerAngles.x;
                transform.rotation = Quaternion.Euler(currentXRotation, transform.eulerAngles.y, 0f);
            }
        }
    }

    private IEnumerator WaitForUIMainScene()
    {
        while (true)
        {
            GameObject rotationAreaObject = GameObject.Find(_rotationAreaObjectName);

            if (rotationAreaObject != null)
            {
                _rotationArea = rotationAreaObject.GetComponent<RectTransform>();
            }
            yield return null;
        }

        //while (Managers.UI.TryGetSceneUI<UIMainScene>(out UIMainScene sceneUI))
        //{
        //    _rotationArea = sceneUI.transform.Find(_rotationAreaObjectName)?.GetComponent<RectTransform>();

        //    if (_rotationArea != null)
        //    {
        //        yield break;
        //    }

        //    yield return null;
        //}
    }
}