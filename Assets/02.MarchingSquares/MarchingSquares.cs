using UnityEngine;
using System.Collections.Generic;

public class MarchingSquares : MonoBehaviour
{
    public class Node
    {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 position)
        {
            this.position = position;
        }
    }

    public class ControlNode : Node
    {
        public bool enabled;
        public Node aboveNode;
        public Node rightNode;

        public ControlNode(Vector3 position, bool enabled, float squareSize) : base(position)
        {
            this.enabled = enabled;
            aboveNode = new Node(position + Vector3.forward * squareSize / 2);
            rightNode = new Node(position + Vector3.right * squareSize / 2);
        }
    }

    public class Square
    {
        public ControlNode topLeftNode;
        public ControlNode topRightNode;
        public ControlNode bottomLeftNode;
        public ControlNode bottomRightNode;
        public Node topNode;
        public Node bottomNode;
        public Node leftNode;
        public Node rightNode;
        public int configuration;

        public Square(ControlNode topLeftNode, ControlNode topRightNode, ControlNode bottomLeftNode, ControlNode bottomRightNode)
        {
            this.topLeftNode = topLeftNode;
            this.topRightNode = topRightNode;
            this.bottomLeftNode = bottomLeftNode;
            this.bottomRightNode = bottomRightNode;

            topNode = topLeftNode.rightNode;
            bottomNode = bottomLeftNode.rightNode;
            leftNode = bottomLeftNode.aboveNode;
            rightNode = bottomRightNode.aboveNode;

            if(topLeftNode.enabled)
            {
                configuration += 8;
            }

            if(topRightNode.enabled)
            {
                configuration += 4;
            }

            if (bottomRightNode.enabled)
            {
                configuration += 2;
            }

            if (bottomLeftNode.enabled)
            {
                configuration += 1;
            }

        }
    }

    public class SquareGrid
    {
        public Square[,] squares;

        public SquareGrid(int[,] map, float squareSize)
        {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);

            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];
            for(int x = 0; x < nodeCountX; x++)
            {
                for(int y = 0; y < nodeCountY; y++)
                {
                    Vector3 position = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, 0, -mapHeight / 2 + y * squareSize + squareSize / 2);
                    controlNodes[x, y] = new ControlNode(position, map[x, y] == 1, squareSize);
                }
            }

            squares = new Square[nodeCountX - 1, nodeCountY - 1];
            for (int x = 0; x < nodeCountX - 1; x++)
            {
                for (int y = 0; y < nodeCountY - 1; y++)
                {
                    squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x, y], controlNodes[x + 1, y]);
                }
            }
        }
    }

    public SquareGrid squareGrid;
    private List<Vector3> m_vertices;
    private List<int> m_triangles;

    public void GenerateMesh(int[,] map, float squareSize)
    {
        squareGrid = new SquareGrid(map, squareSize);
        m_vertices = new List<Vector3>();
        m_triangles = new List<int>();

        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
            {
                TriangulateSquare(squareGrid.squares[x, y]);
            }
        }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = m_vertices.ToArray();
        mesh.triangles = m_triangles.ToArray();
        mesh.RecalculateNormals();
    }

    private void TriangulateSquare(Square square)
    {
        switch(square.configuration)
        {
            case 0:
                break;

            //1 point
            case 1:
                MeshFromPoints(square.bottomNode, square.bottomLeftNode, square.leftNode);
                break;
            case 2:
                MeshFromPoints(square.rightNode, square.bottomRightNode, square.bottomNode);
                break;
            case 4:
                MeshFromPoints(square.topNode, square.topRightNode, square.rightNode);
                break;
            case 8:
                MeshFromPoints(square.topLeftNode, square.topNode, square.leftNode);
                break;

            //2 points
            case 3:
                MeshFromPoints(square.rightNode, square.bottomRightNode, square.bottomLeftNode, square.leftNode);
                break;
            case 6:
                MeshFromPoints(square.topNode, square.topRightNode, square.bottomRightNode, square.bottomNode);
                break;
            case 9:
                MeshFromPoints(square.topLeftNode, square.topNode, square.bottomNode, square.bottomLeftNode);
                break;
            case 12:
                MeshFromPoints(square.topLeftNode, square.topRightNode, square.rightNode, square.leftNode);
                break;
            case 5:
                MeshFromPoints(square.topNode, square.topRightNode, square.rightNode, square.bottomNode, square.bottomLeftNode, square.leftNode);
                break;
            case 10:
                MeshFromPoints(square.topLeftNode, square.topNode, square.rightNode, square.bottomRightNode, square.bottomNode, square.leftNode);
                break;

            //3 points
            case 7:
                MeshFromPoints(square.topNode, square.topRightNode, square.bottomRightNode, square.bottomLeftNode, square.leftNode);
                break;
            case 11:
                MeshFromPoints(square.topLeftNode, square.topNode, square.rightNode, square.bottomRightNode, square.bottomLeftNode);
                break;
            case 13:
                MeshFromPoints(square.topLeftNode, square.topRightNode, square.rightNode, square.bottomNode, square.bottomLeftNode);
                break;
            case 14:
                MeshFromPoints(square.topLeftNode, square.topRightNode, square.bottomRightNode, square.bottomNode, square.leftNode);
                break;

            //4 points
            case 15:
                MeshFromPoints(square.topLeftNode, square.topRightNode, square.bottomRightNode, square.bottomLeftNode);
                break;
        }
    }

    private void MeshFromPoints(params Node[] nodes)
    {
        AssignVertices(nodes);

        if(nodes.Length >= 3)
        {
            CreateTriangle(nodes[0], nodes[1], nodes[2]);
        }

        if(nodes.Length >= 4)
        {
            CreateTriangle(nodes[0], nodes[2], nodes[3]);
        }

        if (nodes.Length >= 5)
        {
            CreateTriangle(nodes[0], nodes[3], nodes[4]);
        }

        if (nodes.Length >= 6)
        {
            CreateTriangle(nodes[0], nodes[4], nodes[5]);
        }
    }

    private void AssignVertices(Node[] nodes)
    {
        for(int i = 0; i < nodes.Length; i++)
        {
            if(nodes[i].vertexIndex == -1)
            {
                nodes[i].vertexIndex = m_vertices.Count;
                m_vertices.Add(nodes[i].position);
            }
        }
    }

    private void CreateTriangle(Node a, Node b, Node c)
    {
        m_triangles.Add(a.vertexIndex);
        m_triangles.Add(b.vertexIndex);
        m_triangles.Add(c.vertexIndex);
    }

    private void OnDrawGizmos()
    {
        if(squareGrid == null)
        {
            return;
        }

        for(int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
            {
                Gizmos.color = (squareGrid.squares[x, y].topLeftNode.enabled) ? Color.black : Color.white;
                Gizmos.DrawCube(squareGrid.squares[x, y].topLeftNode.position, Vector3.one * 0.4f);

                Gizmos.color = (squareGrid.squares[x, y].topRightNode.enabled) ? Color.black : Color.white;
                Gizmos.DrawCube(squareGrid.squares[x, y].topRightNode.position, Vector3.one * 0.4f);

                Gizmos.color = (squareGrid.squares[x, y].bottomRightNode.enabled) ? Color.black : Color.white;
                Gizmos.DrawCube(squareGrid.squares[x, y].bottomRightNode.position, Vector3.one * 0.4f);

                Gizmos.color = (squareGrid.squares[x, y].bottomLeftNode.enabled) ? Color.black : Color.white;
                Gizmos.DrawCube(squareGrid.squares[x, y].bottomLeftNode.position, Vector3.one * 0.4f);

                Gizmos.color = Color.grey;
                Gizmos.DrawCube(squareGrid.squares[x, y].topNode.position, Vector3.one * 0.15f);
                Gizmos.DrawCube(squareGrid.squares[x, y].rightNode.position, Vector3.one * 0.15f);
                Gizmos.DrawCube(squareGrid.squares[x, y].bottomNode.position, Vector3.one * 0.15f);
                Gizmos.DrawCube(squareGrid.squares[x, y].leftNode.position, Vector3.one * 0.15f);
            }
        }
    }
}
