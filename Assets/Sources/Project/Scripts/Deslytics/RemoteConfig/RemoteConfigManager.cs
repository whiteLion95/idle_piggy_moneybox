using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Extensions;
using UnityEngine;

namespace Deslab.RemoteConfig
{
    public class RemoteConfigManager : MonoSingleton<RemoteConfigManager>
    {
        [SerializeField] private DataKeys dataKeys;
        Dictionary<string, object> defaultsData = new Dictionary<string, object>();

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);
            SetDefaults();
        }

        public T GetData<T>(string key)
        {
            return (T)dataKeys.Data.Find(data => data.Key == key).Value;
        }

        public void SetData<T>(string key, T value)
        {
            dataKeys.Data.Find(data => data.Key == key).Value = value;
        }

        public void InitFirebaseRemoteConfig()
        {
            FetchDataAsync();
        }

        private void SetDefaults()
        {
            defaultsData.Add(RemoteKeys.OfflineRewardDisabled, false);
            SetData(RemoteKeys.OfflineRewardDisabled, false);

            defaultsData.Add(RemoteKeys.OnlineOnly, false);
            SetData(RemoteKeys.OnlineOnly, false);

            defaultsData.Add(RemoteKeys.TransformRewardToInter, false);
            SetData(RemoteKeys.TransformRewardToInter, false);

            defaultsData.Add(RemoteKeys.InterCounterBeforeSpecialOffer, 2);
            SetData(RemoteKeys.InterCounterBeforeSpecialOffer, 2);

            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaultsData)
                .ContinueWithOnMainThread(task => { });
        }

        public Task FetchDataAsync()
        {
            Debug.Log("Fetching data...");
            System.Threading.Tasks.Task fetchTask =
                Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
                    TimeSpan.Zero);
            return fetchTask.ContinueWithOnMainThread(FetchComplete);
        }

        void FetchComplete(Task fetchTask)
        {
            if (fetchTask.IsCanceled)
            {
                Debug.Log("Fetch canceled.");
            }
            else if (fetchTask.IsFaulted)
            {
                Debug.Log("Fetch encountered an error.");
            }
            else if (fetchTask.IsCompleted)
            {
                Debug.Log("Fetch completed successfully!");
            }

            var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
            switch (info.LastFetchStatus)
            {
                case Firebase.RemoteConfig.LastFetchStatus.Success:
                    Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
                        .ContinueWithOnMainThread(task =>
                        {
                            UpdateRemoteData();
                            Debug.LogError(String.Format("Remote data loaded and ready (last fetch time {0}).",
                                info.FetchTime));
                        });

                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Failure:
                    switch (info.LastFetchFailureReason)
                    {
                        case Firebase.RemoteConfig.FetchFailureReason.Error:
                            Debug.Log("Fetch failed for unknown reason");
                            break;
                        case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                            Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                            break;
                    }

                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Pending:
                    Debug.Log("Latest Fetch call still pending.");
                    break;
            }
        }

        private void UpdateRemoteData()
        {
            var firebaseRemoteConfig = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance;
            
            SetData(RemoteKeys.OfflineRewardDisabled, firebaseRemoteConfig
                .GetValue(RemoteKeys.OfflineRewardDisabled).BooleanValue);

            SetData(RemoteKeys.OnlineOnly, firebaseRemoteConfig
                .GetValue(RemoteKeys.OnlineOnly).BooleanValue);

            SetData(RemoteKeys.TransformRewardToInter, firebaseRemoteConfig
                .GetValue(RemoteKeys.TransformRewardToInter).BooleanValue);

            SetData(RemoteKeys.InterCounterBeforeSpecialOffer, (int) firebaseRemoteConfig
                .GetValue(RemoteKeys.InterCounterBeforeSpecialOffer).LongValue);
        }
    }
}