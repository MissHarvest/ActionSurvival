using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct SummonPositionJob : IJob
{
    private NativeArray<Vector3> result;
    private Vector3 _origin;
    private Vector3 _forward;
    private float _distance;
    private float _angle;
    private int _count;

    public SummonPositionJob(Vector3 origin, Vector3 forward, float dist, float angle, int count)
    {
        result = new NativeArray<Vector3>(count, Allocator.TempJob);
        _origin = origin;
        _forward = forward;
        _distance = dist;
        _angle = angle;
        _count = count;
    }

    public void Execute()
    {
        Vector3 dir = _origin + _forward * _distance;

        // 오리진에서
        // 각도만큼 떨어진 위치를 담아서
        // 각도만큼 정렬
        //
    }

    public Vector3[] GetResult()
    {
        var output = result.ToArray();
        result.Dispose();
        return output;
    }
}
