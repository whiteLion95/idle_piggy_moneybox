using System;
using System.Collections;
using System.Globalization;
using Deslab.Deslytics;
using Deslab.RemoteConfig;
using Deslab.Scripts.Deslytics.Ads;
using Deslab.UI;
using UnityEngine;

namespace Deslab
{
    public class OfflineRewardManager : MonoBehaviour
    {
        [SerializeField] private OfflineRewardData _rewardData;
        [SerializeField] private CanvasGroupWindow _canvasGroupWindow;
        [SerializeField] private ReducedBigText _amountMoneyLabel;
        private int _rewardAmount;

        // private IEnumerator Start()
        // {
        //     yield return new WaitForSeconds(7f);
        //     if (RemoteConfigManager.Instance.GetData<bool>(RemoteKeys.OfflineRewardDisabled)) yield break;
        //     GetAvailableReward();
        // }

        private void GetAvailableReward()
        {
            if (RemoteConfigManager.Instance == null ||
                RemoteConfigManager.Instance.GetData<bool>(RemoteKeys.OfflineRewardDisabled))
                return;

            DateTime lastTime = GetDateTime(_rewardData.PrefsKey, DateTime.UtcNow);

            TimeSpan timePassed = DateTime.UtcNow - lastTime;
            //int totalHours = (int)timePassed.TotalHours;
            int totalHours = (int) timePassed.TotalHours;

            if (totalHours < 1) return;

            DeslyticsManager.ShowOfflineReward();

            Debug.Log("Offline: " + totalHours);

            foreach (SOfflineRewardData data in _rewardData.RewardData)
            {
                if (totalHours >= data.TimeoutMin && totalHours <= data.TimeoutMax)
                {
                    _rewardAmount =
                        Mathf.RoundToInt(BalanceManager.Instance.AverageProfitPerMinute * data.Multiplier) / 10;
                    _amountMoneyLabel.SetValue(_rewardAmount);

                    _canvasGroupWindow.ShowWindow();
                    break;
                }
            }
        }

        public void OnGetReward()
        {
            AdsController.OnRewardedAdWatched?.Invoke(_rewardAmount);
            _canvasGroupWindow.HideWindow();
        }

        public void WatchAds()
        {
            AdsManager.ShowRewardedAds("OFFLINE_REWARD_X2", GetReward);
        }

        private void GetReward()
        {
            DeslyticsManager.RewardedShow(true);
            AdsController.OnRewardedAdWatched?.Invoke(_rewardAmount * 2);
            _canvasGroupWindow.HideWindow();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
                SetDateTime(_rewardData.PrefsKey, DateTime.UtcNow);
            else
                GetAvailableReward();
        }

        private void OnApplicationQuit()
        {
            SetDateTime(_rewardData.PrefsKey, DateTime.UtcNow);
        }

//------------------------------------------
        public static void SetDateTime(string key, DateTime value)
        {
            string result = value.ToString("u", CultureInfo.InvariantCulture);
            PlayerPrefs.SetString(key, result);
        }

        public static DateTime GetDateTime(string key, DateTime value)
        {
            if (PlayerPrefs.HasKey(key))
            {
                string prefsResult = PlayerPrefs.GetString(key);

                DateTime result = DateTime.ParseExact(prefsResult, "u", CultureInfo.InvariantCulture);
                return result;
            }
            else
            {
                return value;
            }
        }
    }
}