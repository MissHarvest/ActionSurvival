using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct FixMonsterSpawnPointJob : IJob
{
    private NativeArray<bool> _points;
    private NativeArray<Vector3> _result;
    public NativeArray<int> whileCount;
    private int _field;
    private int _monsterCount;
    private int _monsterInterval;
    private int _arraySize;
    private int _hash;
    private int _interval;

    public FixMonsterSpawnPointJob(bool[,] points, int field, int monsterCount, int monsterinterval, int interval, int hash)
    {
        this._field = field;
        this._monsterCount = monsterCount;
        this._monsterInterval = monsterinterval;
        this._hash = hash;
        this._interval = interval;

        var row = points.GetLength(0);
        var col = points.GetLength(1);
        var length = row * col;
        _arraySize = row;

        _points = new NativeArray<bool>(length, Allocator.TempJob);
        for (int i = 0; i < length; ++i)
        {
            var x = i / _arraySize;
            var z = i % _arraySize;
            _points[i] = points[x, z];
        }

        _result = new NativeArray<Vector3>(monsterCount, Allocator.TempJob);
        whileCount = new NativeArray<int>(1, Allocator.TempJob);
    }

    public void Execute()
    {
        LoopCreateMonsterSpawnPoint();
    }

    private void LoopCreateMonsterSpawnPoint()
    {
        bool finished = false;
        uint count = 1;

        while (!finished)
        {
            finished = CreateMonsterSpawnPoint(count, out List<Vector2> list);
            ++count;
            if (count == 10000) break;
            if(finished)
            {
                Save(list);
            }
        }
        whileCount[0] = (int)count;
    }

    private void Save(List<Vector2> list)
    {
        for(int i= 0; i < list.Count; ++i)
        {
            _result[i] = new Vector3(list[i].x * _interval, 10, list[i].y * _interval);
        }
    }

    private bool CreateMonsterSpawnPoint(uint seed, out List<Vector2> list)
    {
        bool[] copyPoints = _points.ToArray();
        int cnt = _monsterCount;
        int field = _field;
        list = new();

        for(int i =0; i < copyPoints.Length; ++i)
        {
            if (copyPoints[i]) continue;

            if(CheckSpawnable(cnt, field, seed))
            {
                --cnt;
                list.Add(new Vector2(i / _arraySize, i % _arraySize));
                LockMonsterAroundPoint(copyPoints, i, ref field);
            }
            copyPoints[i] = true;
            --field;
            if (field <= 0) 
                break;
        }

        return cnt == 0;
    }
    
    private bool CheckSpawnable(int cnt, int field, uint seed)
    {
        var percent = (float)cnt / field;
        
        for (int i = 0; i < 2; ++i)
        {
            var rSeed = seed * (uint)field * (uint)cnt + (uint)i + (uint)_hash;
            var rnd = new Unity.Mathematics.Random(rSeed);
            var ran = rnd.NextFloat(0.0f, 1.0f);
            if (percent >= ran) return true;
        }
        return false;
    }

    private void LockMonsterAroundPoint(bool[] points, int index, ref int field)
    {
        var x = index / _arraySize;
        var z = index % _arraySize;

        var minX = Mathf.Max(0, x - _monsterInterval);
        var maxX = Mathf.Min(_arraySize -1, x + _monsterInterval);
        var minZ = Mathf.Max(0, z - _monsterInterval);
        var maxZ = Mathf.Min(_arraySize - 1, z + _monsterInterval);

        for (int i = minX; i <= maxX; ++i)
        {
            for (int j = minZ; j <= maxZ; ++j)
            {
                var abs = Mathf.Abs(i - x) + Mathf.Abs(j - z);
                if (_monsterInterval >= abs)
                {
                    if (points[i * _arraySize + j] == false) --field;
                    points[i * _arraySize + j] = true;
                }
            }
        }
    }

    public Vector3[] GetResult()
    {
        var result = _result.ToArray();
        _result.Dispose();
        _points.Dispose();
        return result;
    }
}
