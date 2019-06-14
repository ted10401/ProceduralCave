using UnityEngine;
using System.Collections.Generic;

public class MarchingSquare : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public int configuration;
    public float size = 1;
    public bool topLeft;
    public bool topRight;
    public bool bottomRight;
    public bool botoomLeft;

    private Vector3[] m_vertices = new Vector3[8];
    private List<int> m_triangles;

    private void OnValidate()
    {
        UpdateSize(size);
        AssignNodes(topLeft, topRight, bottomRight, botoomLeft);
    }

    public void UpdateSize(float size)
    {
        this.size = size;
        m_vertices[0] = new Vector3(-size / 2, size / 2, 0);
        m_vertices[1] = new Vector3(0, size / 2, 0);
        m_vertices[2] = new Vector3(size / 2, size / 2, 0);
        m_vertices[3] = new Vector3(size / 2, 0, 0);
        m_vertices[4] = new Vector3(size / 2, -size / 2, 0);
        m_vertices[5] = new Vector3(0, -size / 2, 0);
        m_vertices[6] = new Vector3(-size / 2, -size / 2, 0);
        m_vertices[7] = new Vector3(-size / 2, 0, 0);
    }

    public void AssignNodes(bool topLeft, bool topRight, bool bottomRight, bool bottomLeft)
    {
        m_triangles = new List<int>();

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

        Generate();
    }

    private void Generate()
    {
        switch(configuration)
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

        Mesh mesh = new Mesh();
        mesh.vertices = m_vertices;
        mesh.triangles = m_triangles.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    private void AddTriagnles(params int[] triangles)
    {
        if(triangles.Length >= 3)
        {
            m_triangles.Add(triangles[0]);
            m_triangles.Add(triangles[1]);
            m_triangles.Add(triangles[2]);
        }

        if (triangles.Length >= 4)
        {
            m_triangles.Add(triangles[0]);
            m_triangles.Add(triangles[2]);
            m_triangles.Add(triangles[3]);
        }

        if (triangles.Length >= 5)
        {
            m_triangles.Add(triangles[0]);
            m_triangles.Add(triangles[3]);
            m_triangles.Add(triangles[4]);
        }

        if (triangles.Length >= 6)
        {
            m_triangles.Add(triangles[0]);
            m_triangles.Add(triangles[4]);
            m_triangles.Add(triangles[5]);
        }
    }
}
