using UnityEngine;
using UnityEngine.UI;

public class MarchingSquareHandler : MonoBehaviour
{
    public bool automatically = false;
    public float timer = 0.5f;
    private float m_timer;

    public int configuration;
    public Toggle topLeftToggle;
    public Toggle topRightToggle;
    public Toggle bottomRightToggle;
    public Toggle bottomLeftToggle;

    public bool showSegments;
    public float squareSize = 10;
    public MeshFilter meshFilter;

    private MarchingSquare m_marchingSquare;

    private void Awake()
    {
        topLeftToggle.onValueChanged.AddListener(OnToggleValueChanged);
        topRightToggle.onValueChanged.AddListener(OnToggleValueChanged);
        bottomRightToggle.onValueChanged.AddListener(OnToggleValueChanged);
        bottomLeftToggle.onValueChanged.AddListener(OnToggleValueChanged);

        configuration = 0;
        UpdateByConfiguration(configuration);
    }

    private void Update()
    {
        if(automatically)
        {
            m_timer -= Time.deltaTime;
            if(m_timer <= 0)
            {
                m_timer = timer;
                configuration++;
                configuration %= 16;
                UpdateByConfiguration(configuration);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            configuration++;
            UpdateByConfiguration(configuration);
        }
    }

    private void UpdateByConfiguration(int value)
    {
        bottomLeftToggle.isOn = value % 2 == 1;

        value /= 2;
        bottomRightToggle.isOn = value % 2 == 1;

        value /= 2;
        topRightToggle.isOn = value % 2 == 1;

        value /= 2;
        topLeftToggle.isOn = value % 2 == 1;

        OnToggleValueChanged(false);
    }

    private void OnToggleValueChanged(bool enabled)
    {
        if(m_marchingSquare == null)
        {
            m_marchingSquare = new MarchingSquare(Vector3.zero, squareSize, topLeftToggle.isOn, topRightToggle.isOn, bottomRightToggle.isOn, bottomLeftToggle.isOn);
        }
        else
        {
            m_marchingSquare.SetNodes(topLeftToggle.isOn, topRightToggle.isOn, bottomRightToggle.isOn, bottomLeftToggle.isOn);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = m_marchingSquare.vertices;
        mesh.triangles = m_marchingSquare.triangles.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    private void OnDrawGizmos()
    {
        if(m_marchingSquare == null || !showSegments)
        {
            return;
        }

        Gizmos.color = Color.black;

        for(int i = 0; i < m_marchingSquare.triangles.Count; i += 3)
        {
            Gizmos.DrawLine(meshFilter.transform.position + m_marchingSquare.vertices[m_marchingSquare.triangles[i]], meshFilter.transform.position + m_marchingSquare.vertices[m_marchingSquare.triangles[i + 1]]);
            Gizmos.DrawLine(meshFilter.transform.position + m_marchingSquare.vertices[m_marchingSquare.triangles[i]], meshFilter.transform.position + m_marchingSquare.vertices[m_marchingSquare.triangles[i + 2]]);
            Gizmos.DrawLine(meshFilter.transform.position + m_marchingSquare.vertices[m_marchingSquare.triangles[i + 1]], meshFilter.transform.position + m_marchingSquare.vertices[m_marchingSquare.triangles[i + 2]]);
        }
    }
}
