using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralCaveGenerator : MonoBehaviour
{
    public int width;
    public int height;

    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int randomFillPercent;

    private int[,] m_map;

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

        if (Input.GetKeyDown(KeyCode.S))
        {
            SmoothMap();
        }
    }

    private void GenerateMap()
    {
        m_map = new int[width, height];
        RandomFillMap();
    }

    private void RandomFillMap()
    {
        if(useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        System.Random random = new System.Random(seed.GetHashCode());

        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    m_map[x, y] = 1;
                }
                else
                {
                    m_map[x, y] = random.Next(0, 100) < randomFillPercent ? 1 : 0;
                }
            }
        }
    }

    private void SmoothMap()
    {
        bool[,] needUpdate = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int wallCount = GetWallCount(x, y);
                if(wallCount > 4)
                {
                    needUpdate[x, y] = m_map[x, y] != 1;
                }
                else
                {
                    needUpdate[x, y] = m_map[x, y] != 0;
                }
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (needUpdate[x, y])
                {
                    m_map[x, y] = m_map[x, y] == 1 ? 0 : 1;
                }
            }
        }

        MarchingSquares marchingSquares = GetComponent<MarchingSquares>();
        marchingSquares.GenerateMesh(m_map, 1);
    }

    private int GetWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for(int x = gridX - 1; x <= gridX + 1; x++)
        {
            for (int y = gridY - 1; y <= gridY + 1; y++)
            {
                if(x >= 0 && x < width && y >= 0 && y < height)
                {
                    if(x != gridX || y != gridY)
                    {
                        wallCount += m_map[x, y];
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
        //if(m_map == null)
        //{
        //    return;
        //}

        //for (int x = 0; x < width; x++)
        //{
        //    for (int y = 0; y < height; y++)
        //    {
        //        Gizmos.color = m_map[x, y] == 1 ? Color.black : Color.white;
        //        Gizmos.DrawCube(new Vector3(-width / 2 + x + 0.5f, 0, -height / 2 + y + 0.5f), Vector3.one);
        //    }
        //}
    }
}
