using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct RandomInRangeJob : IJob
{
    private Vector3 _center;
    private NativeArray<Vector3> _result;
    private float _range;
    
    public RandomInRangeJob(Vector3 center, float range, int count)
    {
        _result = new NativeArray<Vector3>(count, Allocator.TempJob);
        _center = center;
        _range = range;
    }

    public void Execute()
    {
        for(int i = 0; i < _result.Length; ++i)
        {
            
        }
    }

    public Vector3[] GetResult()
    {
        var output = _result.ToArray();
        _result.Dispose();
        return output;
    }
}
