using UnityEngine;
using System;
using System.Collections;

namespace EasyMobile.AdvertisingInternal
{
    #if EM_ADMOB
    using GoogleMobileAds;
    using GoogleMobileAds.Api;
    using EasyMobile.Internal;
    #endif

    internal class AdMobClientImpl : IAdClient
    {
        private const string NO_SDK_MESSAGE = "SDK missing. Please import the AdMob (Google Mobile Ads) plugin.";

        #pragma warning disable 0067
        public event Action<InterstitialAdNetwork, AdLocation> InterstitialAdCompleted;

        public event Action<RewardedAdNetwork, AdLocation> RewardedAdSkipped;

        public event Action<RewardedAdNetwork, AdLocation> RewardedAdCompleted;
        #pragma warning restore 0067

        // Singleton.
        private static AdMobClientImpl sInstance;

        #if EM_ADMOB
        private bool isInitialized = false;
        private AdSettings globalAdSettings = null;
        private BannerView bannerView = null;
        private InterstitialAd interstitialAd = null;
        private RewardBasedVideoAd rewardedAd = null;
        private bool isRewardedAdPlaying = false;
        private bool isRewardedAdCompleted = false;
        #endif

        #region Object Creators

        private AdMobClientImpl()
        {
        }

        /// <summary>
        /// Creates and initializes the singleton client with the provided settings.
        /// </summary>
        /// <returns>The client.</returns>
        /// <param name="settings">Settings.</param>
        public static AdMobClientImpl CreateClient(AdSettings settings)
        {
            if (sInstance == null)
            {
                sInstance = new AdMobClientImpl();
                sInstance.Init(settings);
            }
            return sInstance;
        }

        #endregion  // Object Creators

        #region IAdClient Implementation

        //------------------------------------------------------------
        // General Info.
        //------------------------------------------------------------

        public AdNetwork Network { get { return AdNetwork.AdMob; } }

        public bool SupportBannerAds { get { return true; } }

        public bool SupportInterstitialAds { get { return true; } }

        public bool SupportRewardedAds { get { return true; } }

        //------------------------------------------------------------
        // Init.
        //------------------------------------------------------------

        public void Init(AdSettings settings)
        {
            #if EM_ADMOB
            if (!isInitialized)
            {
                // Store a reference to the global settings.
                globalAdSettings = settings;

                // Initialize the Google Mobile Ads SDK.
                MobileAds.Initialize(globalAdSettings.AdMobIds.AppId);

                // Ready to work!
                isInitialized = true;

                Debug.Log("AdMob client has been initialized.");
            }
            else
            {
                Debug.Log("AdMob client is already initalized. Ignoring this call.");
            }
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        public bool IsReady()
        {
            #if EM_ADMOB
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
            #if EM_ADMOB
            // If no bannerView object exists, create a new one and load ad into it.
            // Otherwise just show the existing banner (which may be hidden before).
            if (bannerView == null)
            {
                bannerView = new BannerView(
                    globalAdSettings.AdMobIds.BannerAdId, 
                    size.ToAdMobAdSize(), 
                    position.ToAdMobAdPosition()
                );

                // Register for banner ad events.
                bannerView.OnAdLoaded += HandleAdMobBannerAdLoaded;
                bannerView.OnAdFailedToLoad += HandleAdMobBannerAdFailedToLoad;
                bannerView.OnAdOpening += HandleAdMobBannerAdOpened;
                bannerView.OnAdClosed += HandleAdMobBannerAdClosed;
                bannerView.OnAdLeavingApplication += HandleAdMobBannerAdLeftApplication;

                // Load ad.
                bannerView.LoadAd(CreateAdMobAdRequest());
            }

            bannerView.Show();
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        public void HideBannerAd()
        {
            #if EM_ADMOB
            if (bannerView != null)
            {
                bannerView.Hide();
            }
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        public void DestroyBannerAd()
        {
            #if EM_ADMOB
            if (bannerView != null)
            {
                bannerView.Destroy();
                bannerView = null;
            }
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        //------------------------------------------------------------
        // Interstitial Ads.
        //------------------------------------------------------------

        public void LoadInterstitialAd(AdLocation location)
        {
            #if EM_ADMOB
            // Note: On iOS, InterstitialAd objects are one time use objects. 
            // That means once an interstitial is shown, the InterstitialAd object can't be used to load another ad. 
            // To request another interstitial, you'll need to create a new InterstitialAd object.
            if (interstitialAd != null && interstitialAd.IsLoaded())
            {
                return;
            }

            if (interstitialAd == null)
            {
                // Create new interstitial object.
                interstitialAd = new InterstitialAd(globalAdSettings.AdMobIds.InterstitialAdId);

                // Register for interstitial ad events.
                interstitialAd.OnAdLoaded += HandleAdMobInterstitialLoaded;
                interstitialAd.OnAdFailedToLoad += HandleAdMobInterstitialFailedToLoad;
                interstitialAd.OnAdOpening += HandleAdMobInterstitialOpened;
                interstitialAd.OnAdClosed += HandleAdMobInterstitialClosed;
                interstitialAd.OnAdLeavingApplication += HandleAdMobInterstitialLeftApplication;
            }

            // By this time interstitialAd either holds the existing, unloaded, unused ad object, or the newly created one.
            interstitialAd.LoadAd(CreateAdMobAdRequest());
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        public bool IsInterstitialAdReady(AdLocation location)
        {
            #if EM_ADMOB
            return (interstitialAd != null && interstitialAd.IsLoaded());
            #else
            return false;
            #endif
        }

        public void ShowInterstitialAd(AdLocation location)
        {
            #if EM_ADMOB
            if (interstitialAd != null && interstitialAd.IsLoaded())
                interstitialAd.Show();
            else
                Debug.Log("Could not show AdMob interstitial ad: ad is not loaded.");
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        //------------------------------------------------------------
        // Rewarded Ads.
        //------------------------------------------------------------

        public void LoadRewardedAd(AdLocation location)
        {
            #if EM_ADMOB
            // Loading a new rewarded ad seems to disable all events of the currently playing ad,
            // so we shouldn't perform rewarded ad loading while playing another one.
            if (isRewardedAdPlaying)
                return;

            if (rewardedAd == null)
            {
                rewardedAd = RewardBasedVideoAd.Instance;

                // RewardBasedVideoAd is a singleton, so handlers should only be registered once.
                rewardedAd.OnAdLoaded += HandleAdMobRewardBasedVideoLoaded;
                rewardedAd.OnAdFailedToLoad += HandleAdMobRewardBasedVideoFailedToLoad;
                rewardedAd.OnAdOpening += HandleAdMobRewardBasedVideoOpened;
                rewardedAd.OnAdStarted += HandleAdMobRewardBasedVideoStarted;
                rewardedAd.OnAdRewarded += HandleAdMobRewardBasedVideoRewarded;
                rewardedAd.OnAdClosed += HandleAdMobRewardBasedVideoClosed;
                rewardedAd.OnAdLeavingApplication += HandleAdMobRewardBasedVideoLeftApplication;
            }

            rewardedAd.LoadAd(CreateAdMobAdRequest(), globalAdSettings.AdMobIds.RewardedAdId);
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        public bool IsRewardedAdReady(AdLocation location)
        {
            #if EM_ADMOB
            return (rewardedAd != null && rewardedAd.IsLoaded());
            #else
            return false;
            #endif
        }

        public void ShowRewardedAd(AdLocation location)
        {
            #if EM_ADMOB
            if (rewardedAd != null && rewardedAd.IsLoaded())
            {
                isRewardedAdPlaying = true;
                rewardedAd.Show();
            }
            else
            {
                Debug.Log("Could not show AdMob rewarded ad: ad is not loaded.");
            }
            #else
            Debug.LogError(NO_SDK_MESSAGE);
            #endif
        }

        #endregion  // IAdClient Implementation

        #region Private Methods

        #if EM_ADMOB
        private AdRequest CreateAdMobAdRequest()
        {
            AdRequest.Builder adBuilder = new AdRequest.Builder();

            // Targeting settings.
            var targeting = globalAdSettings.AdMobTargeting;

            // Gender.
            if (targeting.gender != AdSettings.TargetGender.Unspecified)
                adBuilder.SetGender(
                    targeting.gender == AdSettings.TargetGender.Male ? 
                    GoogleMobileAds.Api.Gender.Male : 
                    GoogleMobileAds.Api.Gender.Female
                );

            // Birthday.
            if (targeting.setBirthday)
                adBuilder.SetBirthday(targeting.birthday);

            // Child-directed.
            if (targeting.tagForChildDirectedTreatment != AdSettings.ChildDirectedTreatmentOption.Unspecified)
                adBuilder.TagForChildDirectedTreatment(targeting.tagForChildDirectedTreatment == AdSettings.ChildDirectedTreatmentOption.Yes);

            // Extras.
            if (targeting.extras != null)
            {
                foreach (var extra in targeting.extras)
                {
                    if (!string.IsNullOrEmpty(extra.key) && !string.IsNullOrEmpty(extra.value))
                        adBuilder.AddExtra(extra.key, extra.value);
                }
            }

            // Test mode.
            if (globalAdSettings.AdMobEnableTestMode)
            {
                // Add all emulators
                adBuilder.AddTestDevice(AdRequest.TestDeviceSimulator);

                // Add user-specified test devices
                for (int i = 0; i < globalAdSettings.AdMobTestDeviceIds.Length; i++)
                    adBuilder.AddTestDevice(Helper.AutoTrimId(globalAdSettings.AdMobTestDeviceIds[i]));
            }

            return adBuilder.Build();
        }

        #endif

        #endregion // Private Methods

        #region Ad Event Handlers

        #if EM_ADMOB
        
        //------------------------------------------------------------
        // Banner Ads Callbacks.
        //------------------------------------------------------------

        void HandleAdMobBannerAdLoaded(object sender, EventArgs args)
        {
            Debug.Log("AdMob banner ad has been loaded successfully.");
        }

        void HandleAdMobBannerAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log("AdMob banner ad failed to load.");
        }

        void HandleAdMobBannerAdOpened(object sender, EventArgs args)
        {
        }

        void HandleAdMobBannerAdClosed(object sender, EventArgs args)
        {
        }

        void HandleAdMobBannerAdLeftApplication(object sender, EventArgs args)
        {
        }

        //------------------------------------------------------------
        // Interstitial Ads Callbacks.
        //------------------------------------------------------------

        void HandleAdMobInterstitialLoaded(object sender, EventArgs args)
        {
            Debug.Log("AdMob interstitial ad has been loaded successfully.");
        }

        void HandleAdMobInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log("AdMob interstitial ad failed to load.");
        }

        void HandleAdMobInterstitialOpened(object sender, EventArgs args)
        {
        }

        void HandleAdMobInterstitialClosed(object sender, EventArgs args)
        {
            // Note: On iOS, InterstitialAd objects are one time use objects. 
            // ==> Destroy the used interstitial ad object; also reset
            // the reference to force new objects to be created when loading ads.
            interstitialAd.Destroy();
            interstitialAd = null;

            // Make sure the event is raised on main thread.
            Helper.RunOnMainThread(() =>
                {
                    if (InterstitialAdCompleted != null)
                        InterstitialAdCompleted(InterstitialAdNetwork.AdMob, AdLocation.Default);
                });
        }

        void HandleAdMobInterstitialLeftApplication(object sender, EventArgs args)
        {
        }

        //------------------------------------------------------------
        // Rewarded Ads Callbacks.
        //------------------------------------------------------------

        void HandleAdMobRewardBasedVideoLoaded(object sender, EventArgs args)
        {
            Debug.Log("AdMob rewarded video ad has been loaded successfully.");
        }

        void HandleAdMobRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log("AdMob rewarded video ad failed to load.");
        }

        void HandleAdMobRewardBasedVideoOpened(object sender, EventArgs args)
        {
        }

        void HandleAdMobRewardBasedVideoStarted(object sender, EventArgs args)
        {
        }

        void HandleAdMobRewardBasedVideoClosed(object sender, EventArgs args)
        {
            // Ad is not playing anymore.
            isRewardedAdPlaying = false;

            // If the ad was completed, the "rewarded" event should be fired previously,
            // setting the completed bool to true. Otherwise the ad was skipped.
            // Events are raised on main thread.
            if (isRewardedAdCompleted)
            {
                // Ad completed.
                isRewardedAdCompleted = false;
                Helper.RunOnMainThread(() =>
                    {
                        if (RewardedAdCompleted != null)
                            RewardedAdCompleted(RewardedAdNetwork.AdMob, AdLocation.Default);
                    });
            }
            else
            {
                // Ad skipped.
                Helper.RunOnMainThread(() =>
                    {
                        if (RewardedAdSkipped != null)
                            RewardedAdSkipped(RewardedAdNetwork.AdMob, AdLocation.Default);
                    });
            }
        }

        void HandleAdMobRewardBasedVideoRewarded(object sender, Reward args)
        {
            isRewardedAdCompleted = true;
        }

        void HandleAdMobRewardBasedVideoLeftApplication(object sender, EventArgs args)
        {
        }
        #endif

        #endregion // Internal Stuff
    }
}