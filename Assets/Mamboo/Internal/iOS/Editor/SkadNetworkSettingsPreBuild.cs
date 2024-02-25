using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Mamboo.Internal.SkadNetworkSettings.Editor
{
    public class SkadNetworkSettingsPreBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {
            CheckAndUpdateNotificationSettings(MambooSettings.Load());
        }

        public static void CheckAndUpdateNotificationSettings(MambooSettings settings)
        {
#if UNITY_IOS

            if (settings == null || settings.skadnetworkConversionRate <= 0.0f)
            {
                Debug.LogError($"Skadnetwork Conversion Rate must be higher than 0");
            }
#endif

        }
    }
}
