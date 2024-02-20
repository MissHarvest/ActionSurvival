using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class JobTest : MonoBehaviour
{
    public int size = 10000;

    public List<bool> checks = new();
    public List<int> data = new();
    public List<int> result = new();

    private void Start()
    {
        for (int i = 0; i < size; i++)
        {
            checks.Add(i % 2 == 0);
            data.Add(i);
        }

        NativeArray<bool> c = new(checks.ToArray(), Allocator.TempJob);
        NativeArray<int> d = new(data.ToArray(), Allocator.TempJob);
        NativeList<int> r = new(0, Allocator.TempJob);
        Debug.Log(string.Join(", ", c));
        Debug.Log(string.Join(", ", d));
        Debug.Log(string.Join(", ", r.AsArray()));

        var job = new TestJob() { checks = c, data = d, result = r };
        var handle = job.Schedule();
        handle.Complete();

        var jobResult = job.GetResult();
        result = new(jobResult.AsArray());
        jobResult.Dispose();
    }
}

[BurstCompile]
public struct TestJob : IJob
{
    [ReadOnly] public NativeArray<bool> checks;
    [ReadOnly] public NativeArray<int> data;
    [WriteOnly] public NativeList<int> result;

    public void Execute()
    {
        for (int i = 0; i < checks.Length; i++)
        {
            if (checks[i])
                result.Add(data[i] + (int)VoxelLookUpTable.voxelVerts[0].x);
        }
    }

    public NativeList<int> GetResult()
    {
        checks.Dispose();
        data.Dispose();

        return result;
    }
}