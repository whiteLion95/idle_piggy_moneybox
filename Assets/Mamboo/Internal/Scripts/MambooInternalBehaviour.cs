using System;
using UnityEngine;
using Mamboo.Internal.Analytics;
using Mamboo.Internal.Notifications;
using System.Collections;
using Mamboo.Analytics.Adjust.Internal;
using Mamboo.Internal.Scripts;

namespace Mamboo.Internal
{
    internal class MambooInternalBehaviour : MonoBehaviour
    {
        private static MambooInternalBehaviour _instance;
        private MambooSettings _mambooSettings;

        private void Awake()
        {
            if (transform != transform.root)
                throw new Exception("[Mamboo SDK] Mamboo prefab must be at the Root level.");

            _mambooSettings = MambooSettings.Load();
            if (_mambooSettings == null)
                throw new Exception("[Mamboo SDK] Cannot find Mamboo settings file.");

            if (_instance != null) {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(this);

            StartCoroutine(LoadGame());
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
            {
                AdjustWrapper.StartAdjust();
            }
        }

        private IEnumerator LoadGame()
        {
            Settings.Load();

            yield return IDFATracking.IDFATracking.WaitForIDFA();

            AnalyticsManager.Initialize(_mambooSettings);

            yield return InitNotifications(_mambooSettings);
            PrintSDKInitResult();
        }

        private IEnumerator InitNotifications(MambooSettings mambooSettings)
        {
            if (NotificationController.instance != null && mambooSettings.pushNotificationTime.ToLower() != Mamboo.SkipTag)
            {
                NotificationController.SetNotificationTime(mambooSettings.pushNotificationTime);
                yield return StartCoroutine(NotificationController.instance.RegisterNotifications());
            }
            else
                yield return null;
        }

        private void PrintSDKInitResult()
        {
            string message = "[Mamboo SDK] MambooSDK initialized!";
            Color color=Color.green;
            Debug.Log (string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));
        }

#if UNITY_EDITOR

        [UnityEditor.InitializeOnLoadMethod]
        static void InitializeOnLoad()
        {
#if lunar_debug_enabled
            LunarConsoleEditorInternal.Installer.EnablePlugin();
#else
            LunarConsoleEditorInternal.Installer.DisablePlugin();
#endif
        }

#endif
    }
}