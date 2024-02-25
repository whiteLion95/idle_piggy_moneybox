using UnityEngine;
using System.Collections;

public class basic_object : MonoBehaviour
{
    void OnSpawned()
    {
        //this method will be called when an object is spawned by the pool manager
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    void OnDespawned()
    {
        //this method will be called when an object is despawned by the pool manager
    }
}
