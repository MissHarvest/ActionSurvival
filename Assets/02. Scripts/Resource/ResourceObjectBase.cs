using UnityEngine;

public abstract class ResourceObjectBase : MonoBehaviour
{
    protected ResourceObjectParent _parent;
    [SerializeField] protected int _toObjectID;

    public ResourceObjectParent Parent => GetParent();

    public ResourceObjectParent GetParent()
    {
        if (_parent == null)
            _parent = GetComponentInParent<ResourceObjectParent>();
        return _parent;
    }

    public virtual void Initialize()
    {
        GetParent();
    }
}
