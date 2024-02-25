using UnityEngine;
//using AppsFlyerSDK;
#if FLAG_REMOTECONFIG
    using Unity.RemoteConfig;
#endif

namespace Deslab.Deslytics
{
    public class DesAppsFlyer : MonoBehaviour {
    
        [SerializeField] private string devKey;
        [SerializeField] private string appIdIOs = "Number Only";
        [SerializeField] private bool isDebug;
        [SerializeField] private bool remoteConfig;

#if FLAG_REMOTECONFIG
        private struct UserAttributes { }
        private struct AppAttributes { }
#endif
        
#if FLAG_REMOTECONFIG
        private void OnEnable()
        {
            if (!remoteConfig) return;
            ConfigManager.FetchCompleted += Fetch;
            ConfigManager.FetchConfigs(new UserAttributes(), new AppAttributes());
        }

        private void OnDisable()
        {

            if (!remoteConfig) return;
            ConfigManager.FetchCompleted -= Fetch;

        }
        
        private void Fetch(ConfigResponse obj)
        {
            devKey = ConfigManager.appConfig.GetString("AppsFlyerDevKey");
            Init();
        }
#endif
            
        private void Start()
        {
            if (remoteConfig) return;
            
            Init();
        }

        private void Init()
        {
#if !UNITY_EDITOR && FLAG_AF
            AppsFlyer.initSDK(devKey, "null");
            AppsFlyer.setIsDebug (isDebug);
            AppsFlyer.startSDK();
//#if UNITY_IOS
//            /* Mandatory - set your apple app ID
//            NOTE: You should enter the number only and not the "ID" prefix */
//            AppsFlyer.setAppID(appIdIOs);
//            AppsFlyer.getConversionData();
//            AppsFlyer.trackAppLaunch ();
////#elif UNITY_ANDROID
////           /* For getting the conversion data in Android, you need to add the "AppsFlyerTrackerCallbacks" listener.*/
////           AppsFlyer.init (devKey, "AppsFlyerTrackerCallbacks");
////#endif
#endif
        }
    }
}