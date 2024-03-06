using UnityEngine;
using System.Collections.Generic;

//2024-03-06 WJY
public class Vector3EqualityComparer : IEqualityComparer<Vector3>
{
    public bool Equals(Vector3 lhs, Vector3 rhs)
    {
        return lhs.Equals(rhs);
    }

    public int GetHashCode(Vector3 obj)
    {
        return obj.GetHashCode();
    }
}

public class Vector3IntEqualityComparer : IEqualityComparer<Vector3Int>
{
    public bool Equals(Vector3Int lhs, Vector3Int rhs)
    {
        return lhs.Equals(rhs);
    }

    public int GetHashCode(Vector3Int obj)
    {
        return obj.GetHashCode();
    }
}