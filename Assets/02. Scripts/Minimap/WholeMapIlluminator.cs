using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 2024. 02. 20 Byun Jeongmin
[Serializable]
public struct WholeVerticesArray
{
    [SerializeField]
    public Vector3[,] Verticearray;
}

[Serializable]
public struct WholeColorsArray
{
    [SerializeField]
    public Color[] Colorarray;
}

public class WholeMapIlluminator : MonoBehaviour
{
    private GameObject _wholeShadowPlanePrefab;
    private GameObject _wholeShadowPlane;
    private Transform _player;
    private Mesh _wholeMesh;
    private LayerMask _shadowLayer;
    [SerializeField] private WholeVerticesArray _verticesArray;
    [SerializeField] private WholeColorsArray _colorsArray;

    private Queue<Vector3> _queue = new Queue<Vector3>();
    private HashSet<Vector3> _visited = new HashSet<Vector3>();

    private float _shadowRadius = 350f;

    private float _radiusCircle { get { return _shadowRadius; } }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _wholeShadowPlanePrefab = Managers.Resource.GetCache<GameObject>("WholeShadowPlane.prefab");
        _wholeShadowPlane = Instantiate(_wholeShadowPlanePrefab, new Vector3(0, 40, 0), Quaternion.Euler(90, 0, 0));
        _wholeMesh = _wholeShadowPlane.GetComponent<MeshFilter>().mesh;
        _shadowLayer = LayerMask.GetMask("ShadowLayer");

        SortVertices();
        _verticesArray = new WholeVerticesArray { Verticearray = SortVertices() };
        _colorsArray = new WholeColorsArray { Colorarray = new Color[_verticesArray.Verticearray.GetLength(0) * _verticesArray.Verticearray.GetLength(1)] };

        for (int k = 0; k < _colorsArray.Colorarray.Length; k++)
        {
            _colorsArray.Colorarray[k] = Color.black;
        }
    }

    private Vector3[,] SortVertices()
    {
        int rowCount = Mathf.CeilToInt(_wholeMesh.bounds.size.x); //22
        int colCount = Mathf.CeilToInt(_wholeMesh.bounds.size.y); //6

        Vector3[,] sortedVertices = new Vector3[rowCount, colCount];

        // x축 기준 정렬
        Vector3[] sortedX = _wholeMesh.vertices.OrderBy(v => v.x).ToArray();

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < colCount; j++)
            {
                int index = i * colCount + j;
                if (index < sortedX.Length)
                {
                    sortedVertices[i, j] = sortedX[index];
                }
            }
        }

        // z축 기준 정렬
        for (int j = 0; j < colCount; j++)
        {
            List<Vector3> colVertices = new List<Vector3>();
            for (int i = 0; i < rowCount; i++)
            {
                colVertices.Add(sortedVertices[i, j]);
            }
            colVertices = colVertices.OrderBy(v => v.z).ToList();

            for (int i = 0; i < rowCount; i++)
            {
                sortedVertices[i, j] = colVertices[i];
            }
        }

        return sortedVertices;
    }

    private void Start()
    {
        _player = GameManager.Instance.Player.transform;
        Load();
        GameManager.Instance.OnSaveCallback += Save;
    }

    private void Update()
    {
        Ray ray = new Ray(_player.position, Vector3.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, _shadowLayer, QueryTriggerInteraction.Collide))
        {
            BFS(hit.point);
        }
    }

    private void BFS(Vector3 startPoint)
    {
        int rowCount = _verticesArray.Verticearray.GetLength(0);
        int colCount = _verticesArray.Verticearray.GetLength(1);

        int startRow = Mathf.FloorToInt((startPoint.x / _wholeMesh.bounds.size.x) * rowCount);
        int startCol = Mathf.FloorToInt((startPoint.z / _wholeMesh.bounds.size.y) * colCount);

        _queue.Enqueue(_verticesArray.Verticearray[startRow, startCol]);
        _visited.Add(_verticesArray.Verticearray[startRow, startCol]);

        while (_queue.Count > 0)
        {
            Vector3 currentPoint = _queue.Dequeue();

            UpdateAlpha(currentPoint);

            // 인접 정점 큐에 추가
            AddNeighborsToQueue(_queue, _visited, currentPoint, rowCount, colCount);
        }

        UpdateColors();
    }

    private void AddNeighborsToQueue(Queue<Vector3> queue, HashSet<Vector3> visited, Vector3 point, int rowCount, int colCount)
    {
        int row = -1;
        int col = -1;

        // 정렬된 정점배열에서 현재 포인트의 위치 찾기
        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < colCount; j++)
            {
                if (_verticesArray.Verticearray[i, j] == point)
                {
                    row = i;
                    col = j;
                    break;
                }
            }
            if (row != -1) break;
        }

        // 인접 정점 큐에 추가
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int newRow = row + i;
                int newCol = col + j;

                if (newRow >= 0 && newRow < rowCount && newCol >= 0 && newCol < colCount)
                {
                    Vector3 neighbor = _verticesArray.Verticearray[newRow, newCol];

                    // 정점이 유효하고, hitPoint로부터의 거리가 radiusCircle보다 작으면 큐에 추가
                    if (IsValidPoint(neighbor))
                    {
                        float distance = Vector3.Distance(neighbor, point);
                        if (distance < _radiusCircle && !visited.Contains(neighbor))
                        {
                            queue.Enqueue(neighbor);
                            visited.Add(neighbor);
                        }
                    }
                }
            }
        }
    }

    private bool IsValidPoint(Vector3 point)
    {
        return point.x >= 0 && point.x < _wholeMesh.bounds.size.x &&
               point.z >= 0 && point.z < _wholeMesh.bounds.size.y;
    }

    private void UpdateAlpha(Vector3 hitPoint)
    {
        // 알파값 업데이트하는 기존 코드
        for (int i = 0; i < _verticesArray.Verticearray.GetLength(0); i++)
        {
            for (int j = 0; j < _verticesArray.Verticearray.GetLength(1); j++)
            {
                Vector3 vector3 = _wholeShadowPlane.transform.TransformPoint(_verticesArray.Verticearray[i, j]);
                var temp = vector3 - hitPoint;
                temp.y = 0;
                float distance = Vector3.SqrMagnitude(temp);

                if (distance < _radiusCircle * _radiusCircle)
                {
                    int index = i * _verticesArray.Verticearray.GetLength(1) + j;
                    float alpha = Mathf.Min(_colorsArray.Colorarray[index].a, distance / _radiusCircle);
                    _colorsArray.Colorarray[index].a = alpha;
                }
            }
        }
    }

    private void UpdateColors()
    {
        Color32[] colors32 = new Color32[_colorsArray.Colorarray.Length];
        for (int i = 0; i < colors32.Length; i++)
        {
            colors32[i] = _colorsArray.Colorarray[i];
        }
        _wholeMesh.colors32 = colors32;
    }

    public virtual void Load()
    {
        if (SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Runtime, "Minimap"))
        {
            UpdateColors();
        }
    }

    protected virtual void Save()
    {
        var json = JsonUtility.ToJson(this);
        SaveGame.CreateJsonFile("Minimap", json, SaveGame.SaveType.Runtime);
    }
}
