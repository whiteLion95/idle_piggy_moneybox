using System;
using UnityEngine;

namespace Mamboo.Internal.Notifications
{
    [CreateAssetMenu(menuName = "NotificationSettings", fileName = "NotificationSettings")]
    public class NotificationSettings : ScriptableObject
    {
        [Serializable]
        public class NotificationDescription
        {
            [SerializeField]
            private string _title;

            [SerializeField]
            private string _message;

            public string Title => _title;
            public string Message => _message;
        }

        [SerializeField]
        private NotificationDescription[] _notifications;

        public NotificationDescription[] Notifications => _notifications;
    }
}
