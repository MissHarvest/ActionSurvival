using UnityEngine;

// 2024. 01. 18 Byun Jeongmin
// 제작, 요리를 포함하는 기본 클래스
public class CraftBase : MonoBehaviour
{
    public virtual void Awake()
    {
        Debug.Log($"{GetType().Name} Awake");
    }

    public virtual void Start()
    {
        Debug.Log($"{GetType().Name} Start");
    }
}