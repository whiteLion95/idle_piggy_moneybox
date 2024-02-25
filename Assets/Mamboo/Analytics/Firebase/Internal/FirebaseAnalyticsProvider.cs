using Deslab.RemoteConfig;
using UnityEngine;

namespace Mamboo.Internal.Analytics
{
    public class FirebaseAnalyticsProvider: MonoBehaviour
    {
        internal static FirebaseAnalyticsProvider instance;
        internal static Firebase.FirebaseApp app;

        private void Start()
        {
            if (instance == null)
                instance = this;

            var dubl = FindObjectsOfType<FirebaseAnalyticsProvider>();
            if (dubl.Length > 1)
            {
                Destroy(this.gameObject);
            }

            DontDestroyOnLoad(gameObject);

            ConnectFirebase();
        }

        private void Awake()
        {
            if (instance == null)
                instance = FindObjectOfType<FirebaseAnalyticsProvider>();
        }

        private void ConnectFirebase()
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    app = Firebase.FirebaseApp.DefaultInstance;

                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                    RemoteConfigManager.Instance.InitFirebaseRemoteConfig();
                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }
    }
}
