using UnityEngine;
using System;
using System.Collections;

namespace EasyMobile.AdvertisingInternal
{
    #if UNITY_ADS
    using UnityEngine.Advertisements;
    #endif

    internal class UnityAdsClientImpl : IAdClient
    {
        private const string NO_SDK_MESSAGE = "SDK missing. Please enable UnityAds service.";

        #pragma warning disable 0067
        public event Action<InterstitialAdNetwork, AdLocation> InterstitialAdCompleted;

        public event Action<RewardedAdNetwork, AdLocation> RewardedAdSkipped;

        public event Action<RewardedAdNetwork, AdLocation> RewardedAdCompleted;
        #pragma warning restore 0067

        // Singleton.
        private static UnityAdsClientImpl sInstance;

        #if UNITY_ADS
        private const string DEFAULT_VIDEO_ZONE_ID = "video";
        private const string DEFAULT_REWARDED_ZONE_ID = "rewardedVideo";
        #endif

        #region Object Creators

        private UnityAdsClientImpl()
        {
        }

        /// <summary>
        /// Creates and initializes the singleton client with the provided settings.
        /// </summary>
        /// <returns>The client.</returns>
        /// <param name="settings">Settings.</param>
        public static UnityAdsClientImpl CreateClient(AdSettings settings)
        {
            if (sInstance == null)
            {
                sInstance = new UnityAdsClientImpl();
                sInstance.Init(settings);
            }
            return sInstance;
        }

        #endregion  // Object Creators

        #region IAdClient Implementation

        //------------------------------------------------------------
        // General.
        //------------------------------------------------------------

        public AdNetwork Network { get { return AdNetwork.UnityAds; } }

        public bool SupportBannerAds { get { return false; } }

        public bool SupportInterstitialAds { get { return true; } }

        public bool SupportRewardedAds { get { return true; } }

        //------------------------------------------------------------
        // Init.
        //------------------------------------------------------------

        public void Init(AdSettings settings)
        {
            // Initialization is done automatically by Unity.
        }

        public bool IsReady()
        {
            return true;
        }

        //------------------------------------------------------------
        // Banner Ads.
        //------------------------------------------------------------

        public void ShowBannerAd(BannerAdPosition position, BannerAdSize size)
        {
            Debug.LogWarning("UnityAds does not support banner ad format.");
        }

        public void HideBannerAd()
        {
            Debug.LogWarning("UnityAds does not support banner ad format.");
        }

        public void DestroyBannerAd()
        {
            Debug.LogWarning("UnityAds does not support banner ad format.");
        }

        //------------------------------------------------------------
        // Interstitial Ads.
        //------------------------------------------------------------

        public void LoadInterstitialAd(AdLocation location)
        {
            // Unity Ads are loaded automatically if enabled.
            #if !UNITY_ADS
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        public bool IsInterstitialAdReady(AdLocation location)
        {
            #if UNITY_ADS
            return Advertisement.IsReady(
                location == AdLocation.Default ? DEFAULT_VIDEO_ZONE_ID : location.ToUnityAdsZoneId()
            );
            #else
            return false;
            #endif
        }

        public void ShowInterstitialAd(AdLocation location)
        {
            #if UNITY_ADS
            if (IsInterstitialAdReady(location))
            {
                var showOptions = new ShowOptions
                { resultCallback = (result) =>
                    {
                        InterstitialAdCallback(result, location);
                    }
                };
                Advertisement.Show(
                    location == AdLocation.Default ? DEFAULT_VIDEO_ZONE_ID : location.ToUnityAdsZoneId(), 
                    showOptions
                );
            }
            else
            {
                Debug.Log("Could not show UnityAds interstitial ad: ad is not loaded.");
            }
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        //------------------------------------------------------------
        // Rewarded Ads.
        //------------------------------------------------------------

        public void LoadRewardedAd(AdLocation location)
        {
            // Unity Ads are loaded automatically if enabled.
            #if !UNITY_ADS
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        public bool IsRewardedAdReady(AdLocation location)
        {
            #if UNITY_ADS
            return Advertisement.IsReady(
                location == AdLocation.Default ? DEFAULT_REWARDED_ZONE_ID : location.ToUnityAdsZoneId()
            );
            #else
            return false;
            #endif
        }

        public void ShowRewardedAd(AdLocation location)
        {
            #if UNITY_ADS
            if (IsRewardedAdReady(location))
            {
                var showOptions = new ShowOptions
                { resultCallback = (result) =>
                    {
                        RewardedAdCallback(result, location);
                    }
                };
                Advertisement.Show(
                    location == AdLocation.Default ? DEFAULT_REWARDED_ZONE_ID : location.ToUnityAdsZoneId(), 
                    showOptions
                );
            }
            else
            {
                Debug.Log("Could not show UnityAds rewarded ad: ad is not loaded.");
            }
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        #endregion  // IAdClient Implementation

        #region Ad Event Handlers

        #if UNITY_ADS
        void InterstitialAdCallback(ShowResult result, AdLocation location)
        {
            switch (result)
            {
                case ShowResult.Finished:
                    if (InterstitialAdCompleted != null)
                        InterstitialAdCompleted(InterstitialAdNetwork.UnityAds, location);
                    break;
                case ShowResult.Skipped:
                    if (InterstitialAdCompleted != null)
                        InterstitialAdCompleted(InterstitialAdNetwork.UnityAds, location);
                    break;
                case ShowResult.Failed:
                    break;
            }
        }

        void RewardedAdCallback(ShowResult result, AdLocation location)
        {
            switch (result)
            {
                case ShowResult.Finished:
                    if (RewardedAdCompleted != null)
                        RewardedAdCompleted(RewardedAdNetwork.UnityAds, location);
                    break;
                case ShowResult.Skipped:
                    if (RewardedAdSkipped != null)
                        RewardedAdSkipped(RewardedAdNetwork.UnityAds, location);
                    break;
                case ShowResult.Failed:
                    break;
            }
        }
        #endif

        #endregion  // Ad Event Handlers
    }
}