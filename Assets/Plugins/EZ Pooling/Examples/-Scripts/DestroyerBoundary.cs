using UnityEngine;
using System.Collections;

using EZ_Pooling;

public class DestroyerBoundary : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
    {
        EZ_PoolManager.Despawn(other.transform);
    }
}
