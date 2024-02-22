using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct LockBoundaryJob : IJob
{
    private NativeArray<bool> _points;
    private NativeArray<int> _result;

    private int _boundary;
    private int _field;
    private int _arraySize;
    private int _centerRadius;

    public LockBoundaryJob(bool[,] points, int field, int boundary, int centerRadius)
    {
        this._boundary = boundary;
        this._field = field;
        this._centerRadius = centerRadius;

        var row = points.GetLength(0);
        var col = points.GetLength(1);
        var length = row * col;
        _arraySize = row;

        _points = new NativeArray<bool>(length, Allocator.TempJob);
        for(int i = 0; i < length; ++i)
        {
            var x = i / _arraySize;
            var z = i % _arraySize;
            _points[i] = points[x, z];
        }

        _result = new NativeArray<int>(1, Allocator.TempJob);
    }

    public void Execute()
    {
        LockBoundary();
        LockCenter();
        _result[0] = _field;
    }

    private void LockBoundary()
    {
        int boundary = _boundary;
        int field = _field;

        Vector2[] directions = { new Vector2(-1, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1) };

        while(boundary > 0)
        {
            if (TryGetBoundaryStartPoint(out Vector2 start) == false) return;
            var current = start;
            bool stop = false;
            int forward = 0;

            while (!stop)
            {
                _points[(int)(current.x * _arraySize + current.y)] = true;
                --field;

                var directionIndexs = AddForwardSideIndex(forward);

                for (int i = 0; i < directionIndexs.Length; ++i)
                {
                    var next = current + directions[directionIndexs[i]];

                    if (next.x < 0 || next.x >= _arraySize || next.y < 0 || next.y >= _arraySize) continue;

                    if (_points[(int)(next.x * _arraySize + next.y)] == false)
                    {
                        current = next;
                        forward = directionIndexs[i];
                        break;
                    }
                    else
                    {
                        if (start == current + directions[directionIndexs[i]])
                        {
                            stop = true;
                            break;
                        }
                    }
                }
            }

            --boundary;
        }
        _field = field;
    }

    private bool TryGetBoundaryStartPoint(out Vector2 point)
    {
        for(int i = 0; i < _points.Length; ++i)
        {
            if (_points[i] == false)
            {
                point = new Vector2(i / _arraySize, i % _arraySize);
                return true;
            }
        }
        point = Vector2.zero;
        return false;
    }

    private int[] AddForwardSideIndex(int forward)
    {
        int[] result = new int[3];
        for(int i = 0; i < 3; ++i)
        {
            result[i] = (forward + 3 + i) % 4;
        }
        return result;
    }

    private void LockCenter()
    {
        var d = _arraySize / 2;

        for (int x = d - _centerRadius; x <= d + _centerRadius; ++x)
        {
            for (int z = d - _centerRadius; z <= d + _centerRadius; ++z)
            {
                if (_points[x * _arraySize + z] == false)
                {
                    _points[x * _arraySize + z] = true;
                    --_field;
                }
            }
        }
    }

    public (bool[,] points, int field) GetResult()
    {
        bool[,] temp = new bool[_arraySize, _arraySize];
        for(int i = 0; i < _arraySize; ++i)
        {
            for (int j = 0; j < _arraySize; ++j)
            {
                temp[i, j] = _points[_arraySize * i + j];
            }
        }
        var result = (temp, _result[0]);
        _points.Dispose();
        return result;
    }
}
