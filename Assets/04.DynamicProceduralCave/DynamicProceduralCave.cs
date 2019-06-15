﻿using UnityEngine;

public class DynamicProceduralCave : MonoBehaviour
{
    public Vector2Int mapSize = new Vector2Int(10, 10);
    [Range(0f, 1f)] public float cullingValue = 0f;
    [Range(0, 10)] public int smoothInteration = 0;
    [Range(0, 8)] public int smoothThreshold = 4;
    public float cubeSize = 1f;

    private float[,] m_floatMaps;
    private float[,] m_cullingMaps;
    private int[,] m_finalMaps;

    private void OnValidate()
    {
        GenerateCullingMap();
        GenerateFinalMap();
    }

    private void Awake()
    {
        GenerateMap();
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
        GenerateFinalMap();
    }

    private void GenerateFloatMap()
    {
        m_floatMaps = new float[mapSize.x, mapSize.y];

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (x == 0 || x == mapSize.x - 1 || y == 0 || y == mapSize.y - 1)
                {
                    m_floatMaps[x, y] = 1;
                }
                else
                {
                    m_floatMaps[x, y] = Random.Range(0f, 1f);
                }
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

    private void GenerateFinalMap()
    {
        if(smoothInteration <= 0)
        {
            return;
        }

        m_finalMaps = new int[mapSize.x, mapSize.y];
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                m_finalMaps[x, y] = m_cullingMaps[x, y] >= cullingValue ? 1 : 0;
            }
        }

        for (int i = 0; i < smoothInteration; i++)
        {
            bool[,] needUpdate = new bool[mapSize.x, mapSize.y];

            for (int x = 0; x < mapSize.x; x++)
            {
                for (int y = 0; y < mapSize.y; y++)
                {
                    int wallCount = GetWallCount(m_finalMaps, x, y);
                    if (wallCount > smoothThreshold)
                    {
                        needUpdate[x, y] = m_finalMaps[x, y] == 0;
                    }
                    else
                    {
                        needUpdate[x, y] = m_finalMaps[x, y] == 1;
                    }
                }
            }

            for (int x = 0; x < mapSize.x; x++)
            {
                for (int y = 0; y < mapSize.y; y++)
                {
                    if (needUpdate[x, y])
                    {
                        m_finalMaps[x, y] = m_finalMaps[x, y] == 1 ? 0 : 1;
                    }
                }
            }
        }
    }

    private int GetWallCount(int[,] maps, int gridX, int gridY)
    {
        int wallCount = 0;
        for (int x = gridX - 1; x <= gridX + 1; x++)
        {
            for (int y = gridY - 1; y <= gridY + 1; y++)
            {
                if (x >= 0 && x < mapSize.x && y >= 0 && y < mapSize.y)
                {
                    if (x != gridX || y != gridY)
                    {
                        wallCount += maps[x, y];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
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
                if(smoothInteration == 0)
                {
                    Color color = Color.white * (1 - m_cullingMaps[x, y]);
                    color.a = 1;
                    Gizmos.color = color;
                }
                else
                {
                    Color color = Color.white * (1 - m_finalMaps[x, y]);
                    color.a = 1;
                    Gizmos.color = color;
                }

                Gizmos.DrawSphere(new Vector3((float)-mapSize.x / 2 + x + cubeSize / 2, (float)-mapSize.y / 2 + y + cubeSize / 2, 0), cubeSize / 2);
            }
        }
    }
}
