using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

// 2024. 02. 24 Byun Jeongmin
public class ViewPointRotation : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 0.04f;
    [SerializeField] private Transform _virtualCamera;

    private void Start()
    {
        FindVirtualCamera();

        UIBase.BindEvent(gameObject, HandleDrag, UIBase.UIEvents.Drag);
    }

    private void FindVirtualCamera()
    {
        var virtualCameraObject = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCameraObject != null)
        {
            _virtualCamera = virtualCameraObject.transform;
        }
    }

    private void HandleDrag(PointerEventData eventData)
    {
        if (_virtualCamera != null && eventData.button == PointerEventData.InputButton.Right)
        {
            float mouseX = eventData.delta.x;
            _virtualCamera.Rotate(Vector3.up, mouseX * _rotationSpeed, Space.World);

            // X축 회전 고정
            float currentXRotation = _virtualCamera.eulerAngles.x;
            _virtualCamera.rotation = Quaternion.Euler(currentXRotation, _virtualCamera.eulerAngles.y, 0f);
        }
    }
}
