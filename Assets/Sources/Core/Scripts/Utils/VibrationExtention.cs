using System.Collections;
using MoreMountains.NiceVibrations;
using UnityEngine;

namespace Deslab.Utils
{
    public class VibrationExtention : MonoBehaviour
    {
        private static VibrationExtention instance;

        private void Awake()
        {
            instance = this;
        }

        #region Vibration

        public static bool vibrationEnabled = true;
        private static bool canVibrateByWave = true;

        /// <summary>
        /// LightVibrate
        /// </summary>
        public static void LightVibrate()
        {
            if (vibrationEnabled)
                MMVibrationManager.Haptic(HapticTypes.LightImpact);
        }

        /// <summary>
        /// WaveVibrate
        /// </summary>
        public static void WaveVibrate()
        {
            if (vibrationEnabled && canVibrateByWave)
            {
                MMVibrationManager.Haptic(HapticTypes.LightImpact);
                instance.StartCoroutine(instance.LockWaveVibration());
            }
        }

        private IEnumerator LockWaveVibration()
        {
            canVibrateByWave = false;
            yield return new WaitForSecondsRealtime(0.05f);
            canVibrateByWave = true;
        }

        public static void MediumVibrate()
        {
            if (vibrationEnabled)
                MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        }

        /// <summary>
        /// SuccessVibrate
        /// </summary>
        public static void SuccessVibrate()
        {
            if (vibrationEnabled)
                MMVibrationManager.Haptic(HapticTypes.Success);
        }

        /// <summary>
        /// FailureVibrate
        /// </summary>
        public static void FailureVibrate()
        {
            if (vibrationEnabled)
                MMVibrationManager.Haptic(HapticTypes.Failure);
        }

        /// <summary>
        /// SelectionVibrate
        /// </summary>
        public static void SelectionVibrate()
        {
            if (vibrationEnabled)
            {
#if UNITY_ANDROID
                MMNVAndroid.AndroidVibrate(30);
#endif
#if UNITY_IOS
                MMVibrationManager.Haptic(HapticTypes.Selection);
#endif
            }
        }

        #endregion
    }
}
