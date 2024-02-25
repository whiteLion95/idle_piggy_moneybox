using UnityEngine;
using System.Collections;
using EZ_Pooling;

public class test_recycleInstances : MonoBehaviour 
{
    public Transform prefab;

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 70, 100, 30), "Spawn"))
        {
            EZ_PoolManager.Spawn(prefab, Random.insideUnitSphere, Random.rotation);
        }
    }
}
