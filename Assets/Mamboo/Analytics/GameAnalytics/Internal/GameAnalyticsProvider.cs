using GameAnalyticsSDK;
using UnityEngine;

namespace Mamboo.Internal.Analytics
{
    internal class GameAnalyticsProvider : IAnalyticsProvider
    {
        public void Initialize()
        {
            PreventLinkerFromStrippingCommonLocalizationReferences();
            
            GameAnalytics.SetBuildAllPlatforms(Application.version);
            GameAnalytics.Initialize();

#if tenjin_mopub_enabled
            GameAnalyticsILRD.SubscribeMoPubImpressions();
#else
            GameAnalyticsILRD.SubscribeMaxImpressions();
#endif
            
            Debug.Log($"[Mamboo SDK] GameAnalytics initialized");
        }
        
        private static void PreventLinkerFromStrippingCommonLocalizationReferences()
        {
            _ = new System.Globalization.GregorianCalendar();
            _ = new System.Globalization.PersianCalendar();
            _ = new System.Globalization.UmAlQuraCalendar();
            _ = new System.Globalization.ThaiBuddhistCalendar();
        }

    }
}
