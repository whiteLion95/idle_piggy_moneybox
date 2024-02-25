using System;
using Mamboo.Internal.Scripts;
using UnityEngine;

#if UNITY_ANDROID
using System.Collections;
using Google.Play.Review;
#endif

#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace Mamboo.Internal
{
    public class StoreReviewManager : Singleton<StoreReviewManager>
    {
        public void AskReview()
        {
            Debug.Log("AskReview requested");
#if UNITY_IOS && !UNITY_EDITOR
        Device.RequestStoreReview();
#elif UNITY_ANDROID && !UNITY_EDITOR
        StartCoroutine(RequestAndroidReview());
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
    private IEnumerator RequestAndroidReview()
    {
        ReviewManager reviewManager = new ReviewManager();
        
        yield return null;
        
        var requestFlowOperation = reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        PlayReviewInfo playReviewInfo = requestFlowOperation.GetResult();
        
        var launchFlowOperation = reviewManager.LaunchReviewFlow(playReviewInfo);
        yield return launchFlowOperation;
        playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
    }
#endif
    }
}
