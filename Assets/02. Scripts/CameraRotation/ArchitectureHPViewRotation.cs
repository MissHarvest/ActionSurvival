using UnityEngine;

// 2024. 02. 26 Byun Jeongmin
public class ArchitectureHPViewRotation : MonoBehaviour
{
    private void OnEnable()
    {
        transform.LookAt(Camera.main.transform);
    }
}
