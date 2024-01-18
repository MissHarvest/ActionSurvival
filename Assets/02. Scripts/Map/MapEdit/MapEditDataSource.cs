using UnityEngine;

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

    [SerializeField] private WorldData data;
    [SerializeField] private Vector3Int position = Vector3Int.zero;
    [SerializeField] private Vector3Int size = Vector3Int.one;
    [SerializeField] private Direction forward = Direction.Forward;
    [SerializeField] private Inherits type;
    [SerializeField] private int index;
    public BlockType BlockType;

    private void OnValidate()
    {
        transform.position = position - new Vector3(0f, -0.5f, 0f);
        transform.localScale = size;
        switch (forward)
        {
            case Direction.Forward: transform.forward = Vector3Int.forward; break;
            case Direction.Back: transform.forward = Vector3Int.down; break;
            case Direction.Up: transform.forward = Vector3Int.up; break;
            case Direction.Down: transform.forward = Vector3Int.down; break;
            case Direction.Left: transform.forward = Vector3Int.left; break;
            case Direction.Right: transform.forward = Vector3Int.right; break;
        }
    }
}