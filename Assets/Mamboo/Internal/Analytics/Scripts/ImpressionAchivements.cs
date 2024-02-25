using Facebook.Unity;
using Firebase.Analytics;
using System.Collections.Generic;
using UnityEngine;
using Facebook.MiniJSON;
using GameAnalyticsSDK;
using com.adjust.sdk;
using Mamboo.Analytics.Adjust.Internal;

namespace Mamboo.Internal.Analytics
{
    public class ImpressionAchivements : MonoBehaviour
    {
        private int userRewardedImpressions = 0;
        private int userInterImpressions = 0;
        private float userRewardedAdRevenue = 0;
        private float userRewardedECPM = 0;
        private float userInterAdRevenue = 0;
        private float userInterECPM = 0;
        private Dictionary<string, object> p = new Dictionary<string, object>();
        public static ImpressionAchivements instance;

        public int RewardedImpressions() => userRewardedImpressions;

        public int InterImpressions() => userInterImpressions;

        // TODO ANALYTICS: set to true if you have active subscription
        public static int isSubscribed = 0;
        public bool firebaseAppInitialised { get; private set; } = false;

        //Do not change number of achivement, just increase it for new ahivement
        readonly List<ImpressionAchivement> Achivements = new List<ImpressionAchivement>
        {
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalRevenue, Value = 0.5f, Achived = false, NumberOfAchivement = 8 },
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalRevenue, Value = 1,    Achived = false, NumberOfAchivement = 9 },
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalRevenue, Value = 2,    Achived = false, NumberOfAchivement = 10 },
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalRevenue, Value = 4,    Achived = false, NumberOfAchivement = 11 },
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalRevenue, Value = 6,    Achived = false, NumberOfAchivement = 12 },
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalRevenue, Value = 7,    Achived = false, NumberOfAchivement = 13 },
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalRevenue, Value = 10,   Achived = false, NumberOfAchivement = 14 },
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalRevenue, Value = 15,   Achived = false, NumberOfAchivement = 15 },

            new ImpressionAchivement { Type = ImpressionAchivementType.TotalEcpm, Value = 5,   Achived = false, NumberOfAchivement = 0 },
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalEcpm, Value = 15,  Achived = false, NumberOfAchivement = 1 },
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalEcpm, Value = 25,  Achived = false, NumberOfAchivement = 2 },
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalEcpm, Value = 35,  Achived = false, NumberOfAchivement = 3 },
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalEcpm, Value = 45,  Achived = false, NumberOfAchivement = 4 },
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalEcpm, Value = 65,  Achived = false, NumberOfAchivement = 5 },
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalEcpm, Value = 85,  Achived = false, NumberOfAchivement = 6 },
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalEcpm, Value = 100, Achived = false, NumberOfAchivement = 7 },

            new ImpressionAchivement { Type = ImpressionAchivementType.TotalImpressions, Value = 1,    Achived = false, NumberOfAchivement = 16 },
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalImpressions, Value = 5,    Achived = false, NumberOfAchivement = 17 },
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalImpressions, Value = 10,   Achived = false, NumberOfAchivement = 18 },
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalImpressions, Value = 20,   Achived = false, NumberOfAchivement = 24 },
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalImpressions, Value = 25,   Achived = false, NumberOfAchivement = 19 },
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalImpressions, Value = 45,   Achived = false, NumberOfAchivement = 20 },
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalImpressions, Value = 100,  Achived = false, NumberOfAchivement = 21 },
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalImpressions, Value = 200,  Achived = false, NumberOfAchivement = 22 },
            new ImpressionAchivement { Type = ImpressionAchivementType.TotalImpressions, Value = 500,  Achived = false, NumberOfAchivement = 23 }
        };
        
        void Start()
        {
            //impressions data
            if (PlayerPrefs.HasKey("userRewardedImpressions") == false)
            {
                PlayerPrefs.SetInt("userRewardedImpressions", userRewardedImpressions);
                PlayerPrefs.SetFloat("userRewardedAdRevenue", userRewardedAdRevenue);
                PlayerPrefs.SetFloat("userRewardedECPM", userRewardedECPM);

                PlayerPrefs.SetInt("userInterImpressions", userInterImpressions);
                PlayerPrefs.SetFloat("userInterAdRevenue", userInterAdRevenue);
                PlayerPrefs.SetFloat("userInterECPM", userInterECPM);

                foreach(var achivement in Achivements)
                {
                    PlayerPrefs.SetInt("impressionAchivement_" + achivement.NumberOfAchivement, 0);
                }

                PlayerPrefs.Save();
            }
            else
            {
                userRewardedImpressions = PlayerPrefs.GetInt("userRewardedImpressions");
                userRewardedAdRevenue = PlayerPrefs.GetFloat("userRewardedAdRevenue");
                userRewardedECPM = PlayerPrefs.GetFloat("userRewardedECPM");

                userInterImpressions = PlayerPrefs.GetInt("userInterImpressions");
                userInterAdRevenue = PlayerPrefs.GetFloat("userInterAdRevenue");
                userInterECPM = PlayerPrefs.GetFloat("userInterECPM");

                foreach (var achivement in Achivements)
                {
                    achivement.Achived = PlayerPrefs.GetInt("impressionAchivement_" + achivement.NumberOfAchivement, 0) == 0 
                        ? false
                        : true;
                }

                PlayerPrefs.Save();
            }
        }

        private void Awake()
        {
            if (instance == null)
                instance = this;

#if tenjin_mopub_enabled
            MoPubManager.OnImpressionTrackedEventBg += OnImpressionTrackedEvent;
            MoPubManager.OnImpressionTrackedEvent += OnImpressionTrackedEvent;
#else
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += RewardHandle;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += InterHandle;
#endif
        }

        public void LogFBAdRevenueEvent(string adRevenue)
        {
            var parameters = new Dictionary<string, object>();
            parameters["AdRevenue"] = adRevenue;
            FB.LogAppEvent(
                "Ad Revenue", 0,
                parameters
            );
        }


#if tenjin_mopub_enabled

        private void OnImpressionTrackedEvent(string adUnitId, MoPub.ImpressionData impressionData)
        {
            string allData = impressionData.JsonRepresentation;
            double revenue = 0;
            if (impressionData.PublisherRevenue.HasValue)
                revenue = (double)impressionData.PublisherRevenue;

            Debug.Log("ImpressionSuccessEvent: " + allData);

            Parameter[] AdParameters =
            {
                new Parameter("ad_platform", "mopub"),
                new Parameter("ad_source", impressionData.NetworkName),
                new Parameter("ad_unit_name", impressionData.AdUnitName),
                new Parameter("ad_format", impressionData.AdUnitFormat),
                new Parameter("currency", "USD"),
                new Parameter("value", revenue),
            };
            FirebaseAnalytics.LogEvent("ad_impression_mediation", AdParameters);

            var profile = new YandexAppMetricaUserProfile();

            if (impressionData.AdUnitFormat == "Rewarded Video")
            {
                userRewardedImpressions++;
                userRewardedAdRevenue += (float)revenue;
                userRewardedECPM = userRewardedAdRevenue / (float)userRewardedImpressions * 1000;
                FirebaseAnalytics.LogEvent($"ad_reward");

                PlayerPrefs.SetInt("userRewardedImpressions", userRewardedImpressions);
                PlayerPrefs.SetFloat("userRewardedAdRevenue", userRewardedAdRevenue);
                PlayerPrefs.SetFloat("userRewardedECPM", userRewardedECPM);

                PlayerPrefs.Save();
            }

            if (impressionData.AdUnitFormat == "Fullscreen" || impressionData.AdUnitFormat == "Medium Rectangle")
            {
                userInterImpressions++;
                userInterAdRevenue += (float)revenue;
                userInterECPM = userInterAdRevenue / (float)userInterImpressions * 1000;

                PlayerPrefs.SetInt("userInterImpressions", userInterImpressions);
                PlayerPrefs.SetFloat("userInterAdRevenue", userInterAdRevenue);
                PlayerPrefs.SetFloat("userInterECPM", userInterECPM);

                PlayerPrefs.Save();
            }

            var totalImpressions = userInterImpressions + userRewardedImpressions;
            var totalRevenue = userRewardedAdRevenue + userInterAdRevenue;
            var totalECPM = totalRevenue / totalImpressions * 1000;

            profile
                .Apply(YandexAppMetricaAttribute.CustomCounter("Tracked Ads LTV").WithDelta(totalRevenue))
                .Apply(YandexAppMetricaAttribute.CustomBoolean("Is Active Subscription")
                    .WithValue(isSubscribed == 1 ? true : false));

            if (impressionData.AdUnitFormat == "Fullscreen" || impressionData.AdUnitFormat == "Medium Rectangle")
                profile
                    .Apply(YandexAppMetricaAttribute.CustomNumber("Intestitial Impressions")
                        .WithValue(userInterImpressions))
                    .Apply(YandexAppMetricaAttribute.CustomNumber("Intestitial eCPM").WithValue(userInterECPM));
            if (impressionData.AdUnitFormat == "Rewarded Video")
                profile
                    .Apply(YandexAppMetricaAttribute.CustomNumber("Rewarded Impressions")
                        .WithValue(userRewardedImpressions))
                    .Apply(YandexAppMetricaAttribute.CustomNumber("Rewarded eCPM").WithValue(userRewardedECPM));


            AppMetrica.Instance.ReportEvent("Raw Impression Data",
                Json.Deserialize(allData) as Dictionary<string, object>);
            Dictionary<string, object> userLocalImpressionData = new Dictionary<string, object>();
            userLocalImpressionData.Add("Is Active Subscription", isSubscribed == 1 ? true : false);
            userLocalImpressionData.Add("IcrementRevenu", revenue);
            userLocalImpressionData.Add("userRewardedImpressions", userRewardedImpressions);
            userLocalImpressionData.Add("userRewardedAdRevenue", userRewardedAdRevenue);
            userLocalImpressionData.Add("userRewardedECPM", userRewardedECPM);
            userLocalImpressionData.Add("userInterImpressions", userInterImpressions);
            userLocalImpressionData.Add("userInterAdRevenue", userInterAdRevenue);
            userLocalImpressionData.Add("userInterECPM", userInterECPM);
            userLocalImpressionData.Add("totalECPM", totalECPM);
            AppMetrica.Instance.ReportEvent("User Local Impression Data", userLocalImpressionData);

            AppMetrica.Instance.ReportUserProfile(profile);
            AppMetrica.Instance.SendEventsBuffer();

            Debug.LogWarning("total rev: " + totalRevenue + " ecpm: " + totalECPM + " impression: " + totalImpressions);

            foreach (var achivement in Achivements)
            {
                if (!achivement.Achived)
                {
                    if (achivement.Type == ImpressionAchivementType.TotalRevenue)
                    {
                        p.Add("Ad Revenue", $"> {achivement.ValueString}$");
                        this.HandleTotalRevenueAchivement(totalRevenue, achivement);
                    }

                    if (achivement.Type == ImpressionAchivementType.TotalEcpm)
                    {
                        p.Add("eCPM", $"> {achivement.ValueString}$");
                        this.HandleTotalEcpmAchivement(totalECPM, achivement);
                    }

                    if (achivement.Type == ImpressionAchivementType.TotalImpressions)
                    {
                        p.Add("Ad Impressions", $"> {achivement.ValueString}");
                        this.HandleTotalImpressionAchivement(totalImpressions, achivement);
                    }

                    if (achivement.Achived)
                    {
                        PlayerPrefs.SetInt("impressionAchivement_" + achivement.NumberOfAchivement, 1);
                    }
                    p.Clear();
                }
            }
        }
#else
        private void RewardHandle(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("OnRewardedAdDisplayedEvent: " + adUnitId);
            OnImpressionTrackedEvent(adUnitId, AdjustConversions.REWARD_VIDEO_FORMAT, adInfo);
        }

        private void InterHandle(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("OnInterstitialDisplayedEvent: " + adUnitId);
            OnImpressionTrackedEvent(adUnitId, AdjustConversions.INTERSTITIAL_FORMAT, adInfo);
        }

        private void OnImpressionTrackedEvent(string adUnitId, string adFormat, MaxSdkBase.AdInfo adInfo)
        {
            var revenue = adInfo.Revenue;
            Debug.Log("[Mamboo SDK] ImpressionSuccessEvent: " + adFormat.ToStringNullOk());
            this.LogFBAdRevenueEvent(revenue.ToString());

            Parameter[] AdParameters =
            {
                new Parameter("ad_platform", "max"),
                new Parameter("ad_source", adInfo.NetworkName),
                new Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
                new Parameter("ad_format", adFormat),
                new Parameter("currency", "USD"),
                new Parameter("value", revenue),
            };

            FirebaseAnalytics.LogEvent("ad_impression_mediation", AdParameters);
            FirebaseAnalytics.LogEvent("ad_impression", AdParameters);
            FirebaseAnalytics.LogEvent("total_revenue", new Parameter("value", revenue), 
                new Parameter("currency", "USD"));
            
            AppMetrica.Instance.ReportEvent("ad_impression_mediation", new Dictionary<string, object>
            {
                {"ad_platform", "max"},
                {"ad_source", adInfo.NetworkName},
                {"ad_unit_name", adInfo.AdUnitIdentifier},
                {"ad_format", adFormat},
                {"currency", "USD"},
                {"value", revenue}
            });

            switch (adFormat)
            {
                case AdjustConversions.REWARD_VIDEO_FORMAT:
                    userRewardedImpressions++;
                    userRewardedAdRevenue += (float)revenue;
                    userRewardedECPM = userRewardedAdRevenue / userRewardedImpressions * 1000;
                    FirebaseAnalytics.LogEvent($"ad_reward");

                    PlayerPrefs.SetInt("userRewardedImpressions", userRewardedImpressions);
                    PlayerPrefs.SetFloat("userRewardedAdRevenue", userRewardedAdRevenue);
                    PlayerPrefs.SetFloat("userRewardedECPM", userRewardedECPM);

                    break;
                case AdjustConversions.INTERSTITIAL_FORMAT:
                    userInterImpressions++;
                    userInterAdRevenue += (float)revenue;
                    userInterECPM = userInterAdRevenue / userInterImpressions * 1000;

                    PlayerPrefs.SetInt("userInterImpressions", userInterImpressions);
                    PlayerPrefs.SetFloat("userInterAdRevenue", userInterAdRevenue);
                    PlayerPrefs.SetFloat("userInterECPM", userInterECPM);
                    break;
            }

            PlayerPrefs.Save();
            var totalImpressions = userInterImpressions + userRewardedImpressions;
            var totalRevenue = userRewardedAdRevenue + userInterAdRevenue;
            var totalECPM = totalRevenue / totalImpressions * 1000;

            Debug.LogWarning("total rev: " + totalRevenue + " ecpm: " + totalECPM + " impression: " + totalImpressions);

            foreach (var achivement in Achivements)
            {
                if (!achivement.Achived)
                {
                    if (achivement.Type == ImpressionAchivementType.TotalRevenue)
                    {
                        p.Add("Ad Revenue", $"> {achivement.ValueString}$");
                        this.HandleTotalRevenueAchivement(totalRevenue, achivement);
                    }

                    if (achivement.Type == ImpressionAchivementType.TotalEcpm)
                    {
                        p.Add("eCPM", $"> {achivement.ValueString}$");
                        this.HandleTotalEcpmAchivement(totalECPM, achivement);
                    }

                    if (achivement.Type == ImpressionAchivementType.TotalImpressions)
                    {
                        p.Add("Ad Impressions", $"> {achivement.ValueString}");
                        this.HandleTotalImpressionAchivement(totalImpressions, achivement);
                    }

                    if (achivement.Achived)
                    {
                        PlayerPrefs.SetInt("impressionAchivement_" + achivement.NumberOfAchivement, 1);
                    }
                    p.Clear();
                }
            }
        }

#endif

        private void HandleTotalRevenueAchivement(float totalRevenue, ImpressionAchivement achivement)
        {
            if (totalRevenue >= achivement.Value)
            {
                FirebaseAnalytics.LogEvent($"ad_revenue_{achivement.ValueWithoutComma()}_usd", new[] {
                    new Parameter(FirebaseAnalytics.ParameterCurrency, "USD"),
                    new Parameter(FirebaseAnalytics.ParameterValue, achivement.ValueString)
                });
                AppMetrica.Instance.ReportEvent("impressionAchivements", p);
                FB.LogAppEvent(AppEventName.UnlockedAchievement, totalRevenue, p);

                var adjustEvent = new AdjustEvent(AdjustConstants.TokensForOther[$"ad_revenue_{achivement.ValueWithoutComma()}_usd"]);
                Adjust.trackEvent(adjustEvent);

                Debug.LogWarning($"[Mamboo SDK] Achivement unlocked and send: ad_revenue_{achivement.ValueString}$");

                achivement.Achived = true;
            }
        }

        private void HandleTotalEcpmAchivement(float totalECPM, ImpressionAchivement achivement)
        {
            if (totalECPM >= achivement.Value)
            {
                FirebaseAnalytics.LogEvent($"ecpm_{achivement.ValueString}_usd");
                AppMetrica.Instance.ReportEvent("impressionAchivements", p);
                FB.LogAppEvent(AppEventName.UnlockedAchievement, totalECPM, p);

                var adjustEvent = new AdjustEvent(AdjustConstants.TokensForOther[$"ecpm_{achivement.ValueString}_usd"]);
                Adjust.trackEvent(adjustEvent);

                Debug.LogWarning($"[Mamboo SDK] Achivement unlocked and send: eCPM > {achivement.ValueString}$");

                achivement.Achived = true;
            }
        }

        private void HandleTotalImpressionAchivement(float totalImpressions, ImpressionAchivement achivement)
        {
            if (totalImpressions >= achivement.Value)
            {
                FirebaseAnalytics.LogEvent($"ad_impressions_{achivement.ValueString}");
                AppMetrica.Instance.ReportEvent("impressionAchivements", p);
                FB.LogAppEvent(AppEventName.UnlockedAchievement, totalImpressions, p);
                GameAnalytics.NewDesignEvent($"ad_impressions_{achivement.ValueString}");

                var adjustEvent = new AdjustEvent(AdjustConstants.TokensForOther[$"ad_impressions_{achivement.ValueString}"]);
                Adjust.trackEvent(adjustEvent);

                Debug.LogWarning($"[Mamboo SDK] Achivement unlocked and send: ad_impressions_{achivement.ValueString}");

                achivement.Achived = true;
            }
        }

        private class ImpressionAchivement
        {
            public ImpressionAchivementType Type { get; set; }
            public float Value { get; set; }
            public bool Achived { get; set; }
            public int NumberOfAchivement { get; set; }
            public string ValueString { get => Value.ToString(); }

            public string ValueWithoutComma()
            {
                return ValueString?.Replace(",", string.Empty).Replace(".", string.Empty);
            }
        }

        private enum ImpressionAchivementType
        {
            TotalRevenue = 0,
            TotalEcpm,
            TotalImpressions
        }
    }
}