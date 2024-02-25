using Facebook.Unity;
using UnityEngine;

namespace Mamboo.Internal.Analytics
{
    internal class FacebookAnalyticsProvider : IAnalyticsProvider
    {
        public void Initialize()
        {
            FB.Init();
            Debug.Log($"[Mamboo SDK] Facebook initialized");
        }
    }
}
