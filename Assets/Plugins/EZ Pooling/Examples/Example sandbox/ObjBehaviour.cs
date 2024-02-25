using UnityEngine;
using System.Collections;

using EZ_Pooling;

public class ObjBehaviour : MonoBehaviour 
{
    void OnSpawned () 
    {
        StartCoroutine(Decay());
	}

    IEnumerator Decay()
    {
        yield return new WaitForSeconds(3f);

        EZ_PoolManager.Despawn(this.transform);
    }
}
