using GameAnalyticsSDK;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Mamboo.Internal.Analytics.Editor
{
    public class GameAnalyticsPreBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {
            CheckAndUpdateGameAnalyticsSettings(MambooSettings.Load());
        }

        public static void CheckAndUpdateGameAnalyticsSettings(MambooSettings settings)
        {
#if UNITY_ANDROID
            if (settings == null || !CheckGASettings(settings.gameAnalyticsAndroidGameKey, settings.gameAnalyticsAndroidSecretKey,
                RuntimePlatform.Android)) {
                Debug.LogError($"Mamboo: GameAnalytics Android keys are empty");
            }
#elif UNITY_IOS
            if (settings == null || !CheckGASettings(settings.gameAnalyticsIosGameKey, settings.gameAnalyticsIosSecretKey, RuntimePlatform.IPhonePlayer))
            {
                Debug.LogError($"Mamboo: GameAnalytics iOS keys are empty");
            }
#endif
        }

        private static bool CheckGASettings(string gameKey, string secretKey, RuntimePlatform platform)
        {
            if (string.IsNullOrEmpty(gameKey) || string.IsNullOrEmpty(secretKey))
                return false;

            if (gameKey.ToLower() == Mamboo.SkipTag && secretKey.ToLower() == Mamboo.SkipTag)
            {
                Debug.LogWarning($"Mamboo: GameAnalytics implementation skipped");
                return true;
            }

            if (!GameAnalytics.SettingsGA.Platforms.Contains(platform))
                GameAnalytics.SettingsGA.AddPlatform(platform);

            int platformIndex = GameAnalytics.SettingsGA.Platforms.IndexOf(platform);
            GameAnalytics.SettingsGA.UpdateGameKey(platformIndex, gameKey);
            GameAnalytics.SettingsGA.UpdateSecretKey(platformIndex, secretKey);
            GameAnalytics.SettingsGA.Build[platformIndex] = Application.version;
            return true;
        }
    }
}