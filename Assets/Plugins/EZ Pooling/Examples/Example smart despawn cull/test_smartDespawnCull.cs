using UnityEngine;
using System.Collections;
using EZ_Pooling;

public class test_smartDespawnCull : MonoBehaviour
{
    public Transform prefab;

    void Start()
    {
        InvokeRepeating("Auto", 0f, 0.8f);
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 70, 200, 30), "Spawn 20 Prefabs"))
        {
            for (int i = 0; i < 20; ++i)
            {
                EZ_PoolManager.Spawn(prefab, Random.insideUnitSphere*4, Random.rotation);
            }
        }
    }

    void Auto()
    {
        EZ_PoolManager.Spawn(prefab, Random.insideUnitSphere, Random.rotation);
    }
}
