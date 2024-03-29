﻿using UnityEngine;
using System.Collections.Generic;

public class ProceduralCave : MonoBehaviour
{
    public Vector2Int mapSize = new Vector2Int(10, 10);
    [Range(0f, 1f)] public float cullingValue = 0f;
    [Range(0, 10)] public int smoothIteration = 0;
    [Range(0, 8)] public int smoothThreshold = 4;
    public float cubeSize = 1f;
    public bool showMesh;

    public MeshFilter meshFilter;
    private float[,] m_floatMaps;
    private float[,] m_cullingMaps;
    private int[,] m_finalMaps;

    private void OnValidate()
    {
        if(m_floatMaps != null)
        {
            if(m_floatMaps.GetLength(0) != mapSize.x || m_floatMaps.GetLength(1) != mapSize.y)
            {
                GenerateFloatMap();
            }
        }

        GenerateCullingMap();
        GenerateFinalMap();
        GenerateMesh();
    }

    private void Awake()
    {
        GenerateMap();
    }

    public bool showAnimation;
    public float timer;
    public float generateTimer;
    private void Update()
    {
        if(showAnimation)
        {
            timer += Time.deltaTime;

            if (timer >= 5f)
            {
                smoothIteration = 4;
            }

            if (timer >= 10f)
            {
                showMesh = true;
            }

            if (timer < 2.5f)
            {
                cullingValue = (2.5f - timer) / 2.5f;
                if(cullingValue <= 0)
                {
                    cullingValue = 0;
                }
            }
            else if(timer < 5f)
            {
                cullingValue = (timer - 2.5f) / 2.5f;

                if (cullingValue >= 1)
                {
                    cullingValue = 1;
                }
            }
            else if (timer < 7.5f)
            {
                cullingValue = (7.5f - timer) / 2.5f;

                if (cullingValue <= 0)
                {
                    cullingValue = 0;
                }
            }
            else if (timer < 10f)
            {
                cullingValue = (timer - 7.5f) / 2.5f;

                if (cullingValue >= 1)
                {
                    cullingValue = 1;
                }
            }
            else if (timer < 12.5f)
            {
                cullingValue = (12.5f - timer) / 2.5f;

                if (cullingValue <= 0)
                {
                    cullingValue = 0;
                }
            }
            else if (timer < 15f)
            {
                cullingValue = (timer - 12.5f) / 2.5f;

                if (cullingValue >= 1)
                {
                    cullingValue = 1;
                }
            }
            else
            {
                cullingValue = 0.55f;
                smoothIteration = 5;
                smoothThreshold = 4;
                showMesh = true;

                generateTimer += Time.deltaTime;
                if(generateTimer >= 1)
                {
                    generateTimer = 0f;
                    GenerateMap();
                }
            }

            GenerateCullingMap();
            GenerateFinalMap();
            GenerateMesh();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateMap();
        }
    }

    private void GenerateMap()
    {
        GenerateFloatMap();
        GenerateCullingMap();
        GenerateFinalMap();
        GenerateMesh();
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
        if(m_cullingMaps == null)
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

        if (smoothIteration <= 0)
        {
            return;
        }

        for (int i = 0; i < smoothIteration; i++)
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
                    else if (wallCount < smoothThreshold)
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
                    if(x == gridX && y == gridY)
                    {
                        continue;
                    }

                    wallCount += maps[x, y];
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    private void GenerateMesh()
    {
        if (!showMesh)
        {
            meshFilter.mesh = null;
            return;
        }

        if (m_finalMaps == null)
        {
            return;
        }

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        MarchingSquare marchingSquare = new MarchingSquare();
        int count = 0;

        for (int x = 0; x < mapSize.x - 1; x++)
        {
            for (int y = 0; y < mapSize.y - 1; y++)
            {
                marchingSquare.SetCenter(new Vector3(-mapSize.x * 0.5f + x + cubeSize, -mapSize.y * 0.5f + y + cubeSize, 0));
                marchingSquare.SetNodes(
                    m_finalMaps[x, y + 1] == 1,
                    m_finalMaps[x + 1, y + 1] == 1,
                    m_finalMaps[x + 1, y] == 1,
                    m_finalMaps[x, y] == 1);

                vertices.AddRange(marchingSquare.vertices);
                foreach (int triangle in marchingSquare.triangles)
                {
                    triangles.Add(triangle + count);
                }

                count += 8;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    private void OnDrawGizmos()
    {
        if(showMesh)
        {
            return;
        }

        if (m_floatMaps == null)
        {
            return;
        }

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if(smoothIteration == 0)
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
