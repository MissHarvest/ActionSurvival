using UnityEngine;

// 2024. 02. 26 Byun Jeongmin
public class ArchitectureHPViewRotation : MonoBehaviour
{
    private void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
