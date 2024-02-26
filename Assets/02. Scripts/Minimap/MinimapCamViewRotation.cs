using UnityEngine;

// 2024. 02. 25 Byun Jeongmin
public class MinimapCamViewRotation : MonoBehaviour
{
    private Transform mainCameraTransform;

    private void Start()
    {
        GameObject mainCameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        mainCameraTransform = mainCameraObject.transform;
    }

    private void Update()
    {
        if (mainCameraTransform != null)
        {
            float mainCameraRotationY = mainCameraTransform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(90f, mainCameraRotationY, 0f);
        }
    }
}
