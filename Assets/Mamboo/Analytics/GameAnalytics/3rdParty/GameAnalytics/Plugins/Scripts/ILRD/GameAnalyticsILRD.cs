using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace GameAnalyticsSDK
{
    public class GameAnalyticsILRD
    {
        // --------- ANDROID NATIVE METHODS ---------
#if (UNITY_ANDROID) && !(UNITY_EDITOR)
        private static readonly AndroidJavaClass GA = new AndroidJavaClass("com.gameanalytics.sdk.GameAnalytics");
#if gameanalytics_mopub_enabled
        private static readonly AndroidJavaClass MoPubClass = new AndroidJavaClass("com.mopub.unity.MoPubUnityPlugin");
#endif
#if gameanalytics_topon_enabled
        private static readonly AndroidJavaClass TopOnClass = new AndroidJavaClass("com.anythink.core.api.ATSDK");
#endif
#if gameanalytics_aequus_enabled
        private static readonly AndroidJavaClass AequusClass = new AndroidJavaClass("mobi.aequus.sdk.BuildConfig");
#endif

        private static void subscribeMoPubImpressions()
        {
            GAMopubIntegration.ListenForImpressions(MopubImpressionHandler);
        }

        private static void MopubImpressionHandler(string json)
        {
#if gameanalytics_mopub_enabled
            GA.CallStatic("addImpressionMoPubEvent", MoPubClass.CallStatic<string>("getSDKVersion"), json);
#endif
        }

        private static void subscribeFyberImpressions()
        {
            GAFyberIntegration.ListenForImpressions(FyberImpressionHandler);
        }

        private static void FyberImpressionHandler(string json)
        {
#if gameanalytics_fyber_enabled
            GA.CallStatic("addImpressionFyberEvent", Fyber.FairBid.Version, json);
#endif
        }

        private static void subscribeIronSourceImpressions()
        {
            GAIronSourceIntegration.ListenForImpressions(IronSourceImpressionHandler);
        }

        private static void IronSourceImpressionHandler(string json)
        {
#if gameanalytics_ironsource_enabled

            // Remove potential label/tag from version number
            string v = IronSource.pluginVersion();
            int index = v.IndexOf("-");
            if(index >= 0)
            {
                v = v.Substring(0, index);
            }

            GA.CallStatic("addImpressionIronSourceEvent", v, json);
#endif
        }

        private static void subscribeTopOnImpressions()
        {
            GATopOnIntegration.ListenForImpressions(TopOnImpressionHandler);
        }

        private static void TopOnImpressionHandler(string json)
        {
#if gameanalytics_topon_enabled
            GA.CallStatic("addImpressionTopOnEvent", TopOnClass.CallStatic<string>("getSDKVersionName").Replace("UA_", ""), json);
#endif
        }

        private static void subscribeMaxImpressions()
        {
            GAMaxIntegration.ListenForImpressions(MaxImpressionHandler);
        }

        private static void MaxImpressionHandler(string json)
        {
#if gameanalytics_max_enabled
            GA.CallStatic("addImpressionMaxEvent", MaxSdk.Version, json);
#endif
        }

        private static void subscribeAequusImpressions()
        {
            GAAequusIntegration.ListenForImpressions(AequusImpressionHandler);
        }

        private static void AequusImpressionHandler(string json)
        {
            if(!string.IsNullOrEmpty(json))
            {
#if gameanalytics_aequus_enabled
                GA.CallStatic("addImpressionAequusEvent", AequusClass.GetStatic<string>("SDK_VERSION_NAME"), json);
#endif
            }
        }
#endif

        // --------- IOS NATIVE METHODS ---------
#if (UNITY_IOS) && (!UNITY_EDITOR)
        [DllImport ("__Internal")]
        private static extern void addImpressionEvent(string adNetworkName, string adNetworkVersion, string impressionData);
#if gameanalytics_mopub_enabled
        [DllImport("__Internal")]
        private static extern string _moPubGetSDKVersion();
#endif

#if gameanalytics_topon_enabled
        [DllImport("__Internal")]
        private static extern string getTopOnSdkVersion();
#endif

        private static void subscribeMoPubImpressions()
        {
            GAMopubIntegration.ListenForImpressions(MopubImpressionHandler);
        }

        private static void MopubImpressionHandler(string json)
        {
            if(!string.IsNullOrEmpty(json))
            {
#if gameanalytics_mopub_enabled
                addImpressionEvent("mopub", _moPubGetSDKVersion(), json);
#endif
            }
        }

        private static void subscribeFyberImpressions()
        {
            GAFyberIntegration.ListenForImpressions(FyberImpressionHandler);
        }

        private static void FyberImpressionHandler(string json)
        {
            if(!string.IsNullOrEmpty(json))
            {
#if gameanalytics_fyber_enabled
                addImpressionEvent("fyber", Fyber.FairBid.Version, json);
#endif
            }
        }

        private static void subscribeIronSourceImpressions()
        {
            GAIronSourceIntegration.ListenForImpressions(IronSourceImpressionHandler);
        }

        private static void IronSourceImpressionHandler(string json)
        {
            if(!string.IsNullOrEmpty(json))
            {
#if gameanalytics_ironsource_enabled

                // Remove potential label/tag from version number
                string v = IronSource.pluginVersion();
                int index = v.IndexOf("-");
                if(index >= 0)
                {
                    v = v.Substring(0, index);
                }

                addImpressionEvent("ironsource", v, json);
#endif
            }
        }

        private static void subscribeTopOnImpressions()
        {
            GATopOnIntegration.ListenForImpressions(TopOnImpressionHandler);
        }

        private static void TopOnImpressionHandler(string json)
        {
            if(!string.IsNullOrEmpty(json))
            {
#if gameanalytics_topon_enabled
                addImpressionEvent("topon", getTopOnSdkVersion(), json);
#endif
            }
        }

        private static void subscribeMaxImpressions()
        {
            GAMaxIntegration.ListenForImpressions(MaxImpressionHandler);
        }

        private static void MaxImpressionHandler(string json)
        {
            if(!string.IsNullOrEmpty(json))
            {
#if gameanalytics_max_enabled
                addImpressionEvent("max", MaxSdk.Version, json);
#endif
            }
        }

        private static void subscribeAequusImpressions()
        {
            GAAequusIntegration.ListenForImpressions(AequusImpressionHandler);
        }

        private static void AequusImpressionHandler(string json)
        {
            if(!string.IsNullOrEmpty(json))
            {
#if gameanalytics_aequus_enabled
                // TODO: iOS not supported yet for Aequus
#endif
            }
        }
#endif

        // ----------------------- MOPUB AD IMPRESSIONS ---------------------- //
        public static void SubscribeMoPubImpressions()
        {
#if UNITY_EDITOR
            Debug.Log("subscribeMoPubImpressions()");
#elif UNITY_IOS || UNITY_ANDROID
            subscribeMoPubImpressions();
#endif
        }

        // ----------------------- FYBER AD IMPRESSIONS ---------------------- //
        public static void SubscribeFyberImpressions()
        {
#if UNITY_EDITOR
            Debug.Log("subscribeFyberImpressions()");
#elif UNITY_IOS || UNITY_ANDROID
            subscribeFyberImpressions();
#endif
        }

        // ----------------------- IRON SOURCE AD IMPRESSIONS ---------------------- //
        public static void SubscribeIronSourceImpressions()
        {
#if UNITY_EDITOR
            Debug.Log("subscribeIronSourceImpressions()");
#elif UNITY_IOS || UNITY_ANDROID
            subscribeIronSourceImpressions();
#endif
        }

        // ----------------------- TOPON AD IMPRESSIONS ---------------------- //
        public static void SubscribeTopOnImpressions()
        {
#if UNITY_EDITOR
            Debug.Log("subscribeTopOnImpressions()");
#elif UNITY_IOS || UNITY_ANDROID
            subscribeTopOnImpressions();
#endif
        }

        // ----------------------- MAX AD IMPRESSIONS ---------------------- //
        public static void SubscribeMaxImpressions()
        {
#if UNITY_EDITOR
            Debug.Log("subscribeMaxImpressions()");
#elif UNITY_IOS || UNITY_ANDROID
            subscribeMaxImpressions();
#endif
        }

        // ----------------------- AEQUUS AD IMPRESSIONS ---------------------- //
        public static void SubscribeAequusImpressions()
        {
#if UNITY_EDITOR
            Debug.Log("subscribeAequusImpressions()");
#elif UNITY_IOS || UNITY_ANDROID
            subscribeAequusImpressions();
#endif
        }
    }
}

