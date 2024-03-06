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

        var sensitivity = SaveGame.GetSensitivitySetting();
        SetRotationSpeed(sensitivity);
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
        if (_virtualCamera != null && eventData.button == PointerEventData.InputButton.Left)
        {
            RotateCamAngle(eventData.delta.x);
        }
    }

    private void RotateCamAngle(float mouseX)
    {
        _virtualCamera.Rotate(Vector3.up, mouseX * _rotationSpeed, Space.World);

        // X축 회전 고정
        float currentXRotation = _virtualCamera.eulerAngles.x;
        _virtualCamera.rotation = Quaternion.Euler(currentXRotation, _virtualCamera.eulerAngles.y, 0f);
    }

    public void SetRotationSpeed(float newRotationSpeed)
    {
        _rotationSpeed = newRotationSpeed;
        PlayerPrefs.SetFloat($"Sensitivity", _rotationSpeed);
    }
}
