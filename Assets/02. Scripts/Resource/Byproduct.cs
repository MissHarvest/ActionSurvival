using UnityEngine;

// 2024-02-13 WJY
public class Byproduct : MonoBehaviour
{
    private ByproductCreator _parent;

    public void SetInfo(ByproductCreator parent)
    {
        _parent = parent;
    }

    private void OnDestroy()
    {
        _parent.Remove(this);
    }
}