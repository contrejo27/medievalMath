using UnityEngine;
using System.Collections;

namespace EasyMobile
{
    [System.Serializable]
    public partial class GameServicesSettings
    {
        public bool IsGPGSDebug { get { return _gpgsDebugLog; } set { _gpgsDebugLog = value; } }

        public bool IsAutoInit { get { return _autoInit; } }

        public float AutoInitDelay { get { return _autoInitDelay; } }

        public int AndroidMaxLoginRequests { get { return _androidMaxLoginRequests; } }

        public Leaderboard[] Leaderboards { get { return _leaderboards; } }

        public Achievement[] Achievements { get { return _achievements; } }

        // GPGS setup.
        [SerializeField]
        private bool _gpgsDebugLog = false;

        // Auto-init config
        [SerializeField]
        private bool _autoInit = true;
        [SerializeField]
        private float _autoInitDelay = 0f;
        [SerializeField]
        private int _androidMaxLoginRequests = 3;

        // Leaderboards & Achievements
        [SerializeField]
        private Leaderboard[] _leaderboards;
        [SerializeField]
        private Achievement[] _achievements;

        // GPGS setup resources.
        // These fields are only used in our editor, hence the warning suppression.
        #pragma warning disable 0414
        [SerializeField]
        private string _androidXmlResources = string.Empty;
        #pragma warning restore 0414
    }
}

