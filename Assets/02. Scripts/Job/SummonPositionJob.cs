using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct SummonPositionJob : IJob
{
    private NativeArray<Vector3> result;
    private Vector3 _forward;
    private Vector3 _axis;
    private float _distance;
    private float _angle;
    private int _count;

    public SummonPositionJob(Vector3 forward, Vector3 axis, float dist, float angle, int count)
    {
        result = new NativeArray<Vector3>(count, Allocator.TempJob);
        _forward = forward;
        _distance = dist;
        _angle = angle;
        _count = count;
        _axis = axis;
    }

    public void Execute()
    {
        Vector3 dir = _forward * _distance;

        var extraAngle = _angle * _count * 0.5f;
        for (int i = 0; i < result.Length; ++i)
        {
            result[i] = Quaternion.Euler(_axis * (_angle * (i+1) - extraAngle)) * dir;
        }
    }

    public Vector3[] GetResult()
    {
        var output = result.ToArray();
        result.Dispose();
        return output;
    }
}
