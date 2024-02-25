using com.adjust.sdk;
using UnityEngine;

namespace Mamboo.Analytics.Adjust.Internal
{
    public static class AdjustWrapper
    {
        internal static void Initialize()
        {
            StartAdjust();
        }

        public static void StartAdjust()
        {
            var config = new AdjustConfig(AdjustConstants.AppToken, AdjustConstants.Environment, true);
            config.setLogLevel(AdjustLogLevel.Suppress);
            config.setPreinstallTrackingEnabled(true);
            com.adjust.sdk.Adjust.start(config);
            Debug.Log($"[Mamboo SDK] Adjust initialized");
        }
    }
}