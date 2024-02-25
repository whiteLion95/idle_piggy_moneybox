using System.Collections.Generic;
using Facebook.Unity.Settings;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Mamboo.Internal.Analytics.Editor
{
    public class FacebookPreBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {
            CheckAndUpdateFacebookSettings(MambooSettings.Load());
        }

        public static void CheckAndUpdateFacebookSettings(MambooSettings settings)
        {
#if UNITY_ANDROID || UNITY_IOS

            if (settings == null || settings.facebookAppId.ToLower() == Mamboo.SkipTag)
            {
                Debug.LogWarning($"Mamboo: Facebook implementation skipped");
                return;
            }

            if (settings == null || string.IsNullOrEmpty(settings.facebookAppId))
                Debug.LogError($"Facebook app Id is empty");
            else
            {
                FacebookSettings.AppIds = new List<string> { settings.facebookAppId};
                FacebookSettings.AppLabels = new List<string> {Application.productName};
                EditorUtility.SetDirty(FacebookSettings.Instance);
            }      
#endif
            
        }
    }
}