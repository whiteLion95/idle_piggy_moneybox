using System.Collections;
using System.Collections.Generic;
using Mamboo.Internal;
using UnityEngine;

public class StoreReview : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(240);
        StoreReviewManager.instance.AskReview();
    }
}
