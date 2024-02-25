using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWidthToScreenResolution : MonoBehaviour
{
    [SerializeField] private float horizontalFoV = 90.0f;

    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
    }

    void Update()
    {
        float halfWidth = Mathf.Tan(0.5f * horizontalFoV * Mathf.Deg2Rad);

        float halfHeight = halfWidth * Screen.height / Screen.width;

        float verticalFoV = 2.0f * Mathf.Atan(halfHeight) * Mathf.Rad2Deg;

        _cam.fieldOfView = verticalFoV;
    }
}
