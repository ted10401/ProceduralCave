﻿using UnityEngine;
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

    public MarchingSquare marchingSquare;

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
        marchingSquare.AssignNodes(topLeftToggle.isOn, topRightToggle.isOn, bottomRightToggle.isOn, bottomLeftToggle.isOn);
    }
}
