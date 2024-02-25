using UnityEngine;

namespace Deslab.Utils
{
    public class CameraConstantWidth : MonoBehaviour
    {
        public Vector2 DefaultResolution = new Vector2(720, 1280);
        [Range(0f, 1f)] public float WidthOrHeight = 0;
        public bool useUpdate = false;

        private Camera componentCamera;

        private float initialSize;
        private float targetAspect;

        private float initialFov;
        private float horizontalFov = 120f;

        private void Start()
        {
            componentCamera = GetComponent<Camera>();
            initialSize = componentCamera.orthographicSize;

            targetAspect = DefaultResolution.x / DefaultResolution.y;

            initialFov = componentCamera.fieldOfView;
            horizontalFov = CalcVerticalFov(initialFov, 1 / targetAspect);

            if (componentCamera.orthographic)
            {
                float constantWidthSize = initialSize * (targetAspect / componentCamera.aspect);
                componentCamera.orthographicSize = constantWidthSize;
            }
            else
            {
                float constantWidthFov = CalcVerticalFov(horizontalFov, componentCamera.aspect);
                componentCamera.fieldOfView = constantWidthFov;
            }
        }

        private void Update()
        {
            if (useUpdate)
            {
                if (componentCamera.orthographic)
                {
                    float constantWidthSize = initialSize * (targetAspect / componentCamera.aspect);
                    componentCamera.orthographicSize = Mathf.Lerp(constantWidthSize, initialSize, WidthOrHeight);
                }
                else
                {
                    float constantWidthFov = CalcVerticalFov(horizontalFov, componentCamera.aspect);
                    componentCamera.fieldOfView = Mathf.Lerp(constantWidthFov, initialFov, WidthOrHeight);
                }
            }
        }

        private float CalcVerticalFov(float hFovInDeg, float aspectRatio)
        {
            float hFovInRads = hFovInDeg * Mathf.Deg2Rad;

            float vFovInRads = 2 * Mathf.Atan(Mathf.Tan(hFovInRads / 2) / aspectRatio);

            return vFovInRads * Mathf.Rad2Deg;
        }
    }
}