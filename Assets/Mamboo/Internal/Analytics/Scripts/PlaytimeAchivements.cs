using System;
using System.Collections.Generic;
using Firebase.Analytics;
using UnityEngine;
using com.adjust.sdk;
using Mamboo.Analytics.Adjust.Internal;

namespace Mamboo.Internal.Analytics
{
    public class PlaytimeAchivements : MonoBehaviour
    {
        public static PlaytimeAchivements instance;
        List<PlaytimeAchivement> Achivements = new List<PlaytimeAchivement>
        {
            new PlaytimeAchivement { Value = 5,   Achived = false, NumberOfAchivement = 5 },
            new PlaytimeAchivement { Value = 10,   Achived = false, NumberOfAchivement = 6 },
            new PlaytimeAchivement { Value = 20,   Achived = false, NumberOfAchivement = 7 },
            new PlaytimeAchivement { Value = 40,   Achived = false, NumberOfAchivement = 0 },
            new PlaytimeAchivement { Value = 80,   Achived = false, NumberOfAchivement = 1 },
            new PlaytimeAchivement { Value = 150,  Achived = false, NumberOfAchivement = 2 },
            new PlaytimeAchivement { Value = 250,  Achived = false, NumberOfAchivement = 3 },
            new PlaytimeAchivement { Value = 400,  Achived = false, NumberOfAchivement = 4 }
        };
        

        public static Saveable<double> startTime = new Saveable<double>("startTime", 0);
        public static Saveable<int> playTime = new Saveable<int>("playTime", 0);

        private static float oneMin = 0;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (!PlayerPrefs.HasKey("startTime"))
                startTime.value = DateTime.UtcNow.TotalSeconds();

            var hasFirstAchivement = PlayerPrefs.HasKey("playTimeAchivements_0");
            foreach (var achivement in Achivements)
            {
                if (hasFirstAchivement)
                {
                    achivement.Achived = PlayerPrefs.GetInt("playTimeAchivements_" + achivement.NumberOfAchivement, 0) == 1 ? true : false;
                }
                else
                    PlayerPrefs.SetInt("playTimeAchivements_" + achivement.NumberOfAchivement, 0);
            }
            PlayerPrefs.Save();
            DontDestroyOnLoad(this.gameObject);
        }

        private void Update()
        {
            oneMin += Time.deltaTime;
            if (oneMin < 60)
                return;

            playTime.value += (int)oneMin;
            oneMin = 0;
            
            if (playTime % 60 != 0) return;

            Debug.Log("[Mamboo SDK] <color=blue>PlayTime: " + playTime / 60 + " min.</color>");

            if (playTime % 1500 == 0 && playTime.value <= 6000)
                Firebase.Analytics.FirebaseAnalytics.LogEvent("PlayTime:" + (playTime.value / 60).ToString() + "minutes");

            var profile = new YandexAppMetricaUserProfile();
            profile.Apply(YandexAppMetricaAttribute.CustomNumber("PlayTime").WithValue(playTime.value / 60));

            if (ImpressionAchivements.instance.RewardedImpressions() > 0)
                profile.Apply(YandexAppMetricaAttribute.CustomNumber("Rewarded Density").WithValue(playTime.value / 60 / ImpressionAchivements.instance.RewardedImpressions()));

            if (ImpressionAchivements.instance.InterImpressions() > 0)
                profile.Apply(YandexAppMetricaAttribute.CustomNumber("Inter Density").WithValue(playTime.value / 60 / ImpressionAchivements.instance.InterImpressions()));

            AppMetrica.Instance.ReportUserProfile(profile);
            AppMetrica.Instance.SendEventsBuffer();

            CheckPlayTimeAchievements(playTime.value / 60);
        }

        void HandlePlaytimeAchivement(PlaytimeAchivement achivement, int playTimeMinutes)
        {
            if (playTimeMinutes > achivement.Value && !achivement.Achived)
            {
                FirebaseAnalytics.LogEvent($"PlayTime_{achivement.Value}");
                Debug.LogWarning($"Achivement unlocked and send: PlayTime_{achivement.Value}");

                var adjustEvent = new AdjustEvent(AdjustConstants.TokensForPlaytime[$"playtime_{achivement.Value}"]);
                Adjust.trackEvent(adjustEvent);

                achivement.Achived = true;
            }
        }

        void CheckPlayTimeAchievements(int playTimeMinutes)
        {
            foreach(var achivement in Achivements)
            {
                if (!achivement.Achived)
                {
                    HandlePlaytimeAchivement(achivement, playTimeMinutes);

                    if (achivement.Achived)
                        PlayerPrefs.SetInt("playTimeAchivements_" + achivement.NumberOfAchivement, 1);
                }
            }
            PlayerPrefs.Save();
        }

        private class PlaytimeAchivement
        {
            public float Value { get; set; }
            public bool Achived { get; set; }
            public int NumberOfAchivement { get; set; }
            public string ValueString { get => Value.ToString(); }

            public string ValueWithoutComma()
            {
                return ValueString?.Replace(",", string.Empty).Replace(".", string.Empty);
            }
        }
    }

    public class Saveable<T>
    {
        public static implicit operator T(Saveable<T> d) => d.value;
        struct Wrapper<R> { public R value; };

        private string name;
        private T _value;

        public Saveable(string inName, T defaultValue)
        {
            name = inName;
            _value = defaultValue;
        }

        public bool IsSetted()
        {
            return PlayerPrefs.HasKey(name);
        }

        public T value
        {
            get
            {
                if (PlayerPrefs.HasKey(name))
                {
                    var temp = JsonUtility.FromJson<Wrapper<T>>(PlayerPrefs.GetString(name));
                    _value = temp.value;
                }
                return _value;
            }
            set
            {
                if (!value.Equals(_value))
                    Save(value);
            }
        }

        private void Save(T value)
        {
            _value = value;
            var temp = new Wrapper<T> { value = _value };
            PlayerPrefs.SetString(name, JsonUtility.ToJson(temp));
        }
    }

    internal static class DateTimeExtensions
    {
        public static double TotalSeconds(this DateTime dateTime) => DateTime.Now.Subtract(DateTime.MinValue).TotalSeconds;
    }
}