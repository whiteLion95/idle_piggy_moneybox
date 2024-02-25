using UnityEngine;
using System.Collections;

using EZ_Pooling;

public class test_sandbox : MonoBehaviour 
{
    public Transform[] prefabs;

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 70, 100, 30), "Spawn prefab 1"))
        {
            EZ_PoolManager.Spawn(prefabs[0], Random.insideUnitSphere * 4, Random.rotation);
        }
        if (GUI.Button(new Rect(10, 100, 100, 30), "Spawn prefab 2"))
        {
            EZ_PoolManager.Spawn(prefabs[1], Random.insideUnitSphere * 4, Random.rotation);
        }
        if (GUI.Button(new Rect(10, 130, 100, 30), "Spawn prefab 3"))
        {
            EZ_PoolManager.Spawn(prefabs[2], Random.insideUnitSphere * 4, Random.rotation);
        }
        if (GUI.Button(new Rect(10, 160, 100, 30), "Spawn prefab 4"))
        {
            EZ_PoolManager.Spawn(prefabs[3], Random.insideUnitSphere * 4, Random.rotation);
        }
    }
}
