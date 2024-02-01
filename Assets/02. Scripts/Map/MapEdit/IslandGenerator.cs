using UnityEngine;

// 2024-02-01 WJY
public class IslandGernerator : MonoBehaviour
{
    public GameObject cubePrefab;

    public int sizeX;
    public int sizeZ;

    public int heightMin;
    public int heightMax;

    public AnimationCurve curve;

    [Range(0.0001f, 10f)] public float scale;
    public float seed;

    [HideInInspector] public float[,] heightMap;

    private void Start()
    {
        Generate();
        TestCubeCreate();
    }

    public void Generate()
    {
        heightMap = new float[sizeX, sizeZ];

        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++) 
            {
                float curveValue = Mathf.Min(curve.Evaluate((float)x / sizeX), curve.Evaluate((float)z / sizeZ));
                Debug.Log(curveValue);
                float noise = Mathf.PerlinNoise((float)x / sizeX, (float)z / sizeZ) * scale * curveValue;
                heightMap[x, z] = Mathf.Lerp(heightMin, heightMax, noise);
                //Debug.Log(heightMap[x, z]);
            }
        }
    }

    public void TestCubeCreate()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                for (int y = 0; y < heightMap[x, z]; y++)
                {
                    Instantiate(cubePrefab, new Vector3(x, y, z), Quaternion.identity);
                }
            }
        }
    }
}