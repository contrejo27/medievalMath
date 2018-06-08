using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EasyMobile.AdvertisingInternal
{
    #if EM_ADCOLONY
    using EM_AdColony = AdColony;
    #endif

    internal class AdColonyClientImpl : IAdClient
    {
        private const string NO_SDK_MESSAGE = "SDK missing. Please import the AdColony plugin.";

        #pragma warning disable 0067
        public event Action<InterstitialAdNetwork, AdLocation> InterstitialAdCompleted;

        public event Action<RewardedAdNetwork, AdLocation> RewardedAdSkipped;

        public event Action<RewardedAdNetwork, AdLocation> RewardedAdCompleted;
        #pragma warning restore 0067

        // Singleton.
        private static AdColonyClientImpl sInstance;

        #if EM_ADCOLONY
        private bool isInitialized = false;
        private AdSettings globalAdSettings = null;
        private EM_AdColony.InterstitialAd interstitialAd = null;
        private EM_AdColony.InterstitialAd rewardedAd = null;
        private bool isRewardedAdCompleted = false;
        #endif

        #region Object Creators

        private AdColonyClientImpl()
        {
        }

        /// <summary>
        /// Creates and initializes the singleton client with the provided settings.
        /// </summary>
        /// <returns>The client.</returns>
        /// <param name="settings">Settings.</param>
        public static AdColonyClientImpl CreateClient(AdSettings settings)
        {
            if (sInstance == null)
            {
                sInstance = new AdColonyClientImpl();
                sInstance.Init(settings);
            }
            return sInstance;
        }

        #endregion  // Object Creators

        #region IAdClient Implementation

        //------------------------------------------------------------
        // General Info.
        //------------------------------------------------------------
        public AdNetwork Network { get { return AdNetwork.AdColony; } }

        public bool SupportBannerAds { get { return false; } }

        public bool SupportInterstitialAds { get { return true; } }

        public bool SupportRewardedAds { get { return true; } }

        //------------------------------------------------------------
        // Init.
        //------------------------------------------------------------

        public void Init(AdSettings settings)
        {
            #if EM_ADCOLONY
            if (isInitialized)
            {
                Debug.Log("AdColony client is already initialized. Ignoring this call.");
                return;
            }

            // Store a reference to the global settings.
            globalAdSettings = settings;

            // Subscribe ad events.
            EM_AdColony.Ads.OnConfigurationCompleted += OnConfigurationCompleted;
            EM_AdColony.Ads.OnRequestInterstitial += OnRequestAdCompleted;
            EM_AdColony.Ads.OnRequestInterstitialFailedWithZone += OnRequestAdFailedWithZone;
            EM_AdColony.Ads.OnOpened += OnAdOpened;
            EM_AdColony.Ads.OnClosed += OnAdClosed;
            EM_AdColony.Ads.OnExpiring += OnAdExpiring;
            EM_AdColony.Ads.OnLeftApplication += OnLeftApplication;
            EM_AdColony.Ads.OnClicked += OnAdClicked;
            EM_AdColony.Ads.OnRewardGranted += OnRewardGranted;

            // Configure AdColony.
            var adColonyIds = globalAdSettings.AdColonyIds;
            var appOptions = new EM_AdColony.AppOptions()
            {
                AdOrientation = ToAdColonyAdOrientation(globalAdSettings.AdColonyAdOrientation)
            };

            EM_AdColony.Ads.Configure(
                adColonyIds.AppId,
                appOptions,
                adColonyIds.InterstitialAdId,
                adColonyIds.RewardedAdId
            );
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        public bool IsReady()
        {
            #if EM_ADCOLONY
            return isInitialized;
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            return false;
            #endif
        }

        //------------------------------------------------------------
        // Banner Ads.
        //------------------------------------------------------------

        public void ShowBannerAd(BannerAdPosition position, BannerAdSize size)
        {
            Debug.LogWarning("AdColony does not support banner ad format.");
        }

        public void HideBannerAd()
        {
            Debug.LogWarning("AdColony does not support banner ad format.");
        }

        public void DestroyBannerAd()
        {
            Debug.LogWarning("AdColony does not support banner ad format.");
        }

        //------------------------------------------------------------
        // Interstitial Ads.
        //------------------------------------------------------------

        public void LoadInterstitialAd(AdLocation location)
        {
            #if EM_ADCOLONY
            if (!isInitialized)
                return;

            // Do nothing if an interstitial ad is already loaded.
            if (interstitialAd != null && !interstitialAd.Expired)
                return;

            EM_AdColony.Ads.RequestInterstitialAd(globalAdSettings.AdColonyIds.InterstitialAdId, null);
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        public bool IsInterstitialAdReady(AdLocation location)
        {
            #if EM_ADCOLONY
            return (interstitialAd != null && !interstitialAd.Expired);
            #else
            return false;
            #endif
        }

        public void ShowInterstitialAd(AdLocation location)
        {
            #if EM_ADCOLONY
            if (interstitialAd != null && !interstitialAd.Expired)
                EM_AdColony.Ads.ShowAd(interstitialAd);
            else
                Debug.Log("Could not show AdColony interstitial ad: ad is not loaded or has expired.");
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        //------------------------------------------------------------
        // Rewarded Ads.
        //------------------------------------------------------------

        public void LoadRewardedAd(AdLocation location)
        {
            #if EM_ADCOLONY
            if (!isInitialized)
                return;

            // Do nothing if a rewarded ad is already loaded.
            if (rewardedAd != null && !rewardedAd.Expired)
                return;

            var adOptions = new EM_AdColony.AdOptions()
            { 
                ShowPrePopup = globalAdSettings.AdColonyShowRewardedAdPrePopup,
                ShowPostPopup = globalAdSettings.AdColonyShowRewardedAdPostPopup
            };

            EM_AdColony.Ads.RequestInterstitialAd(globalAdSettings.AdColonyIds.RewardedAdId, adOptions);
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        public bool IsRewardedAdReady(AdLocation location)
        {
            #if EM_ADCOLONY
            return (rewardedAd != null && !rewardedAd.Expired);
            #else
            return false;
            #endif
        }

        public void ShowRewardedAd(AdLocation location)
        {
            #if EM_ADCOLONY
            if (rewardedAd != null && !rewardedAd.Expired)
                EM_AdColony.Ads.ShowAd(rewardedAd);
            else
                Debug.Log("Could not show AdColony rewarded ad: ad is not loaded or has expired.");
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        #endregion // IAdClient Implementation

        #region Private Methods

        #if EM_ADCOLONY
        private static EM_AdColony.AdOrientationType ToAdColonyAdOrientation(AdSettings.AdOrientation orientation)
        {
            switch (orientation)
            {
                case AdSettings.AdOrientation.AdOrientationLandscape:
                    return EM_AdColony.AdOrientationType.AdColonyOrientationLandscape;
                case AdSettings.AdOrientation.AdOrientationPortrait:
                    return EM_AdColony.AdOrientationType.AdColonyOrientationPortrait;
                case AdSettings.AdOrientation.AdOrientationAll:
                    return EM_AdColony.AdOrientationType.AdColonyOrientationAll;
                default:
                    return EM_AdColony.AdOrientationType.AdColonyOrientationAll;
            }
        }
        #endif

        #endregion // Private Methods

        #region Ad Event Handlers

        #if EM_ADCOLONY
        private void OnConfigurationCompleted(List<EM_AdColony.Zone> obj)
        {
            isInitialized = true;
            Debug.Log("AdColony client has been initialized.");
        }

        private void OnRequestAdFailedWithZone(string zoneId)
        {
            Debug.Log("AdColony request ad failed with zoneId: " + zoneId);
        }

        private void OnRequestAdCompleted(EM_AdColony.InterstitialAd ad)
        {
            Debug.Log("AdColony successfully loaded ad with zoneId: " + ad.ZoneId);
            var adColonyIds = globalAdSettings.AdColonyIds;

            if (ad.ZoneId.Equals(adColonyIds.InterstitialAdId))
                interstitialAd = ad;
            else if (ad.ZoneId.Equals(adColonyIds.RewardedAdId))
                rewardedAd = ad;
        }

        private void OnAdOpened(EM_AdColony.InterstitialAd ad)
        {
        }

        private void OnAdClosed(EM_AdColony.InterstitialAd ad)
        {
            var adColonyIds = globalAdSettings.AdColonyIds;

            if (ad.ZoneId.Equals(adColonyIds.InterstitialAdId))
            {
                if (InterstitialAdCompleted != null)
                    InterstitialAdCompleted(InterstitialAdNetwork.AdColony, AdLocation.Default);
            }
            else if (ad.ZoneId.Equals(adColonyIds.RewardedAdId))
            {
                if (isRewardedAdCompleted)
                {
                    isRewardedAdCompleted = false;

                    if (RewardedAdCompleted != null)
                        RewardedAdCompleted(RewardedAdNetwork.AdColony, AdLocation.Default);
                }
                else
                {
                    if (RewardedAdSkipped != null)
                        RewardedAdSkipped(RewardedAdNetwork.AdColony, AdLocation.Default);
                }
            }
        }

        private void OnAdExpiring(EM_AdColony.InterstitialAd ad)
        {
        }

        private void OnLeftApplication(EM_AdColony.InterstitialAd ad)
        {
        }

        private void OnAdClicked(EM_AdColony.InterstitialAd ad)
        {
        }

        private void OnRewardGranted(string zoneId, bool success, string rewardName, int rewardQuantity)
        {
            var adColonyIds = globalAdSettings.AdColonyIds;

            if (zoneId.Equals(adColonyIds.RewardedAdId))
                isRewardedAdCompleted = true;
        }
        #endif

        #endregion  // Ad Event Handlers
    }
}