using UnityEngine;

public class DynamicProceduralCave : MonoBehaviour
{
    public Vector2Int mapSize = new Vector2Int(10, 10);
    [Range(0f, 1f)] public float cullingValue = 0f;
    public float cubeSize = 1f;

    private float[,] m_floatMaps;
    private float[,] m_cullingMaps;

    private void OnValidate()
    {
        GenerateCullingMap();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GenerateMap();
        }
    }

    private void GenerateMap()
    {
        GenerateFloatMap();
        GenerateCullingMap();
    }

    private void GenerateFloatMap()
    {
        m_floatMaps = new float[mapSize.x, mapSize.y];

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                m_floatMaps[x, y] = Random.Range(0f, 1f);
            }
        }
    }

    private void GenerateCullingMap()
    {
        if(m_floatMaps == null)
        {
            return;
        }

        m_cullingMaps = new float[mapSize.x, mapSize.y];

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                m_cullingMaps[x, y] = m_floatMaps[x, y] >= cullingValue ? 1 : m_floatMaps[x, y];
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(m_floatMaps == null)
        {
            return;
        }

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Color color = Color.white * (1 - m_cullingMaps[x, y]);
                color.a = 1;
                Gizmos.color = color;

                Gizmos.DrawCube(new Vector3((float)-mapSize.x / 2 + x + cubeSize / 2, (float)-mapSize.y / 2 + y + cubeSize / 2, 0), Vector3.one * cubeSize);
            }
        }
    }
}
