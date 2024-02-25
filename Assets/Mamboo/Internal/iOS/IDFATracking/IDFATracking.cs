using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace IDFATracking
{
    public enum Status
    {
        NotDetermined, //No requests have been made, status unknown
        Restricted, //User has explicitly blocked tracking requests from ALL apps via device settings
        Denied, //Request has been made and user denied tracking
        Authorized, //Request has been made and user allowed tracking
    }

    public delegate void RequestDelegate(Status status);

    internal interface IDFAPlatform
    {
        void RequestTracking(RequestDelegate on_complete);
        Status GetCurrentStatus();
    }

    public static class IDFATracking
    {
        public static IEnumerator WaitForIDFA()
        {
#if UNITY_IOS && !UNITY_EDITOR
            var version = Device.systemVersion;
            Debug.LogWarning("iOS version = " + version);
            var nums = version.Split('.');
            var isOs145OrHigher = (Convert.ToInt32(nums[0]) == 14 && Convert.ToInt32(nums[1]) >= 5) || Convert.ToInt32(nums[0]) > 14;
            if (isOs145OrHigher)
            {
                if (GetCurrentStatus() == Status.NotDetermined)
                {
                    bool gotResponse = false;

                    IDFATracking.RequestTracking(OnIDFAStatus);
                    void OnIDFAStatus(Status status)
                    {
                        Debug.Log("===> App Tracking Transparency Authorization Status: " + status);
                        gotResponse = true;
                    }

                    Debug.Log("Attemping ATT request");

                    if (!gotResponse)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }

                }
                else
                    Debug.Log("ATT is aprooved. Skip request");
            }
            else
                Debug.LogWarning("iOS version below 14.5 ==> skipping att request");

            Debug.Log($"Current IDFA status: {GetCurrentStatus()}");
#endif
            yield return null;
        }

        public static void RequestTracking(RequestDelegate on_complete)
        {
            GetPlatform().RequestTracking(on_complete);
        }

        public static Status GetCurrentStatus()
        {
            return GetPlatform().GetCurrentStatus();
        }

        static IDFAPlatform impl;

        static IDFAPlatform GetPlatform()
        {
            if (impl != null)
                return impl;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
                impl = new IDFAPlatformIOS();
            else
                impl = new IDFAPlatformMock();

            return impl;
        }
    }

    internal class IDFAPlatformMock : IDFAPlatform
    {
        public void RequestTracking(RequestDelegate on_complete)
        {
            on_complete?.Invoke(GetCurrentStatus());
        }

        public Status GetCurrentStatus()
        {
            return Status.Authorized;
        }
    }

    internal class IDFAPlatformIOS : IDFAPlatform
    {
        const string IOS_ATTrackingManagerAuthorizationStatusNotDetermined = "0";
        const string IOS_ATTrackingManagerAuthorizationStatusRestricted = "1";
        const string IOS_ATTrackingManagerAuthorizationStatusDenied = "2";
        const string IOS_ATTrackingManagerAuthorizationStatusAuthorized = "3";
        const string IOS_CustomUnsupportedVersion = "UnsupportedVersion";

        RequestDelegate pending_request;
        IDFAPlatformIOSImpl impl;

        public void RequestTracking(RequestDelegate on_complete)
        {
            pending_request = on_complete;
            GetImpl().Request();
        }

        IDFAPlatformIOSImpl GetImpl()
        {
            if (impl != null)
                return impl;

            var impl_go = new GameObject();
            impl = impl_go.AddComponent<IDFAPlatformIOSImpl>();
            impl.OnNewAuthStatus += OnNewAuthStatus;

            impl_go.name = impl.GetType().Name;
            UnityEngine.Object.DontDestroyOnLoad(impl_go);

            return impl;
        }

        void OnNewAuthStatus(string os_status)
        {
            Status status = FromOSStatus(os_status);
            pending_request?.Invoke(status);
        }

        static Status FromOSStatus(string os_status)
        {
            if (os_status == IOS_ATTrackingManagerAuthorizationStatusNotDetermined)
                return Status.NotDetermined;

            if (os_status == IOS_ATTrackingManagerAuthorizationStatusRestricted)
                return Status.Restricted;

            if (os_status == IOS_ATTrackingManagerAuthorizationStatusDenied)
                return Status.Denied;

            if (os_status == IOS_ATTrackingManagerAuthorizationStatusAuthorized ||
               os_status == IOS_CustomUnsupportedVersion)
                return Status.Authorized;

            return Status.NotDetermined;
        }

        public Status GetCurrentStatus()
        {
            string os_status = GetImpl().GetCurrentStatus();
            return FromOSStatus(os_status);
        }
    }

    internal class IDFAPlatformIOSImpl : MonoBehaviour
    {
        public System.Action<string> OnNewAuthStatus;

        public void OnTrackingAuthorizationComplete(string status)
        {
            OnNewAuthStatus?.Invoke(status);
        }

        public void Request()
        {
#if UNITY_IOS
    IDFATrackingRequestAuthorization();
#endif
        }

        public string GetCurrentStatus()
        {
#if UNITY_IOS
    return IDFATRackingGetCurrentStatus();
#else
            return "0";
#endif

        }

#if UNITY_IOS
  [DllImport("__Internal")]
  private static extern void IDFATrackingRequestAuthorization();
  
  [DllImport("__Internal")]
  private static extern string IDFATRackingGetCurrentStatus();
#endif
    }

}
