using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerPath : MonoBehaviour
{
    [field: SerializeField] public Transform Point1;
    [field: SerializeField] public Transform Point2;

    public Transform GetRandomPoint()
    {
        if (Point1 != null && Point2 != null)
        {
            int randIndex = Random.Range(0, 2);
            Transform randPoint = (randIndex == 0) ? Point1 : Point2;
            return randPoint;
        }

        return null;
    }
}
