using System.Linq;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Mamboo.Internal.Notifications.Editor
{
    public class NotificationPreBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {
            CheckAndUpdateNotificationSettings(MambooSettings.Load());
        }

        public static void CheckAndUpdateNotificationSettings(MambooSettings settings)
        {
#if UNITY_ANDROID || UNITY_IOS

            if (settings == null || settings.pushNotificationTime.ToLower() == Mamboo.SkipTag)
            {
                Debug.LogWarning($"Mamboo: Notifications implementation skipped");
                return;
            }

            if (settings == null || string.IsNullOrEmpty(settings.pushNotificationTime))
            {
                Debug.LogError($"Notification time is empty");
            }
            else if (!settings.pushNotificationTime.Contains(":") || settings.pushNotificationTime.Count(c => c.Equals(':')) > 1)
            {
                Debug.LogError($"Notification time format is not valid");
            }
#endif
            
        }
    }
}