using UnityEngine;

namespace Mamboo.Internal
{
    [CreateAssetMenu(fileName = "Assets/Mamboo/Internal/Settings/Resources/MambooSettings", menuName = "Mamboo/Settings")]
    public class MambooSettings : ScriptableObject
    {
        private const string SETTING_RESOURCES_PATH = "Mamboo/Settings";

        public static MambooSettings Load() => Resources.Load<MambooSettings>(SETTING_RESOURCES_PATH);

        [Header("Mamboo version " + Mamboo.Version, order = 0)]
        [Header("GameAnalytics", order = 1)]
        [Tooltip("GameAnalytics iOS Game Key")]
        public string gameAnalyticsIosGameKey;

        [Tooltip("GameAnalytics iOS Secret Key")]
        public string gameAnalyticsIosSecretKey;

        [Tooltip("GameAnalytics Android Game Key")]
        public string gameAnalyticsAndroidGameKey;

        [Tooltip("GameAnalytics Android Secret Key")]
        public string gameAnalyticsAndroidSecretKey;

        [Header("Facebook")]
        [Tooltip("Facebook App Id")]
        public string facebookAppId;

        [Header("Notifications")]
        [Tooltip("Push notification time (format: hh:mm) example - 20:00)")]
        public string pushNotificationTime;

        [Tooltip("Game genre")]
        public GameGenre gameGenre;
    }
}