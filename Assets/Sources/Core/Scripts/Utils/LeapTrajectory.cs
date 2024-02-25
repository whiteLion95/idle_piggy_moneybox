using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Deslab.Utils
{
    public class LeapTrajectory : MonoBehaviour
    {
        [SerializeField] private Transform[] controlPoints;
        [SerializeField] [Range(0.01f, 0.1f)] private float _density = 0.01f;
        //[SerializeField] [Range(0.05f, 0.5f)] private float _gizmosRadius = 0.1f;
        [SerializeField] [Range(0f, 1f)] private float _relYPosOfCP2 = 0.4f;
        [SerializeField] [Range(0f, 1f)] private float _relZPosOfCP2 = 0.25f;
        [SerializeField] [Range(0f, 1f)] private float _relYPosOfCP3 = 0.4f;
        [SerializeField] [Range(-1f, 0f)] private float _relZPosOfCP3 = -0.25f;
        [SerializeField] private float _distance;

        private Vector3 _gizmosPosition;

        public float Distance
        {
            set
            {
                Vector3 tempPos = controlPoints[3].localPosition;
                tempPos.z = value;
                controlPoints[3].localPosition = tempPos;
            }
        }

        public Transform this[int i]
        {
            get
            {
                if (i >= 0 && i < controlPoints.Length)
                {
                    return controlPoints[i];
                }
                return null;
            }
        }

        private void Update()
        {
            Vector3 distanceVector = controlPoints[3].position - controlPoints[0].position;
            _distance = distanceVector.magnitude;
            controlPoints[1].localPosition = new Vector3(distanceVector.x, _relYPosOfCP2 * _distance, _relZPosOfCP2 * _distance);
            controlPoints[2].localPosition = new Vector3(distanceVector.x, _relYPosOfCP3 * _distance, _relZPosOfCP3 * _distance);

            for (float t = 0; t <= 1; t += _density)
            {
                _gizmosPosition = MathFormulas.CalculateBezierPoint(t, controlPoints[0].position, controlPoints[1].position,
                    controlPoints[2].position, controlPoints[3].position);

                //Gizmos.DrawSphere(_gizmosPosition, _gizmosRadius);
            }

            //DrawCurve();
        }

        //protected virtual void OnDrawGizmos()
        //{
        //    Vector3 distanceVector = controlPoints[3].position - controlPoints[0].position;
        //    _distance = distanceVector.magnitude;
        //    controlPoints[1].localPosition = new Vector3(distanceVector.x, _relYPosOfCP2 * _distance, _relZPosOfCP2 * _distance);
        //    controlPoints[2].localPosition = new Vector3(distanceVector.x, _relYPosOfCP3 * _distance, _relZPosOfCP3 * _distance);

        //    for (float t = 0; t <= 1; t += _density)
        //    {
        //        _gizmosPosition = MathFormulas.CalculateBezierPoint(t, controlPoints[0].position, controlPoints[1].position,
        //            controlPoints[2].position, controlPoints[3].position);

        //        Gizmos.DrawSphere(_gizmosPosition, _gizmosRadius);
        //    }

        //    DrawCurve();
        //}

        //protected void DrawCurve()
        //{
        //    Gizmos.DrawLine(new Vector3(controlPoints[0].position.x, controlPoints[0].position.y, controlPoints[0].position.z),
        //        new Vector3(controlPoints[1].position.x, controlPoints[1].position.y, controlPoints[1].position.z));
        //    Gizmos.DrawLine(new Vector3(controlPoints[2].position.x, controlPoints[2].position.y, controlPoints[2].position.z),
        //        new Vector3(controlPoints[3].position.x, controlPoints[3].position.y, controlPoints[3].position.z));
        //}
    }
}
