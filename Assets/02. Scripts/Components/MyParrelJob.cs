using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct MyParrelJob : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<Vector3Int> a;
    
    public NativeArray<ChunkCoord> result;

    public int voxelSizeX;
    public int voxelSizeZ;


    public void Execute(int index)
    {
        ChunkCoord res = a[index];
        res.x /= voxelSizeX;
        res.z /= voxelSizeZ;
        result[index] = res;
    }
}
