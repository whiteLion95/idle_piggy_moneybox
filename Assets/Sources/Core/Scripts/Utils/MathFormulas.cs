using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Deslab.Utils
{
    public static class MathFormulas
    {
        /// <summary>
        /// Calculates Bezier point of the cubic curve by the following formula: 
        /// [x,y] = (1–t)^3*P0 + 3(1–t)*2t*P1 + 3*(1–t)*t^2*P2 + t^3*P3
        /// </summary>
        /// <param name="t">Determines a position on the curve. 
        /// When t=0 we get the starting point of the curve, when t=1 we get the end point of the curve</param>
        /// <param name="p0">Starting control point</param>
        /// <param name="p1">Second control point</param>
        /// <param name="p2">Third control point</param>
        /// <param name="p3">End control point</param>
        /// <returns>The position on the curve, depending on the "t" parameter</returns>
        public static Vector3 CalculateBezierPoint(float t,
            Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0; //first term
            p += 3 * uu * t * p1; //second term
            p += 3 * u * tt * p2; //third term
            p += ttt * p3; // fourth term

            return p;
        }

        /// <summary>
        /// Calculates and returns the middle point between 2 vectors
        /// </summary>
        /// <param name="pointA">Vector1</param>
        /// <param name="pointB">Vector2</param>
        /// <returns>Middle point</returns>
        public static Vector3 CalculateMidPoint(Vector3 pointA, Vector3 pointB)
        {
            Vector3 midPoint = new Vector3();

            midPoint.x = (pointA.x + pointB.x) / 2;
            midPoint.y = (pointA.y + pointB.y) / 2;
            midPoint.z = (pointA.z + pointB.z) / 2;

            return midPoint;
        }
    }
}
