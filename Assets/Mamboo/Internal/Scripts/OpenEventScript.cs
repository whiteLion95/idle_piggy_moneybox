using System;
using com.adjust.sdk;
using Mamboo.Analytics.Adjust.Internal;
using UnityEngine;

namespace Mamboo.Internal.Scripts
{
    public class OpenEventScript : MonoBehaviour
    {
        private void Start()
        {
            var openAppEvent = new AdjustEvent(AdjustConstants.StartAppEventId);
            Adjust.trackEvent(openAppEvent);
            Debug.Log($"[Mamboo SDK] App open event tracked");
            DontDestroyOnLoad(this);
            
            RetentionEvent();
        }

        private static void RetentionEvent()
        {
            // try to get the launch date saved as a string:
            var savedDate = PlayerPrefs.GetString("LaunchDate", "");
            if (savedDate == "")
            { 
                // if not saved yet...
                // convert current date to string...
                savedDate = DateTime.Now.ToString();
                // and save it in PlayerPrefs as LaunchDate:
                PlayerPrefs.SetString("LaunchDate", savedDate);
            }
            // at this point, the string savedDate contains the launch date
            // let's convert it to DateTime:
            DateTime.TryParse(savedDate, out var launchDate);
            // get current DateTime:
            var now = DateTime.Now;
            // calculate days ellapsed since launch date:
            var days = (now-launchDate).Days;
            if (days != 1) return;
            
            var adjustEvent = new AdjustEvent(AdjustConstants.Retention);
            Adjust.trackEvent(adjustEvent);
            Debug.Log($"[Mamboo SDK] Retention event tracked");
        }
    }
}
