using UnityEngine;
using System;
using System.Collections;

namespace EasyMobile.AdvertisingInternal
{
    #if EM_CHARTBOOST
    using ChartboostSDK;
    #endif

    internal class ChartboostClientImpl : IAdClient
    {
        private const string NO_SDK_MESSAGE = "SDK missing. Please import the Chartboost plugin.";

        #pragma warning disable 0067
        public event Action<InterstitialAdNetwork, AdLocation> InterstitialAdCompleted;

        public event Action<RewardedAdNetwork, AdLocation> RewardedAdSkipped;

        public event Action<RewardedAdNetwork, AdLocation> RewardedAdCompleted;
        #pragma warning restore 0067

        // Singleton.
        private static ChartboostClientImpl sInstance;

        #if EM_CHARTBOOST
        private bool isInitialized = false;
        private bool isCBRewardedAdCompleted;
        #endif

        #region Object Creators

        private ChartboostClientImpl()
        {
        }

        /// <summary>
        /// Creates and initializes the singleton client with the provided settings.
        /// </summary>
        /// <returns>The client.</returns>
        /// <param name="settings">Settings.</param>
        public static ChartboostClientImpl CreateClient(AdSettings settings)
        {
            if (sInstance == null)
            {
                sInstance = new ChartboostClientImpl();
                sInstance.Init(settings);
            }
            return sInstance;
        }

        #endregion  // Object Creators

        #region IAdClient Implementation

        //------------------------------------------------------------
        // General Info.
        //------------------------------------------------------------

        public AdNetwork Network { get { return AdNetwork.Chartboost; } }

        public bool SupportBannerAds { get { return false; } }

        public bool SupportInterstitialAds { get { return true; } }

        public bool SupportRewardedAds { get { return true; } }

        //------------------------------------------------------------
        // Init.
        //------------------------------------------------------------

        public void Init(AdSettings settings)
        {
            #if EM_CHARTBOOST
            if (isInitialized)
            {
                Debug.Log("Chartboost client is already initialized. Ignoring this call.");
                return;
            }

            Chartboost.didCacheInterstitial += CBDidCacheInterstitial;
            Chartboost.didClickInterstitial += CBDidClickInterstitial;
            Chartboost.didCloseInterstitial += CBDidCloseInterstitial;
            Chartboost.didDismissInterstitial += CBDidDismissInterstitial;
            Chartboost.didFailToLoadInterstitial += CBDidFailToLoadInterstitial;

            Chartboost.didCacheRewardedVideo += CBDidCacheRewardedVideo;
            Chartboost.didClickRewardedVideo += CBDidClickRewardedVideo;
            Chartboost.didCloseRewardedVideo += CBDidCloseRewardedVideo;
            Chartboost.didDismissRewardedVideo += CBDidDismissRewardedVideo;
            Chartboost.didFailToLoadRewardedVideo += CBDidFailToLoadRewardedVideo;
            Chartboost.didCompleteRewardedVideo += CBDidCompleteRewardedVideo;

            // Create Chartboost object. Even if Chartboost is not listed as one of default ad networks,
            // it should still be created so that we can use it with the API as an "undefault" network.
            // We'll also handle ad loading, so turning off Chartboost's autocache feature.
            Chartboost.setAutoCacheAds(false);
            Chartboost.Create();
            isInitialized = true;
            Debug.Log("Chartboost client has been initialized");
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        public bool IsReady()
        {
            #if EM_CHARTBOOST
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
            Debug.LogWarning("Chartboost does not support banner ad format.");
        }

        public void HideBannerAd()
        {
            Debug.LogWarning("Chartboost does not support banner ad format.");
        }

        public void DestroyBannerAd()
        {
            Debug.LogWarning("Chartboost does not support banner ad format.");
        }

        //------------------------------------------------------------
        // Interstitial Ads.
        //------------------------------------------------------------

        public void LoadInterstitialAd(AdLocation location)
        {
            #if EM_CHARTBOOST
            Chartboost.cacheInterstitial(location.ToChartboostLocation());
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        public bool IsInterstitialAdReady(AdLocation location)
        {
            #if EM_CHARTBOOST
            return Chartboost.hasInterstitial(location.ToChartboostLocation());
            #else
            return false;
            #endif
        }

        public void ShowInterstitialAd(AdLocation location)
        {   
            #if EM_CHARTBOOST
            if (IsInterstitialAdReady(location))
                Chartboost.showInterstitial(location.ToChartboostLocation());
            else
                Debug.Log("Could not show Chartboost interstitial ad: ad is not loaded.");
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        //------------------------------------------------------------
        // Rewarded Ads.
        //------------------------------------------------------------

        public void LoadRewardedAd(AdLocation location)
        {
            #if EM_CHARTBOOST
            Chartboost.cacheRewardedVideo(location.ToChartboostLocation());
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        public bool IsRewardedAdReady(AdLocation location)
        {
            #if EM_CHARTBOOST
            return Chartboost.hasRewardedVideo(location.ToChartboostLocation());
            #else
            return false;
            #endif
        }

        public void ShowRewardedAd(AdLocation location)
        {
            #if EM_CHARTBOOST
            if (IsRewardedAdReady(location))
                Chartboost.showRewardedVideo(location.ToChartboostLocation());
            else
                Debug.Log("Could not show Chartboost rewarded ad: ad is not loaded.");
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        #endregion  // IAdClient Implementation

        #region Ad Event Handlers

        #if EM_CHARTBOOST
        //------------------------------------------------------------
        // Interstitial Ad Events.
        //------------------------------------------------------------

        void CBDidCacheInterstitial(CBLocation location)
        {
            Debug.Log("Chartboost interstitial ad has been loaded successfully.");
        }

        void CBDidClickInterstitial(CBLocation location)
        {
        }

        void CBDidCloseInterstitial(CBLocation location)
        {
            if (InterstitialAdCompleted != null)
                InterstitialAdCompleted(InterstitialAdNetwork.Chartboost, AdLocation.LocationFromName(location.ToString()));
        }

        void CBDidDismissInterstitial(CBLocation location)
        {
        }

        void CBDidFailToLoadInterstitial(CBLocation location, CBImpressionError error)
        {
            Debug.Log("Chartboost interstitial ad failed to load.");
        }

        //------------------------------------------------------------
        // Rewarded Ad Events.
        //------------------------------------------------------------

        void CBDidCacheRewardedVideo(CBLocation location)
        {
            Debug.Log("Chartboost rewarded video ad has been loaded successfully.");
        }

        void CBDidFailToLoadRewardedVideo(CBLocation location, CBImpressionError error)
        {
            Debug.Log("Chartboost rewarded video ad failed to load.");
        }

        void CBDidCompleteRewardedVideo(CBLocation location, int reward)
        {
            isCBRewardedAdCompleted = true;
        }

        void CBDidClickRewardedVideo(CBLocation location)
        {
        }

        void CBDidCloseRewardedVideo(CBLocation location)
        {
            if (isCBRewardedAdCompleted)
            {
                isCBRewardedAdCompleted = false;

                if (RewardedAdCompleted != null)
                    RewardedAdCompleted(RewardedAdNetwork.Chartboost, AdLocation.LocationFromName(location.ToString()));
            }
            else
            {
                if (RewardedAdSkipped != null)
                    RewardedAdSkipped(RewardedAdNetwork.Chartboost, AdLocation.LocationFromName(location.ToString()));
            }
        }

        void CBDidDismissRewardedVideo(CBLocation location)
        {
        }
        #endif

        #endregion  // Ad Event Handlers
    }
}