using System;
using UnityEditor;
using UnityEngine;
using Mamboo.Internal.Analytics.Editor;
using Mamboo.Internal.Notifications.Editor;
using Mamboo.Internal.SkadNetworkSettings.Editor;

namespace Mamboo.Internal.Editor
{
    [CustomEditor(typeof(MambooSettings))]
    public class MambooSettingsEditor : UnityEditor.Editor
    {
        private MambooSettings Settings => target as MambooSettings;

        [MenuItem("Mamboo/Settings")]
        private static void EditSettings()
        {
            Selection.activeObject = CreateOrGetSettings();
        }

        private static MambooSettings CreateOrGetSettings()
        {
            MambooSettings settings = MambooSettings.Load();
            if (settings == null) {
                settings = CreateInstance<MambooSettings>();
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    AssetDatabase.CreateFolder("Assets", "Resources");

                if (!AssetDatabase.IsValidFolder("Assets/Resources/Mamboo"))
                    AssetDatabase.CreateFolder("Assets/Resources", "Mamboo");

                AssetDatabase.CreateAsset(settings, "Assets/Resources/Mamboo/Settings.asset");
                settings = MambooSettings.Load();
            }

            return settings;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(15);

#if UNITY_IOS || UNITY_ANDROID      
            if (GUILayout.Button(Environment.NewLine + "Validate settings" + Environment.NewLine)) {
                ValidateSettings(Settings);
            }  
#endif
        }

        private static void ValidateSettings(MambooSettings settings)
        {
            Console.Clear();

            GameAnalyticsPreBuild.CheckAndUpdateGameAnalyticsSettings(settings);
            FacebookPreBuild.CheckAndUpdateFacebookSettings(settings);
            NotificationPreBuild.CheckAndUpdateNotificationSettings(settings);
            SkadNetworkSettingsPreBuild.CheckAndUpdateNotificationSettings(settings);
        }
    }
}