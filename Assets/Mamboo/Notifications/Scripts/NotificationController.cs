using System;
using System.Linq;
using UnityEngine;
using static Mamboo.Internal.Notifications.NotificationSettings;
using System.Collections;
using Mamboo.Internal.Scripts;

#if UNITY_ANDROID && !UNITY_EDITOR
using Unity.Notifications.Android;

#endif

#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

namespace Mamboo.Internal.Notifications
{
    public class NotificationController : Singleton<NotificationController>
    {
        [SerializeField] NotificationSettings NotificationSettings;

        private static DateTime Now => DateTime.Now;

        private static DateTime NotificationTime = new DateTime(Now.Year, Now.Month, Now.Day, 20, 0, 0);

        public static void SetNotificationTime(string notificationTime)
        {
            if (string.IsNullOrWhiteSpace(notificationTime))
                throw new ArgumentNullException("Notification time is empty");

            if (!notificationTime.Contains(":") || notificationTime.Count(c => c.Equals(':')) > 1)
                throw new ArgumentNullException("Notification time format is not valid");

            var newTime = notificationTime.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            var hour = int.Parse(newTime[0]);
            var minute = int.Parse(newTime[1]);

            NotificationTime = new DateTime(Now.Year, Now.Month, Now.Day, hour, minute, 0);
        }

        public IEnumerator RegisterNotifications()
        {
#if !UNITY_STANDALONE && !UNITY_EDITOR
        Init();
#endif
            yield return null;
        }

        #if !UNITY_EDITOR

#if UNITY_IOS

        private IEnumerator RequestTrackingPermission()
        {
            using (var req = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound, false))
            {
                while (!req.IsFinished)
                {
                    yield return null;
                };

                string result = "\n RequestAuthorization: \n";
                result += "\n finished: " + req.IsFinished;
                result += "\n granted :  " + req.Granted;
                result += "\n error:  " + req.Error;
                result += "\n deviceToken:  " + req.DeviceToken;
                Debug.Log(result);
            }

        }

        private void Init()
        {
            StartCoroutine(RequestTrackingPermission());

            if (iOSNotificationCenter.GetNotificationSettings().AuthorizationStatus != AuthorizationStatus.Authorized)
                return;

            if (iOSNotificationCenter.GetScheduledNotifications().Length != (NotificationSettings.Notifications.Length))
            {
                iOSNotificationCenter.RemoveAllScheduledNotifications();

                ScheduleNotifications();
            }
        }

        private void ScheduleNotifications()
        {
            var now = DateTime.Now;

            ScheduleMessageSequenceWithExactTime(NotificationSettings.Notifications, new Func<DateTime>(() => NotificationTime));

            //ScheduleRandomTimeMessages(NotificationSettings.SecondNotifications,
            //    new Func<DateTime>(() => new DateTime(now.Year, now.Month, now.Day, UnityEngine.Random.Range(18, 20), UnityEngine.Random.Range(0, 60), 0)));
        }

        private void ScheduleMessageSequenceWithExactTime(NotificationDescription[] notifications, Func<DateTime> notificationTime)
        {
            if (notifications == null || !notifications.Any())
                return;

            int firstDay = 0;
            if (Now > notificationTime())
                firstDay++;

            var scheduled = iOSNotificationCenter.GetScheduledNotifications();

            var notificationsList = notifications.ToList();

            foreach (var message in notificationsList)
            {
                // message Day starts from 1
                var day = firstDay + notificationsList.IndexOf(message);
                var messageId = $"notification_{day}";

                if (scheduled.FirstOrDefault(n => n.Identifier == messageId) != null)
                {
                    continue;
                }

                var date = Now.AddDays(day);
                var time = notificationTime();

                var trigger = new iOSNotificationCalendarTrigger
                {
                    Repeats = false,

                    Year = date.Year,
                    Month = date.Month,
                    Day = date.Day,
                    Hour = time.Hour,
                    Minute = time.Minute
                };

                var notification = new iOSNotification
                {
                    Identifier = messageId,
                    Title = message.Title,
                    Body = message.Message,
                    ShowInForeground = false,
                    ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound | PresentationOption.Badge,
                    CategoryIdentifier = "reminders",
                    ThreadIdentifier = "main_thread",
                    Trigger = trigger
                };

                iOSNotificationCenter.ScheduleNotification(notification);
            }
        }

        private void ScheduleRandomTimeMessages(NotificationDescription[] notifications, Func<DateTime> notificationTime)
        {
            if (notifications == null || !notifications.Any())
                return;

            var now = DateTime.Now;

            int day = 0;
            if (now > notificationTime())
                day++;

            var scheduled = iOSNotificationCenter.GetScheduledNotifications();

            var notificationsList = notifications.ToList();

            foreach (var message in notificationsList)
            {
                var messageId = $"notification_{day}";

                if (scheduled.FirstOrDefault(n => n.Identifier == messageId) != null)
                {
                    day++;
                    continue;
                }

                var date = now.AddDays(day);
                var time = notificationTime();

                var trigger = new iOSNotificationCalendarTrigger
                {
                    Repeats = false,

                    Year = date.Year,
                    Month = date.Month,
                    Day = date.Day,
                    Hour = time.Hour,
                    Minute = time.Minute
                };

                var notification = new iOSNotification
                {
                    Identifier = messageId,
                    Title = message.Title,
                    Body = message.Message,
                    ShowInForeground = false,
                    ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound | PresentationOption.Badge,
                    CategoryIdentifier = "reminders",
                    ThreadIdentifier = "main_thread",
                    Trigger = trigger
                };

                iOSNotificationCenter.ScheduleNotification(notification);
                day++;
            }
        }
#endif

#if UNITY_ANDROID

    private string NotificationsChannelName => Application.identifier.Replace(".", "") +"_channel";

    public AndroidNotificationChannel defaultNotificationChannel;

    private void Init()
    {
        defaultNotificationChannel = AndroidNotificationCenter.GetNotificationChannel(NotificationsChannelName);

        if (string.IsNullOrEmpty(defaultNotificationChannel.Id))
        {
            defaultNotificationChannel = new AndroidNotificationChannel
            {
                Id = NotificationsChannelName,
                Name = Application.productName + " Channel",
                Description = "For Generic notifications",
                Importance = Importance.Default
            };

            AndroidNotificationCenter.RegisterNotificationChannel(defaultNotificationChannel);
        }

        ScheduleNotifications();
    }

    private void ScheduleNotifications()
    {
        var now = DateTime.Now;

        ScheduleMessageSequenceWithExactTime(NotificationSettings.Notifications, new Func<DateTime>(() => NotificationTime));
    }

    private void ScheduleMessageSequenceWithExactTime(NotificationDescription[] notifications, Func<DateTime> notificationTime)
    {
        if (notifications == null || !notifications.Any())
            return;

        int firstDay = 0;
        if (Now > notificationTime())
            firstDay++;

        var notificationsList = notifications.ToList();

        foreach (var message in notificationsList)
        {
            // message Day starts from 1
            var day = firstDay + notificationsList.IndexOf(message);

            var notificationKey = $"{NotificationsChannelName}_{day}";

            var existingId = PlayerPrefs.GetInt(notificationKey);

            if (existingId != default)
            {
                var delieveryStatus = AndroidNotificationCenter.CheckScheduledNotificationStatus(existingId);

                if (delieveryStatus == NotificationStatus.Scheduled)
                {
                    day++;
                    continue;
                }
                else
                {
                    PlayerPrefs.DeleteKey(notificationKey);
                }
            }

            var date = Now.AddDays(day);
            var time = notificationTime();

            var fireDateTime = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);

            var notification = new AndroidNotification
            {
                Title = message.Title,
                Text = message.Message,
                FireTime = fireDateTime
            };

            var id = AndroidNotificationCenter.SendNotification(notification, NotificationsChannelName);

            PlayerPrefs.SetInt(notificationKey, id);
        }
    }

#endif

#endif

    }
}

