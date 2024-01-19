using UnityEngine;
using System.Collections.Generic;

public class MapEditDataSource : MonoBehaviour
{
    public enum Inherits
    {
        Normal,
        Slide,
    }

    public enum Direction
    {
        Forward,
        Back,
        Up,
        Down,
        Left,
        Right,
    }

    [SerializeField] private Vector3Int _position = Vector3Int.zero;
    [SerializeField] private Vector3Int _size = Vector3Int.one;
    [SerializeField] private Direction _forward = Direction.Forward;
    [SerializeField] private Inherits _type;
    [SerializeField] private int _index;

    private void OnValidate()
    {
        transform.position = _position - new Vector3(0f, -0.5f, 0f);

        _size.x = Mathf.Abs(_size.x);
        _size.y = Mathf.Abs(_size.y);
        _size.z = Mathf.Abs(_size.z);
        _size.x = _size.x == 0 ? 1 : _size.x;
        _size.y = _size.y == 0 ? 1 : _size.y;
        _size.z = _size.z == 0 ? 1 : _size.z;

        transform.localScale = Vector3Int.one + (_size - Vector3Int.one) * 2;

        transform.forward = _forward switch
        {
            Direction.Forward => Vector3Int.forward,
            Direction.Back  => Vector3Int.back,
            Direction.Up => Vector3Int.up,
            Direction.Down => Vector3Int.down,
            Direction.Left => Vector3Int.left,
            Direction.Right => Vector3Int.right,
            _ => Vector3Int.forward
        };
    }

    public List<(Vector3Int pos, MapData data)> GetSourceData()
    {
        List<(Vector3Int pos, MapData data)> result = new();

        Vector3 forward = transform.forward;
        Vector3Int pos;
        for (int x = -_size.x + 1; x < _size.x; x++)
        {
            for (int y = -_size.y + 1; y < _size.y; y++)
            {
                for (int z = -_size.z + 1; z < _size.z; z++)
                {
                    if (_forward == Direction.Left || _forward == Direction.Right)
                        pos = new(z + _position.x, y + _position.y, x + _position.z);
                    else
                        pos = new(x + _position.x, y + _position.y, z + _position.z);

                    result.Add((pos, new(_type, _index, forward)));
                }
            }
        }

        return result;
    }
}


[System.Serializable]
public class MapData
{
    public MapEditDataSource.Inherits type;
    public int typeIndex;
    public Vector3 forward;

    public MapData() { }
    public MapData(MapEditDataSource.Inherits type, int typeIndex, Vector3 forward)
    {
        this.type = type;
        this.typeIndex = typeIndex;
        this.forward = forward;
    }
}