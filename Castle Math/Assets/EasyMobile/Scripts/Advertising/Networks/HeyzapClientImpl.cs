using UnityEngine;
using System;
using System.Collections;

namespace EasyMobile.AdvertisingInternal
{
    #if EM_HEYZAP
    using Heyzap;
    #endif

    internal class HeyzapClientImpl : IAdClient
    {
        private const string NO_SDK_MESSAGE = "SDK missing. Please import the Heyzap plugin.";

        #pragma warning disable 0067
        public event Action<InterstitialAdNetwork, AdLocation> InterstitialAdCompleted;

        public event Action<RewardedAdNetwork, AdLocation> RewardedAdSkipped;

        public event Action<RewardedAdNetwork, AdLocation> RewardedAdCompleted;
        #pragma warning restore 0067

        // Singleton.
        private static HeyzapClientImpl sInstance;

        #if EM_HEYZAP
        private bool isInitialized = false;
        private AdSettings globalAdSettings = null;
        #endif

        #region Object Creators

        private HeyzapClientImpl()
        {
        }

        /// <summary>
        /// Creates and initializes the singleton client with the provided settings.
        /// </summary>
        /// <returns>The client.</returns>
        /// <param name="settings">Settings.</param>
        public static HeyzapClientImpl CreateClient(AdSettings settings)
        {
            if (sInstance == null)
            {
                sInstance = new HeyzapClientImpl();
                sInstance.Init(settings);
            }
            return sInstance;
        }

        #endregion  // Object Creators

        #region IAdClient Implementation

        //------------------------------------------------------------
        // General Info.
        //------------------------------------------------------------

        public AdNetwork Network { get { return AdNetwork.Heyzap; } }

        public bool SupportBannerAds { get { return true; } }

        public bool SupportInterstitialAds { get { return true; } }

        public bool SupportRewardedAds { get { return true; } }

        //------------------------------------------------------------
        // Init.
        //------------------------------------------------------------

        public void Init(AdSettings settings)
        {
            #if EM_HEYZAP
            if (isInitialized)
            {
                Debug.Log("Heyzap client is already initialized. Ignoring this call.");
                return;
            }

            // Store a reference to the global settings.
            globalAdSettings = settings;

            // Start Heyzap with no automatic fetching since we'll handle ad loading.
            HeyzapAds.Start(globalAdSettings.HeyzapPublisherId, HeyzapAds.FLAG_DISABLE_AUTOMATIC_FETCHING);

            // Add callback handlers
            HZBannerAd.SetDisplayListener(BannerAdCallback);
            HZInterstitialAd.SetDisplayListener(InterstitialAdCallback);
            HZIncentivizedAd.SetDisplayListener(RewardedAdCallback);

            isInitialized = true;
            Debug.Log("Heyzap client has been initialized.");
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        public bool IsReady()
        {
            return false;
        }

        //------------------------------------------------------------
        // Show Test Suite (not IAdClient method)
        //------------------------------------------------------------

        public void ShowTestSuite()
        {
            #if EM_HEYZAP
            HeyzapAds.ShowMediationTestSuite();
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        //------------------------------------------------------------
        // Banner Ads.
        //------------------------------------------------------------

        public void ShowBannerAd(BannerAdPosition position, BannerAdSize size)
        {
            #if EM_HEYZAP
            HZBannerShowOptions showOptions = new HZBannerShowOptions();
            showOptions.Position = position.ToHeyzapAdPosition();
            HZBannerAd.ShowWithOptions(showOptions);
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        public void HideBannerAd()
        {
            #if EM_HEYZAP
            HZBannerAd.Hide();
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        public void DestroyBannerAd()
        {
            #if EM_HEYZAP
            HZBannerAd.Destroy();
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        //------------------------------------------------------------
        // Interstitial Ads.
        //------------------------------------------------------------

        public void LoadInterstitialAd(AdLocation location)
        {
            #if EM_HEYZAP
            HZInterstitialAd.Fetch();
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        public bool IsInterstitialAdReady(AdLocation location)
        {
            #if EM_HEYZAP
            return HZInterstitialAd.IsAvailable();
            #else
            return false;
            #endif
        }

        public void ShowInterstitialAd(AdLocation location)
        {
            #if EM_HEYZAP
            if (HZInterstitialAd.IsAvailable())
                HZInterstitialAd.Show();
            else
                Debug.Log("Could not show Heyzap interstitial ad: ad is not loaded.");
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        //------------------------------------------------------------
        // Rewarded Ads.
        //------------------------------------------------------------

        public void LoadRewardedAd(AdLocation location)
        {
            #if EM_HEYZAP
            HZIncentivizedAd.Fetch();
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        public bool IsRewardedAdReady(AdLocation location)
        {
            #if EM_HEYZAP
            return HZIncentivizedAd.IsAvailable();
            #else
            return false;
            #endif
        }

        public void ShowRewardedAd(AdLocation location)
        {
            #if EM_HEYZAP
            if (HZIncentivizedAd.IsAvailable())
                HZIncentivizedAd.Show();
            else
                Debug.Log("Could not show Heyzap rewarded ad: ad is not loaded.");
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        #endregion  // IAdClient Implementation

        #region Ad Event Handlers

        #if EM_HEYZAP
        
        void BannerAdCallback(string adState, string adTag)
        {
            if (adState == "loaded")
            {
                // Do something when the banner ad is loaded
                Debug.Log("Heyzap banner ad has been loaded.");
            }
            if (adState == "error")
            {
                // Do something when the banner ad fails to load (they can fail when refreshing after successfully loading)
                Debug.Log("Heyzap banner ad failed to load.");
            }
            if (adState == "click")
            {
                // Do something when the banner ad is clicked, like pause your game
            }
        }

        void InterstitialAdCallback(string adState, string adTag)
        {
            if (adState.Equals("show"))
            {
                // Sent when an ad has been displayed.
                // This is a good place to pause your app, if applicable.
            }
            else if (adState.Equals("hide"))
            {
                // Sent when an ad has been removed from view.
                // This is a good place to unpause your app, if applicable.
                if (InterstitialAdCompleted != null)
                    InterstitialAdCompleted(InterstitialAdNetwork.Heyzap, AdLocation.Default);
            }
            else if (adState.Equals("failed"))
            {
                // Sent when you call `show`, but there isn't an ad to be shown.
                // Some of the possible reasons for show errors:
                //    - `HeyzapAds.PauseExpensiveWork()` was called, which pauses 
                //      expensive operations like SDK initializations and ad
                //      fetches, andand `HeyzapAds.ResumeExpensiveWork()` has not
                //      yet been called
                //    - The given ad tag is disabled (see your app's Publisher
                //      Settings dashboard)
                //    - An ad is already showing
                //    - A recent IAP is blocking ads from being shown (see your
                //      app's Publisher Settings dashboard)
                //    - One or more of the segments the user falls into are
                //      preventing an ad from being shown (see your Segmentation
                //      Settings dashboard)
                //    - Incentivized ad rate limiting (see your app's Publisher
                //      Settings dashboard)
                //    - One of the mediated SDKs reported it had an ad to show
                //      but did not display one when asked (a rare case)
                //    - The SDK is waiting for a network request to return before an
                //      ad can show
            }
            else if (adState.Equals("available"))
            {
                // Sent when an ad has been loaded and is ready to be displayed,
                //   either because we autofetched an ad or because you called
                //   `Fetch`.
                Debug.Log("Heyzap interstitial ad has been loaded.");
            }
            else if (adState.Equals("fetch_failed"))
            {
                // Sent when an ad has failed to load.
                // This is sent with when we try to autofetch an ad and fail, and also
                //    as a response to calls you make to `Fetch` that fail.
                // Some of the possible reasons for fetch failures:
                //    - Incentivized ad rate limiting (see your app's Publisher
                //      Settings dashboard)
                //    - None of the available ad networks had any fill
                //    - Network connectivity
                //    - The given ad tag is disabled (see your app's Publisher
                //      Settings dashboard)
                //    - One or more of the segments the user falls into are
                //      preventing an ad from being fetched (see your
                //      Segmentation Settings dashboard)
                Debug.Log("Heyzap interstitial ad failed to load.");
            }
            else if (adState.Equals("audio_starting"))
            {
                // The ad about to be shown will need audio.
                // Mute any background music.
            }
            else if (adState.Equals("audio_finished"))
            {
                // The ad being shown no longer needs audio.
                // Any background music can be resumed.
            }
        }

        void RewardedAdCallback(string adState, string adTag)
        {
            if (adState.Equals("incentivized_result_complete"))
            {
                // The user has watched the entire video and should be given a reward.
                if (RewardedAdCompleted != null)
                    RewardedAdCompleted(RewardedAdNetwork.Heyzap, AdLocation.Default);
            }
            else if (adState.Equals("incentivized_result_incomplete"))
            {
                // The user did not watch the entire video and should not be given a reward.
                if (RewardedAdSkipped != null)
                    RewardedAdSkipped(RewardedAdNetwork.Heyzap, AdLocation.Default);
            }
        }

        #endif

        #endregion  // Ad Event Handlers
    }
}