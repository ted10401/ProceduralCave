using UnityEngine;
using System.Collections.Generic;

public class MarchingSquare
{
    public int configuration;
    public Vector3 center;
    public float size = 1;
    public bool topLeft;
    public bool topRight;
    public bool bottomRight;
    public bool botoomLeft;

    public Vector3[] vertices = new Vector3[8];
    public List<int> triangles = new List<int>();

    public MarchingSquare()
    {

    }

    public MarchingSquare(Vector3 center, float size, bool topLeft, bool topRight, bool bottomRight, bool bottomLeft)
    {
        this.center = center;
        this.size = size;
        UpdateVertices();
        SetNodes(topLeft, topRight, bottomRight, bottomLeft);
    }

    public void SetCenter(Vector3 center)
    {
        this.center = center;
        UpdateVertices();
    }

    public void SetSize(float size)
    {
        this.size = size;
        UpdateVertices();
    }

    private void UpdateVertices()
    {
        vertices[0] = center + new Vector3(-size / 2, size / 2, 0);
        vertices[1] = center + new Vector3(0, size / 2, 0);
        vertices[2] = center + new Vector3(size / 2, size / 2, 0);
        vertices[3] = center + new Vector3(size / 2, 0, 0);
        vertices[4] = center + new Vector3(size / 2, -size / 2, 0);
        vertices[5] = center + new Vector3(0, -size / 2, 0);
        vertices[6] = center + new Vector3(-size / 2, -size / 2, 0);
        vertices[7] = center + new Vector3(-size / 2, 0, 0);
    }

    public void SetNodes(bool topLeft, bool topRight, bool bottomRight, bool bottomLeft)
    {
        configuration = 0;
        if (topLeft)
        {
            configuration += 8;
        }

        if (topRight)
        {
            configuration += 4;
        }

        if (bottomRight)
        {
            configuration += 2;
        }

        if (bottomLeft)
        {
            configuration += 1;
        }

        GenerateTriangles();
    }

    private void GenerateTriangles()
    {
        triangles.Clear();

        switch (configuration)
        {
            case 8:
                AddTriagnles(0, 1, 7);
                break;
            case 4:
                AddTriagnles(1, 2, 3);
                break;
            case 2:
                AddTriagnles(3, 4, 5);
                break;
            case 1:
                AddTriagnles(5, 6, 7);
                break;

            case 12:
                AddTriagnles(0, 2, 3, 7);
                break;
            case 10:
                AddTriagnles(0, 1, 3, 4, 5, 7);
                break;
            case 9:
                AddTriagnles(0, 1, 5, 6);
                break;
            case 6:
                AddTriagnles(1, 2, 4, 5);
                break;
            case 5:
                AddTriagnles(1, 2, 3, 5, 6, 7);
                break;
            case 3:
                AddTriagnles(3, 4, 6, 7);
                break;

            case 14:
                AddTriagnles(0, 2, 4, 5, 7);
                break;
            case 13:
                AddTriagnles(0, 2, 3, 5, 6);
                break;
            case 7:
                AddTriagnles(1, 2, 4, 6, 7);
                break;
            case 11:
                AddTriagnles(0, 1, 3, 4, 6);
                break;

            case 15:
                AddTriagnles(0, 2, 4, 6);
                break;
        }
    }

    private void AddTriagnles(params int[] indexes)
    {
        if(indexes.Length >= 3)
        {
            triangles.Add(indexes[0]);
            triangles.Add(indexes[1]);
            triangles.Add(indexes[2]);
        }

        if (indexes.Length >= 4)
        {
            triangles.Add(indexes[0]);
            triangles.Add(indexes[2]);
            triangles.Add(indexes[3]);
        }

        if (indexes.Length >= 5)
        {
            triangles.Add(indexes[0]);
            triangles.Add(indexes[3]);
            triangles.Add(indexes[4]);
        }

        if (indexes.Length >= 6)
        {
            triangles.Add(indexes[0]);
            triangles.Add(indexes[4]);
            triangles.Add(indexes[5]);
        }
    }
}
