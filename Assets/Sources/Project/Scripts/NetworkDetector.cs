using System.Collections;
using Deslab.Deslytics;
using Deslab.RemoteConfig;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Deslab
{
    public class NetworkDetector : MonoBehaviour
    {
        [SerializeField] private GameObject networkCanvas;
        [SerializeField] private float _checkDelay = 1f;

        [BoxGroup("DEBUG MODE", true, true)] [GUIColor(0.3f, 0.8f, 0.8f, 1f)]
        public bool disableInternetCheck;
        [BoxGroup("DEBUG MODE", true, true)] [GUIColor(0.3f, 0.8f, 0.8f, 1f)]
        public bool emulateOnlineOnly = false;
        [BoxGroup("DEBUG MODE", true, true)] [GUIColor(0.3f, 0.8f, 0.8f, 1f)]
        public bool internetDisabled = false;
        
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(3f);
#if UNITY_EDITOR
            if (disableInternetCheck) yield break;
#else
            if (!RemoteConfigManager.Instance.GetData<bool>(RemoteKeys.OnlineOnly)) yield break;
#endif
            StartCoroutine(NetworkStateRoutine());
            DeslyticsManager.OnlineOnly();
        }

        private IEnumerator NetworkStateRoutine()
        {
            WaitForSecondsRealtime wfs = new WaitForSecondsRealtime(_checkDelay);
            float _timeScale = Time.timeScale;

            bool state = true;

            while (true)
            {
                yield return wfs;

                WWW www = new WWW("http://google.com");
                yield return www;

                state = www.error == null;
#if UNITY_EDITOR
                if (emulateOnlineOnly)
                    state = internetDisabled;
#endif
                NetworkState(!state);
                www.Dispose();
            }
        }

        public void NetworkState(bool state)
        {
            networkCanvas.SetActive(state);
            Time.timeScale = !state ? 1f : 0f;
        }
    }
}