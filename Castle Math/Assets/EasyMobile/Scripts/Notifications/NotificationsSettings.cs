using UnityEngine;
using System;
using System.Collections;
using SgLib.Attributes;

namespace EasyMobile
{
    [System.Serializable]
    public class NotificationsSettings
    {
        public const string DEFAULT_CATEGORY_ID = "notification.category.default";
        public const string DEFAULT_CATEGORY_NAME = "Default";

        public bool IsAutoInit { get { return _autoInit; } }

        public float AutoInitDelay { get { return _autoInitDelay; } }

        public NotificationAuthOptions iOSAuthOptions { get { return _iosAuthOptions; } }

        public PushNotificationProvider PushNotificationService { get { return _pushNotificationService; } }

        public string OneSignalAppId { get { return _oneSignalAppId; } }

        public NotificationCategoryGroup[] CategoryGroups { get { return _categoryGroups; } }

        public NotificationCategory DefaultCategory { get { return _defaultCategory; } }

        public NotificationCategory[] UserCategories { get { return _userCategories; } }

        // Initialization config
        [SerializeField]
        private bool _autoInit = true;
        [SerializeField]
        private float _autoInitDelay = 0f;

        // iOS authorization options
        [SerializeField][EnumFlags]
        private NotificationAuthOptions _iosAuthOptions = NotificationAuthOptions.Alert | NotificationAuthOptions.Badge | NotificationAuthOptions.Sound;

        // Remote notification settings
        [SerializeField]
        private PushNotificationProvider _pushNotificationService = PushNotificationProvider.None;

        [SerializeField]
        private string _oneSignalAppId;

        // Category groups
        [SerializeField]
        private NotificationCategoryGroup[] _categoryGroups;

        // Default notification category
        [SerializeField]
        private NotificationCategory _defaultCategory = new NotificationCategory()
        {
            id = DEFAULT_CATEGORY_ID,
            name = DEFAULT_CATEGORY_NAME
        };

        // User categories
        [SerializeField]
        private NotificationCategory[] _userCategories;

        public NotificationCategory GetCategoryWithId(string categoryId)
        {
            if (!string.IsNullOrEmpty(categoryId))
            {
                if (categoryId.Equals(DefaultCategory.id))
                {
                    return DefaultCategory;
                }
                else if (UserCategories != null)
                {
                    foreach (var c in UserCategories)
                    {
                        if (categoryId.Equals(c.id))
                            return c;
                    }
                }
            }

            return null;
        }
    }

    public enum PushNotificationProvider
    {
        None,
        OneSignal
    }

    [Flags]
    public enum NotificationAuthOptions
    {
        Alert = 1 << 0,
        Badge = 1 << 1,
        Sound = 1 << 2,
    }
}

