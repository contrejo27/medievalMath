using UnityEngine;
using System.Collections;
using EasyMobile.Internal;
using System;
using System.Collections.Generic;

namespace EasyMobile
{
    [System.Serializable]
    public class AdSettings
    {
        public AdColonyConfig AdColonyIds
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                    return _androidAdColonyConfig;
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                    return _iosAdColonyConfig;
                else
                    return new AdColonyConfig();
            }
        }

        public bool AdColonyShowRewardedAdPrePopup { get { return _adColonyShowRewardedAdPrePopup; } }

        public bool AdColonyShowRewardedAdPostPopup { get { return _adColonyShowRewardedAdPostPopup; } }

        public AdOrientation AdColonyAdOrientation { get { return _adColonyAdOrientation; } }

        public AdMobConfig AdMobIds
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                    return _androidAdMobConfig;
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                    return _iosAdMobConfig;
                else
                    return new AdMobConfig();
            }
        }

        public AdMobTargetingSettings AdMobTargeting
        { 
            get { return _admobTargeting; } 
            set
            {
                if (value != null)
                    _admobTargeting = value;
            }
        }

        public bool AdMobEnableTestMode { get { return _admobEnableTestMode; } }

        public string[] AdMobTestDeviceIds { get { return _admobTestDeviceIds; } }

        public string HeyzapPublisherId { get { return Helper.AutoTrimId(_heyzapPublisherId); } }

        public bool HeyzapShowTestSuite { get { return _heyzapShowTestSuite; } }

        public bool IsAutoLoadDefaultAds { get { return _autoLoadDefaultAds; } set { _autoLoadDefaultAds = value; } }

        public float AdCheckingInterval { get { return _adCheckingInterval; } set { _adCheckingInterval = value; } }

        public float AdLoadingInterval { get { return _adLoadingInterval; } set { _adLoadingInterval = value; } }

        public DefaultAdNetworks IosDefaultAdNetworks { get { return _iosDefaultAdNetworks; } }

        public DefaultAdNetworks AndroidDefaultAdNetworks { get { return _androidDefaultAdNetwork; } }

        // AdColony config
        [SerializeField]
        private AdColonyConfig _iosAdColonyConfig;
        [SerializeField]
        private AdColonyConfig _androidAdColonyConfig;
        [SerializeField]
        private AdOrientation _adColonyAdOrientation = AdOrientation.AdOrientationAll;
        [SerializeField]
        private bool _adColonyShowRewardedAdPrePopup;
        [SerializeField]
        private bool _adColonyShowRewardedAdPostPopup;

        // AdMob config
        [SerializeField]
        private AdMobConfig _iosAdMobConfig;
        [SerializeField]
        private AdMobConfig _androidAdMobConfig;
        [SerializeField]
        private AdMobTargetingSettings _admobTargeting;
        [SerializeField]
        private bool _admobEnableTestMode;
        [SerializeField]
        private string[] _admobTestDeviceIds;

        // Heyzap config
        [SerializeField]
        private string _heyzapPublisherId;
        [SerializeField]
        private bool _heyzapShowTestSuite;

        // Automatic ad-loading config
        [SerializeField]
        private bool _autoLoadDefaultAds = true;
        [SerializeField]
        private float _adCheckingInterval = 10f;
        [SerializeField]
        private float _adLoadingInterval = 20f;

        // Default ad networks
        [SerializeField]
        private DefaultAdNetworks _iosDefaultAdNetworks = new DefaultAdNetworks(BannerAdNetwork.None, InterstitialAdNetwork.None, RewardedAdNetwork.None);
        [SerializeField]
        private DefaultAdNetworks _androidDefaultAdNetwork = new DefaultAdNetworks(BannerAdNetwork.None, InterstitialAdNetwork.None, RewardedAdNetwork.None);

        [System.Serializable]
        public struct DefaultAdNetworks
        {
            public BannerAdNetwork bannerAdNetwork;
            public InterstitialAdNetwork interstitialAdNetwork;
            public RewardedAdNetwork rewardedAdNetwork;

            public DefaultAdNetworks(BannerAdNetwork banner, InterstitialAdNetwork interstitial, RewardedAdNetwork rewarded)
            {
                bannerAdNetwork = banner;
                interstitialAdNetwork = interstitial;
                rewardedAdNetwork = rewarded;
            }
        }

        [System.Serializable]
        public class AdColonyConfig
        {
            public string AppId { get { return Helper.AutoTrimId(_appId); } }

            public string InterstitialAdId { get { return Helper.AutoTrimId(_interstitialAdId); } }

            public string RewardedAdId { get { return Helper.AutoTrimId(_rewardedAdId); } }

            [SerializeField]
            private string _appId;
            [SerializeField]
            private string _interstitialAdId;
            [SerializeField]
            private string _rewardedAdId;
        }

        [System.Serializable]
        public class AdMobConfig
        {
            public string AppId { get { return Helper.AutoTrimId(_appId); } }

            public string BannerAdId { get { return Helper.AutoTrimId(_bannerAdId); } }

            public string InterstitialAdId { get { return Helper.AutoTrimId(_interstitialAdId); } }

            public string RewardedAdId { get { return Helper.AutoTrimId(_rewardedAdId); } }

            [SerializeField]
            private string _appId;
            [SerializeField]
            private string _bannerAdId;
            [SerializeField]
            private string _interstitialAdId;
            [SerializeField]
            private string _rewardedAdId;
        }

        [System.Serializable]
        public class AdMobTargetingSettings
        {
            public TargetGender gender = TargetGender.Unspecified;
            public bool setBirthday = false;
            public DateTime birthday;
            public ChildDirectedTreatmentOption tagForChildDirectedTreatment = ChildDirectedTreatmentOption.Unspecified;
            public Extra[] extras;

            [System.Serializable]
            public class Extra
            {
                public string key;
                public string value;

                public Extra(string key, string val)
                {
                    this.key = key;
                    this.value = val;
                }
            }
        }

        public enum AdOrientation
        {
            AdOrientationPortrait,
            AdOrientationLandscape,
            AdOrientationAll
        }

        public enum ChildDirectedTreatmentOption
        {
            Unspecified,
            Yes,
            No
        }

        public enum TargetGender
        {
            Unspecified,
            Male,
            Female
        }
    }

    // List of all supported ad networks
    public enum AdNetwork
    {
        None,
        AdColony,
        AdMob,
        Chartboost,
        Heyzap,
        UnityAds
    }

    public enum AdType
    {
        Banner,
        Interstitial,
        Rewarded
    }

    public enum BannerAdNetwork
    {
        None = AdNetwork.None,
        AdMob = AdNetwork.AdMob,
        Heyzap = AdNetwork.Heyzap
    }

    public enum InterstitialAdNetwork
    {
        None = AdNetwork.None,
        AdColony = AdNetwork.AdColony,
        AdMob = AdNetwork.AdMob,
        Chartboost = AdNetwork.Chartboost,
        Heyzap = AdNetwork.Heyzap,
        UnityAds = AdNetwork.UnityAds
    }

    public enum RewardedAdNetwork
    {
        None = AdNetwork.None,
        AdColony = AdNetwork.AdColony,
        AdMob = AdNetwork.AdMob,
        Chartboost = AdNetwork.Chartboost,
        Heyzap = AdNetwork.Heyzap,
        UnityAds = AdNetwork.UnityAds
    }
}

