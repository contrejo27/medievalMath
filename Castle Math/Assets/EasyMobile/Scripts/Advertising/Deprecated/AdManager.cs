using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if EM_ADCOLONY
using EM_AdColony = AdColony;
#endif

#if EM_ADMOB
using GoogleMobileAds;
using GoogleMobileAds.Api;
#endif

#if EM_CHARTBOOST
using ChartboostSDK;
#endif

#if EM_HEYZAP
using Heyzap;
#endif

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

namespace EasyMobile
{
    [AddComponentMenu("")][System.Obsolete("This class was deprecated in Easy Mobile Pro 1.2.0. Please use EasyMobile.Advertising instead")]
    public class AdManager : MonoBehaviour
    {
        public static AdManager Instance { get; private set; }

        // Suppress the "Event is never used" warnings.
        #pragma warning disable 0067
        /// <summary>
        /// Occurs when an interstitial ad is closed.
        /// </summary>
        public static event Action<InterstitialAdNetwork, AdLocation> InterstitialAdCompleted = delegate {};

        /// <summary>
        /// Occurs when a rewarded ad is skipped (the user didn't complete watching
        /// the ad and therefore is not entitled to the reward).
        /// </summary>
        public static event Action<RewardedAdNetwork, AdLocation> RewardedAdSkipped = delegate {};

        /// <summary>
        /// Occurs when a rewarded ad completed and the user should be rewarded.
        /// </summary>
        public static event Action<RewardedAdNetwork, AdLocation> RewardedAdCompleted = delegate {};
        #pragma warning restore 0067

        /// <summary>
        /// Occurs when ads have been removed.
        /// </summary>
        public static event Action AdsRemoved = delegate {};

        private const string UNITYADS_REWARDED_ZONE_ID = "rewardedVideo";
        private static float lastInterstitialAdLoadTimestamp = -1000f;
        private static float lastRewardedAdLoadTimestamp = -1000f;
        private static List<BannerAdNetwork> activeBannerAdNetworks = new List<BannerAdNetwork>();

        // For storing removeAds status
        private const string AD_REMOVE_STATUS_PPKEY = "EM_REMOVE_ADS";
        private const int AD_ENABLED = 1;
        private const int AD_DISABLED = -1;

        // AdColony specific stuff
        #if EM_ADCOLONY
        private static bool isAdColonyInitialized;
        private static EM_AdColony.InterstitialAd adColonyInterstitialAd;
        private static EM_AdColony.InterstitialAd adColonyRewardedAd;
        private static bool isAdColonyRewardedAdCompleted;
        #endif

        // AdMob specific stuff
        #if EM_ADMOB
        private static BannerView admobBannerView;
        private static InterstitialAd admobInterstitial;
        private static RewardBasedVideoAd admobRewardedAd;
        private static bool isAdMobInterstitialClosed;
        private static bool isAdMobRewardedAdPlaying;
        private static bool isAdMobRewardedAdClosed;
        private static bool isAdMobRewardedAdCompleted;
        #endif

        // Chartboost specific stuff
        #if EM_CHARTBOOST
        private static bool isCBRewardedAdCompleted;
        #endif

        // Auto load ads coroutine
        private static IEnumerator autoLoadAdsCoroutine;
        private static bool isAutoLoadDefaultAds;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        void OnEnable()
        {
            #if EM_ADCOLONY
            EM_AdColony.Ads.OnConfigurationCompleted += OnAdColonyConfigurationCompleted;
            EM_AdColony.Ads.OnRequestInterstitialFailedWithZone += OnAdColonyRequestAdFailedWithZone;
            EM_AdColony.Ads.OnRequestInterstitial += OnAdColonyRequestAdCompleted;
            EM_AdColony.Ads.OnOpened += OnAdColonyAdOpened;
            EM_AdColony.Ads.OnClosed += OnAdColonyAdClosed;
            EM_AdColony.Ads.OnRewardGranted += OnAdColonyRewardGranted;
            #endif

            #if EM_CHARTBOOST
            Chartboost.didCacheInterstitial += CBDidCacheInterstitial;
            Chartboost.didDisplayInterstitial += CBDidDisplayInterstitial;
            Chartboost.didClickInterstitial += CBDidClickInterstitial;
            Chartboost.didCloseInterstitial += CBDidCloseInterstitial;
            Chartboost.didDismissInterstitial += CBDidDismissInterstitial;
            Chartboost.didFailToLoadInterstitial += CBDidFailToLoadInterstitial;

            Chartboost.didCacheRewardedVideo += CBDidCacheRewardedVideo;
            Chartboost.didClickRewardedVideo += CBDidClickRewardedVideo;
            Chartboost.didCloseRewardedVideo += CBDidCloseRewardedVideo;
            Chartboost.didFailToLoadRewardedVideo += CBDidFailToLoadRewardedVideo;
            Chartboost.didCompleteRewardedVideo += CBDidCompleteRewardedVideo;
            #endif
        }

        void OnDisable()
        {
            #if EM_ADCOLONY
            EM_AdColony.Ads.OnConfigurationCompleted -= OnAdColonyConfigurationCompleted;
            EM_AdColony.Ads.OnRequestInterstitialFailedWithZone -= OnAdColonyRequestAdFailedWithZone;
            EM_AdColony.Ads.OnRequestInterstitial -= OnAdColonyRequestAdCompleted;
            EM_AdColony.Ads.OnOpened += OnAdColonyAdOpened;
            EM_AdColony.Ads.OnClosed += OnAdColonyAdClosed;
            EM_AdColony.Ads.OnRewardGranted -= OnAdColonyRewardGranted;
            #endif

            #if EM_CHARTBOOST
            Chartboost.didCacheInterstitial -= CBDidCacheInterstitial;
            Chartboost.didDisplayInterstitial -= CBDidDisplayInterstitial;
            Chartboost.didClickInterstitial -= CBDidClickInterstitial;
            Chartboost.didCloseInterstitial -= CBDidCloseInterstitial;
            Chartboost.didDismissInterstitial -= CBDidDismissInterstitial;
            Chartboost.didFailToLoadInterstitial -= CBDidFailToLoadInterstitial;

            Chartboost.didCacheRewardedVideo -= CBDidCacheRewardedVideo;
            Chartboost.didClickRewardedVideo -= CBDidClickRewardedVideo;
            Chartboost.didCloseRewardedVideo -= CBDidCloseRewardedVideo;
            Chartboost.didFailToLoadRewardedVideo -= CBDidFailToLoadRewardedVideo;
            Chartboost.didCompleteRewardedVideo -= CBDidCompleteRewardedVideo;
            #endif
        }

        void Start()
        {
            #if EM_ADCOLONY
            // Configure AdColony
            var adColonyIds = EM_Settings.Advertising.AdColonyIds;
            var appOptions = new EM_AdColony.AppOptions()
            {
                AdOrientation = ToAdColonyAdOrientation(EM_Settings.Advertising.AdColonyAdOrientation)
            };

            EM_AdColony.Ads.Configure(
                adColonyIds.AppId,
                appOptions,
                adColonyIds.InterstitialAdId,
                adColonyIds.RewardedAdId
            );
            #endif

            #if EM_CHARTBOOST
            // Create Chartboost object. Even if Chartboost is not listed as one of default ad networks,
            // it should still be created so that we can use it with the API as an "undefault" network.
            // We'll also handle ad loading, so turning off Chartboost's autocache feature.
            Chartboost.Create();
            Chartboost.setAutoCacheAds(false);
            #endif

            #if EM_HEYZAP
            // Start Heyzap with no automatic fetching since we'll handle ad loading.
            HeyzapAds.Start(EM_Settings.Advertising.HeyzapPublisherId, HeyzapAds.FLAG_DISABLE_AUTOMATIC_FETCHING);

            // Add callback handlers
            HZBannerAd.SetDisplayListener(HeyzapBannerAdCallback);
            HZInterstitialAd.SetDisplayListener(HeyzapInterstitialAdCallback);
            HZIncentivizedAd.SetDisplayListener(HeyzapRewardedAdCallback);

            // Show TestSuite if needed.
            if (EM_Settings.Advertising.HeyzapShowTestSuite)
            HeyzapAds.ShowMediationTestSuite();
            #endif

            // Start the coroutine that checks for ads readiness and performs loading if they're not.
            isAutoLoadDefaultAds = EM_Settings.Advertising.IsAutoLoadDefaultAds;
            if (isAutoLoadDefaultAds)
            {
                autoLoadAdsCoroutine = CRAutoLoadAds();
                StartCoroutine(autoLoadAdsCoroutine);
            }
        }

        void Update()
        {   
            // Always track EM_Settings.Advertising.IsAutoLoadDefaultAds so that we can adjust
            // accordingly if it was changed elsewhere.
            if (isAutoLoadDefaultAds != EM_Settings.Advertising.IsAutoLoadDefaultAds)
            {
                SetAutoLoadDefaultAds(EM_Settings.Advertising.IsAutoLoadDefaultAds);
            }

            #if EM_ADMOB
            // AdMob events are not raised from the main thread,
            // so we just set appropriate flags in those events and poll their values here.
            if (isAdMobInterstitialClosed)
            {
                isAdMobInterstitialClosed = false;
                InterstitialAdCompleted(InterstitialAdNetwork.AdMob, AdLocation.Default);
            }

            if (isAdMobRewardedAdClosed)
            {
                isAdMobRewardedAdClosed = false;
                isAdMobRewardedAdPlaying = false;

                if (isAdMobRewardedAdCompleted)
                {
                    isAdMobRewardedAdCompleted = false;
                    RewardedAdCompleted(RewardedAdNetwork.AdMob, AdLocation.Default);
                }
                else
                {
                    RewardedAdSkipped(RewardedAdNetwork.AdMob, AdLocation.Default);
                }
            }
            #endif
        }

        /// <summary>
        /// Whether auto loading of default ads is enabled.
        /// </summary>
        /// <returns><c>true</c> if auto load default ads is enabled; otherwise, <c>false</c>.</returns>
        public static bool IsAutoLoadDefaultAds()
        {
            return EM_Settings.Advertising.IsAutoLoadDefaultAds;
        }

        /// <summary>
        /// Enables or disables auto loading of default ads.
        /// </summary>
        /// <param name="isAutoLoad">If set to <c>true</c> auto load is enabled, otherwise it is disabled.</param>
        public static void SetAutoLoadDefaultAds(bool isAutoLoad)
        {
            isAutoLoadDefaultAds = isAutoLoad;
            EM_Settings.Advertising.IsAutoLoadDefaultAds = isAutoLoad;

            if (!isAutoLoad)
            {
                if (autoLoadAdsCoroutine != null)
                {
                    Instance.StopCoroutine(autoLoadAdsCoroutine);
                    autoLoadAdsCoroutine = null;
                }
            }
            else
            {
                if (autoLoadAdsCoroutine == null)
                {
                    autoLoadAdsCoroutine = CRAutoLoadAds();
                    Instance.StartCoroutine(autoLoadAdsCoroutine);
                }
            }
        }

        /// <summary>
        /// Shows banner ad using the default banner ad network.
        /// </summary>
        /// <param name="position">Position.</param>
        public static void ShowBannerAd(BannerAdPosition position)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    ShowBannerAd(EM_Settings.Advertising.AndroidDefaultAdNetworks.bannerAdNetwork, position, BannerAdSize.SmartBanner);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    ShowBannerAd(EM_Settings.Advertising.IosDefaultAdNetworks.bannerAdNetwork, position, BannerAdSize.SmartBanner);
                    break;
            }
        }

        /// <summary>
        /// Shows banner ad using the default banner ad network at the specified position and size.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="size">Ad size, applicable for AdMob banner only.</param>
        public static void ShowBannerAd(BannerAdPosition position, BannerAdSize size)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    ShowBannerAd(EM_Settings.Advertising.AndroidDefaultAdNetworks.bannerAdNetwork, position, size);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    ShowBannerAd(EM_Settings.Advertising.IosDefaultAdNetworks.bannerAdNetwork, position, size);
                    break;
            }
        }

        /// <summary>
        /// Shows banner ad using the specified ad network at the specified position and size.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="position">Position.</param>
        /// <param name="size">Ad size, applicable for AdMob banner only.</param>
        public static void ShowBannerAd(BannerAdNetwork adNetwork, BannerAdPosition position, BannerAdSize size)
        {
            if (IsAdRemoved())
            {
                Debug.Log("ShowBannerAd FAILED: Ads were removed.");
                return;
            }

            switch ((AdNetwork)adNetwork)
            {
                case AdNetwork.AdMob:

                    #if EM_ADMOB
                    if (admobBannerView == null)
                    {
                        admobBannerView = new BannerView(EM_Settings.Advertising.AdMobIds.BannerAdId, size.ToAdMobAdSize(), position.ToAdMobAdPosition());

                        if (admobBannerView != null)
                        {
                            // Register for ad events.
                            admobBannerView.OnAdLoaded += HandleAdMobBannerAdLoaded;
                            admobBannerView.OnAdFailedToLoad += HandleAdMobBannerAdFailedToLoad;
                            admobBannerView.OnAdOpening += HandleAdMobBannerAdOpened;
                            admobBannerView.OnAdClosed += HandleAdMobBannerAdClosed;
                            admobBannerView.OnAdLeavingApplication += HandleAdMobBannerAdLeftApplication;

                            // Load ad
                            admobBannerView.LoadAd(CreateAdMobAdRequest());
                        }
                    }

                    if (admobBannerView != null)
                    {
                        admobBannerView.Show();

                        if (!activeBannerAdNetworks.Contains(adNetwork))
                            activeBannerAdNetworks.Add(adNetwork);
                    }
                    #else
                    Debug.LogError("SDK missing. Please import Google Mobile Ads plugin.");
                    #endif

                    break;
                case AdNetwork.Heyzap:

                    #if EM_HEYZAP
                    HZBannerShowOptions showOptions = new HZBannerShowOptions();
                    showOptions.Position = position.ToHeyzapAdPosition();
                    HZBannerAd.ShowWithOptions(showOptions);

                    if (!activeBannerAdNetworks.Contains(adNetwork))
                    activeBannerAdNetworks.Add(adNetwork);
                    #else
                    Debug.LogError("SDK missing. Please import Heyzap plugin.");
                    #endif

                    break;
            }
        }

        /// <summary>
        /// Hides banner ad of the default banner ad network.
        /// </summary>
        public static void HideBannerAd()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    HideBannerAd(EM_Settings.Advertising.AndroidDefaultAdNetworks.bannerAdNetwork);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    HideBannerAd(EM_Settings.Advertising.IosDefaultAdNetworks.bannerAdNetwork);
                    break;
            }
        }

        /// <summary>
        /// Hides banner ad of the specified ad network if one is shown, otherwise this method is a no-opt.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        public static void HideBannerAd(BannerAdNetwork adNetwork)
        {
            switch ((AdNetwork)adNetwork)
            {
                case AdNetwork.AdMob:

                    #if EM_ADMOB
                    if (admobBannerView != null)
                    {
                        admobBannerView.Hide();

                        if (activeBannerAdNetworks.Contains(adNetwork))
                            activeBannerAdNetworks.Remove(adNetwork);
                    }
                    #else
                    Debug.LogError("SDK missing. Please import Google Mobile Ads plugin.");
                    #endif

                    break;
                case AdNetwork.Heyzap:

                    #if EM_HEYZAP
                    HZBannerAd.Hide();

                    if (activeBannerAdNetworks.Contains(adNetwork))
                    activeBannerAdNetworks.Remove(adNetwork);
                    #else
                    Debug.LogError("SDK missing. Please import Heyzap plugin.");
                    #endif

                    break;
            }
        }

        /// <summary>
        /// Destroys the banner ad of the default banner ad network.
        /// </summary>
        public static void DestroyBannerAd()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    DestroyBannerAd(EM_Settings.Advertising.AndroidDefaultAdNetworks.bannerAdNetwork);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    DestroyBannerAd(EM_Settings.Advertising.IosDefaultAdNetworks.bannerAdNetwork);
                    break;
            }
        }

        /// <summary>
        /// Destroys the banner ad of the specified ad network.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        public static void DestroyBannerAd(BannerAdNetwork adNetwork)
        {
            switch ((AdNetwork)adNetwork)
            {
                case AdNetwork.AdMob:

                    #if EM_ADMOB
                    if (admobBannerView != null)
                    {
                        admobBannerView.Destroy();
                        admobBannerView = null;

                        if (activeBannerAdNetworks.Contains(adNetwork))
                            activeBannerAdNetworks.Remove(adNetwork);
                    }
                    #else
                    Debug.LogError("SDK missing. Please import Google Mobile Ads plugin.");
                    #endif

                    break;
                case AdNetwork.Heyzap:

                    #if EM_HEYZAP
                    HZBannerAd.Destroy();

                    if (activeBannerAdNetworks.Contains(adNetwork))
                    activeBannerAdNetworks.Remove(adNetwork);
                    #else
                    Debug.LogError("SDK missing. Please import Heyzap plugin.");
                    #endif

                    break;
            }
        }

        /// <summary>
        /// Determines whether there's a banner ad being shown.
        /// </summary>
        /// <returns><c>true</c> if this instance is showing banner ad; otherwise, <c>false</c>.</returns>
        public static bool IsShowingBannerAd()
        {
            return activeBannerAdNetworks.Count > 0;
        }

        /// <summary>
        /// Returns the array of banner ad networks having a banner ad being shown.
        /// </summary>
        /// <returns>The active banner ad networks.</returns>
        public static BannerAdNetwork[] GetActiveBannerAdNetworks()
        {
            return activeBannerAdNetworks.ToArray();
        }

        /// <summary>
        /// Loads the default interstitial ad.
        /// </summary>
        public static void LoadInterstitialAd()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    LoadInterstitialAd(EM_Settings.Advertising.AndroidDefaultAdNetworks.interstitialAdNetwork, AdLocation.Default);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    LoadInterstitialAd(EM_Settings.Advertising.IosDefaultAdNetworks.interstitialAdNetwork, AdLocation.Default);
                    break;
            }
        }

        /// <summary>
        /// Loads interstitial ad using the specified interstitial ad network at the specified location.
        ///     - For AdColony, AdMob and Heyzap, the location will be ignored. You can pass AdLocation.Default.
        ///     - For Chartboost, select one of available locations or create a new location using AdLocation.LocationFromName(name).
        ///     - For Unity Ads, create a new location for the desired zoneId using AdLocation.LocationFromName(zoneId).
        /// If the specified network doesn't support interstitial ads, this method is a no-opt.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="location">Location.</param>
        public static void LoadInterstitialAd(InterstitialAdNetwork adNetwork, AdLocation location)
        {
            if (IsAdRemoved())
                return;

            switch ((AdNetwork)adNetwork)
            {
                case AdNetwork.AdColony:

                    #if EM_ADCOLONY
                    if (!isAdColonyInitialized)
                        return;

                    if (adColonyInterstitialAd != null)
                        return;

                    EM_AdColony.Ads.RequestInterstitialAd(EM_Settings.Advertising.AdColonyIds.InterstitialAdId, null);
                    #else
                    Debug.LogError("SDK missing. Please import AdColony plugin.");
                    #endif

                    break;
                case AdNetwork.AdMob:

                    #if EM_ADMOB
                    // Destroy old interstitial object if any
                    if (admobInterstitial != null)
                    {
                        admobInterstitial.Destroy();
                        admobInterstitial = null;
                    }

                    // Create new interstitial object
                    admobInterstitial = new InterstitialAd(EM_Settings.Advertising.AdMobIds.InterstitialAdId);

                    if (admobInterstitial != null)
                    {
                        // Register for ad events.
                        admobInterstitial.OnAdLoaded += HandleAdMobInterstitialLoaded;
                        admobInterstitial.OnAdFailedToLoad += HandleAdMobInterstitialFailedToLoad;
                        admobInterstitial.OnAdOpening += HandleAdMobInterstitialOpened;
                        admobInterstitial.OnAdClosed += HandleAdMobInterstitialClosed;
                        admobInterstitial.OnAdLeavingApplication += HandleAdMobInterstitialLeftApplication;

                        // Load the ad
                        admobInterstitial.LoadAd(CreateAdMobAdRequest());
                    }
                    #else
                    Debug.LogError("SDK missing. Please import Google Mobile Ads plugin.");
                    #endif

                    break;
                case AdNetwork.Chartboost:

                    #if EM_CHARTBOOST
                    Chartboost.cacheInterstitial(location.ToChartboostLocation());
                    #else
                    Debug.LogError("SDK missing. Please import Chartboost plugin.");
                    #endif

                    break;
                case AdNetwork.Heyzap:

                    #if EM_HEYZAP
                    HZInterstitialAd.Fetch();
                    #else
                    Debug.LogError("SDK missing. Please import Heyzap plugin.");
                    #endif

                    break;
                case AdNetwork.UnityAds:

                    // Unity Ads are loaded automatically if enabled.
                    #if !UNITY_ADS
                    Debug.LogError("SDK missing. Please enable Unity Ads service.");
                    #endif

                    break;
            }
        }

        /// <summary>
        /// Determines whether the default interstitial ad is ready to show.
        /// </summary>
        /// <returns><c>true</c> if the default interstitial ad is ready; otherwise, <c>false</c>.</returns>
        public static bool IsInterstitialAdReady()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return IsInterstitialAdReady(EM_Settings.Advertising.AndroidDefaultAdNetworks.interstitialAdNetwork, AdLocation.Default);
                case RuntimePlatform.IPhonePlayer:
                    return IsInterstitialAdReady(EM_Settings.Advertising.IosDefaultAdNetworks.interstitialAdNetwork, AdLocation.Default);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether interstitial ad of the specified ad network is ready to show at the given location.
        ///     - For AdColony, AdMob and Heyzap, the location will be ignored. You can pass AdLocation.Default.
        ///     - For Chartboost, select one of available locations or create a new location using AdLocation.LocationFromName(name).
        ///     - For Unity Ads, create a new location for the desired zoneId using AdLocation.LocationFromName(zoneId).
        /// If the specified network doesn't support interstitial ads, this method always returns false.
        /// </summary>
        /// <returns><c>true</c> if interstitial ad is ready for the specified location; otherwise, <c>false</c>.</returns>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="location">Location.</param>
        public static bool IsInterstitialAdReady(InterstitialAdNetwork adNetwork, AdLocation location)
        {
            if (IsAdRemoved())
                return false;

            switch ((AdNetwork)adNetwork)
            {
                case AdNetwork.AdColony:

                    #if EM_ADCOLONY
                    return (isAdColonyInitialized && adColonyInterstitialAd != null);
                    #else
                    return false;
                    #endif

                case AdNetwork.AdMob:

                    #if EM_ADMOB
                    return (admobInterstitial != null && admobInterstitial.IsLoaded());
                    #else
                    return false;
                    #endif

                case AdNetwork.Chartboost:

                    #if EM_CHARTBOOST
                    return Chartboost.hasInterstitial(location.ToChartboostLocation());
                    #else
                    return false;
                    #endif

                case AdNetwork.Heyzap:

                    #if EM_HEYZAP
                    return HZInterstitialAd.IsAvailable();
                    #else
                    return false;
                    #endif

                case AdNetwork.UnityAds:
                    #if UNITY_ADS
                    if (location == AdLocation.Default)
                        return Advertisement.IsReady();
                    else
                        return Advertisement.IsReady(location.ToUnityAdsZoneId());
                    #else
                    return false;
                    #endif

                default:
                    return false;
            }
        }

        /// <summary>
        /// Shows the default interstitial ad.
        /// </summary>
        public static void ShowInterstitialAd()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    ShowInterstitialAd(EM_Settings.Advertising.AndroidDefaultAdNetworks.interstitialAdNetwork, AdLocation.Default);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    ShowInterstitialAd(EM_Settings.Advertising.IosDefaultAdNetworks.interstitialAdNetwork, AdLocation.Default);
                    break;
            }
        }

        /// <summary>
        /// Show an interstitial ad using the specified ad network, at the specified location.
        ///     - For AdColony, AdMob and Heyzap, the location will be ignored. You can pass AdLocation.Default. 
        ///     - For Chartboost, select one of available locations or create a new location using AdLocation.LocationFromName(name).
        ///     - For Unity Ads, create a new location for the desired zoneId using AdLocation.LocationFromName(zoneId).
        /// If the specified network doesn't support interstitial ads, this method is a no-opt.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="location">Location.</param>
        public static void ShowInterstitialAd(InterstitialAdNetwork adNetwork, AdLocation location)
        {
            if (IsAdRemoved())
            {
                Debug.Log("ShowInterstitialAd FAILED: Ads were removed.");
                return;
            }

            if (!IsInterstitialAdReady(adNetwork, location))
            {
                Debug.Log("ShowInterstitialAd FAILED: Interstitial ad is not loaded.");
                return;
            }

            switch ((AdNetwork)adNetwork)
            {
                case AdNetwork.AdColony:

                    #if EM_ADCOLONY
                    EM_AdColony.Ads.ShowAd(adColonyInterstitialAd);
                    adColonyInterstitialAd = null;
                    #else
                    Debug.LogError("SDK missing. Please import AdColony plugin.");
                    #endif

                    break;
                case AdNetwork.AdMob:

                    #if EM_ADMOB
                    admobInterstitial.Show();
                    #else
                    Debug.LogError("SDK missing. Please import Google Mobile Ads plugin.");
                    #endif

                    break;
                case AdNetwork.Chartboost:

                    #if EM_CHARTBOOST
                    Chartboost.showInterstitial(location.ToChartboostLocation());
                    #else
                    Debug.LogError("SDK missing. Please import Chartboost plugin.");
                    #endif

                    break;
                case AdNetwork.Heyzap:

                    #if EM_HEYZAP
                    HZInterstitialAd.Show();
                    #else
                    Debug.LogError("SDK missing. Please import Heyzap plugin.");
                    #endif

                    break;
                case AdNetwork.UnityAds:

                    #if UNITY_ADS
                    if (location == AdLocation.Default)
                    {
                        Advertisement.Show();
                    }
                    else
                    {
                        var showOptions = new ShowOptions { resultCallback = UnityAdsInterstitialCallback };
                        Advertisement.Show(location.ToUnityAdsZoneId(), showOptions);
                    }
                    #else
                    Debug.LogError("SDK missing. Please enable Unity Ads service.");
                    #endif

                    break;
            }
        }

        /// <summary>
        /// Loads the default rewarded ad.
        /// </summary>
        public static void LoadRewardedAd()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    LoadRewardedAd(EM_Settings.Advertising.AndroidDefaultAdNetworks.rewardedAdNetwork, AdLocation.Default);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    LoadRewardedAd(EM_Settings.Advertising.IosDefaultAdNetworks.rewardedAdNetwork, AdLocation.Default);
                    break;
            }
        }

        /// <summary>
        /// Loads a rewarded ad using the specified ad network, at the specified location.
        ///     - For AdColony, AdMob and Heyzap, the location will be ignored. You can pass AdLocation.Default.
        ///     - For Chartboost, select one of available locations or create a new location using AdLocation.LocationFromName(name).
        ///     - For Unity Ads, create a new location for the desired zoneId using AdLocation.LocationFromName(zoneId).
        /// If the specified network doesn't support rewarded ads, this method is a no-opt.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="location">Location.</param>
        public static void LoadRewardedAd(RewardedAdNetwork adNetwork, AdLocation location)
        {
            switch ((AdNetwork)adNetwork)
            {
                case AdNetwork.AdColony:

                    #if EM_ADCOLONY
                    if (!isAdColonyInitialized)
                        return;

                    if (adColonyRewardedAd != null)
                        return;

                    var adOptions = new EM_AdColony.AdOptions()
                    { 
                        ShowPrePopup = EM_Settings.Advertising.AdColonyShowRewardedAdPrePopup,
                        ShowPostPopup = EM_Settings.Advertising.AdColonyShowRewardedAdPostPopup
                    };

                    EM_AdColony.Ads.RequestInterstitialAd(EM_Settings.Advertising.AdColonyIds.RewardedAdId, adOptions);
                    #else
                    Debug.LogError("SDK missing. Please import AdColony plugin.");
                    #endif

                    break;
                case AdNetwork.AdMob:

                    #if EM_ADMOB
                    // Loading a new rewarded ad seems to disable all events of the currently playing ad,
                    // so we shouldn't perform rewarded ad loading while playing another one.
                    if (isAdMobRewardedAdPlaying)
                        return;

                    if (admobRewardedAd == null)
                    {
                        admobRewardedAd = RewardBasedVideoAd.Instance;

                        // RewardBasedVideoAd is a singleton, so handlers should only be registered once.
                        admobRewardedAd.OnAdLoaded += HandleAdMobRewardBasedVideoLoaded;
                        admobRewardedAd.OnAdFailedToLoad += HandleAdMobRewardBasedVideoFailedToLoad;
                        admobRewardedAd.OnAdOpening += HandleAdMobRewardBasedVideoOpened;
                        admobRewardedAd.OnAdStarted += HandleAdMobRewardBasedVideoStarted;
                        admobRewardedAd.OnAdRewarded += HandleAdMobRewardBasedVideoRewarded;
                        admobRewardedAd.OnAdClosed += HandleAdMobRewardBasedVideoClosed;
                        admobRewardedAd.OnAdLeavingApplication += HandleAdMobRewardBasedVideoLeftApplication;

                    }

                    admobRewardedAd.LoadAd(CreateAdMobAdRequest(), EM_Settings.Advertising.AdMobIds.RewardedAdId);
                    #else
                    Debug.LogError("SDK missing. Please import Google Mobile Ads plugin.");
                    #endif

                    break;
                case AdNetwork.Chartboost:

                    #if EM_CHARTBOOST
                    Chartboost.cacheRewardedVideo(location.ToChartboostLocation());
                    #else
                    Debug.LogError("SDK missing. Please import Chartboost plugin.");
                    #endif

                    break;
                case AdNetwork.Heyzap:

                    #if EM_HEYZAP
                    HZIncentivizedAd.Fetch();
                    #else
                    Debug.LogError("SDK missing. Please import Heyzap plugin.");
                    #endif

                    break;
                case AdNetwork.UnityAds:

                    // Unity Ads are loaded automatically if enabled.
                    #if !UNITY_ADS
                    Debug.LogError("SDK missing. Please enable Unity Ads service.");
                    #endif

                    break;
            }
        }

        /// <summary>
        /// Determines whether the default rewarded ad is ready to show.
        /// </summary>
        /// <returns><c>true</c> if rewarded ad ready is ready; otherwise, <c>false</c>.</returns>
        public static bool IsRewardedAdReady()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return IsRewardedAdReady(EM_Settings.Advertising.AndroidDefaultAdNetworks.rewardedAdNetwork, AdLocation.Default);
                case RuntimePlatform.IPhonePlayer:
                    return IsRewardedAdReady(EM_Settings.Advertising.IosDefaultAdNetworks.rewardedAdNetwork, AdLocation.Default);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether a rewarded ad is ready to show, using the specified ad network, at the specified location.
        ///     - For AdColony, AdMob and Heyzap, the location will be ignored. You can pass AdLocation.Default.
        ///     - For Chartboost, select one of available locations or create a new location using AdLocation.LocationFromName(name).
        ///     - For Unity Ads, create a new location for the desired zoneId using AdLocation.LocationFromName(zoneId).
        /// If the specified network doesn't support rewarded ads, this method always returns false.
        /// </summary>
        /// <returns><c>true</c> if rewarded ad is ready; otherwise, <c>false</c>.</returns>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="location">Location.</param>
        public static bool IsRewardedAdReady(RewardedAdNetwork adNetwork, AdLocation location)
        {
            switch ((AdNetwork)adNetwork)
            {
                case AdNetwork.AdColony:

                    #if EM_ADCOLONY
                    return (isAdColonyInitialized && adColonyRewardedAd != null);
                    #else
                    return false;
                    #endif

                case AdNetwork.AdMob:

                    #if EM_ADMOB
                    return (admobRewardedAd != null && admobRewardedAd.IsLoaded());
                    #else
                    return false;
                    #endif

                case AdNetwork.Chartboost:

                    #if EM_CHARTBOOST
                    return Chartboost.hasRewardedVideo(location.ToChartboostLocation());
                    #else
                    return false;
                    #endif

                case AdNetwork.Heyzap:

                    #if EM_HEYZAP
                    return HZIncentivizedAd.IsAvailable();
                    #else
                    return false;
                    #endif

                case AdNetwork.UnityAds:

                    #if UNITY_ADS
                    return Advertisement.IsReady(UNITYADS_REWARDED_ZONE_ID);
                    #else
                    return false;
                    #endif

                default:
                    return false;
            }
        }

        /// <summary>
        /// Shows the default rewarded ad.
        /// </summary>
        public static void ShowRewardedAd()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    ShowRewardedAd(EM_Settings.Advertising.AndroidDefaultAdNetworks.rewardedAdNetwork, AdLocation.Default);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    ShowRewardedAd(EM_Settings.Advertising.IosDefaultAdNetworks.rewardedAdNetwork, AdLocation.Default);
                    break;
            }
        }

        /// <summary>
        /// Shows a rewarded ad using the specified ad network, at the specified location.
        ///     - For AdColony, AdMob and Heyzap, the location will be ignored. You can pass AdLocation.Default.
        ///     - For Chartboost, select one of available locations or create a new location using AdLocation.LocationFromName(name).
        ///     - For Unity Ads, create a new location for the desired zoneId using AdLocation.LocationFromName(zoneId).
        /// If the specified network doesn't support rewarded ads, this method is a no-opt.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="location">Location.</param>
        public static void ShowRewardedAd(RewardedAdNetwork adNetwork, AdLocation location)
        { 
            // Note that we don't check if ads were removed because rewarded should still be available after ad removal.
            if (!IsRewardedAdReady(adNetwork, location))
            {
                Debug.Log("ShowRewardedAd FAILED: Rewarded ad is not loaded.");
                return;
            }                

            switch ((AdNetwork)adNetwork)
            {
                case AdNetwork.AdColony:

                    #if EM_ADCOLONY
                    EM_AdColony.Ads.ShowAd(adColonyRewardedAd);
                    adColonyRewardedAd = null;
                    #else
                    Debug.LogError("SDK missing. Please import AdColony plugin.");
                    #endif

                    break;
                case AdNetwork.AdMob:

                    #if EM_ADMOB
                    isAdMobRewardedAdPlaying = true;
                    admobRewardedAd.Show();
                    #else
                    Debug.LogError("SDK missing. Please import Google Mobile Ads plugin.");
                    #endif

                    break;
                case AdNetwork.Chartboost:

                    #if EM_CHARTBOOST
                    Chartboost.showRewardedVideo(location.ToChartboostLocation());
                    #else
                    Debug.LogError("SDK missing. Please import Chartboost plugin.");
                    #endif

                    break;
                case AdNetwork.Heyzap:

                    #if EM_HEYZAP
                    HZIncentivizedAd.Show();
                    #else
                    Debug.LogError("SDK missing. Please import Heyzap plugin.");
                    #endif

                    break;
                case AdNetwork.UnityAds:

                    #if UNITY_ADS
                    var showOptions = new ShowOptions { resultCallback = UnityAdsRewardedAdCallback };
                    Advertisement.Show(UNITYADS_REWARDED_ZONE_ID, showOptions);
                    #else
                    Debug.LogError("SDK missing. Please enable Unity Ads service.");
                    #endif

                    break;
            }
        }

        /// <summary>
        /// Determines whether ads were removed.
        /// </summary>
        /// <returns><c>true</c> if ads were removed; otherwise, <c>false</c>.</returns>
        public static bool IsAdRemoved()
        {
            return (PlayerPrefs.GetInt(AD_REMOVE_STATUS_PPKEY, AD_ENABLED) == AD_DISABLED);
        }

        /// <summary>
        /// Removes ads permanently. Use this for the RemoveAds button.
        /// This will hide the default banner ad if it is being shown and
        /// prohibit future loading and showing of all ads except rewarded ads.
        /// </summary>
        public static void RemoveAds()
        {
            Debug.Log("******* REMOVING ADS... *******");

            // Destroy the default banner ad if any
            DestroyBannerAd();

            // Update ad availability
            PlayerPrefs.SetInt(AD_REMOVE_STATUS_PPKEY, AD_DISABLED);
            PlayerPrefs.Save();

            // Fire event
            AdsRemoved();
        }

        /// <summary>
        /// Resets the remove ads status, allows showing ads again.
        /// </summary>
        public static void ResetRemoveAds()
        {
            Debug.Log("******* RESET REMOVE ADS STATUS... *******");

            PlayerPrefs.SetInt(AD_REMOVE_STATUS_PPKEY, AD_ENABLED);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// This coroutine regularly checks if intersititial and rewarded ads are loaded, if they aren't
        /// it will automatically perform loading.
        /// If ads were removed, other ads will no longer be loaded except rewarded ads since they are
        /// shown under user discretion and therefore can still possibly be used even if ads were removed.
        /// </summary>
        private static IEnumerator CRAutoLoadAds()
        {
            while (true)
            {               
                foreach (AdType type in Enum.GetValues(typeof(AdType)))
                {
                    switch (type)
                    {
                        case AdType.Interstitial:
                            if (!IsInterstitialAdReady() && !IsAdRemoved())
                            {
                                if (Time.realtimeSinceStartup - lastInterstitialAdLoadTimestamp >= EM_Settings.Advertising.AdLoadingInterval)
                                {
                                    LoadInterstitialAd();
                                    lastInterstitialAdLoadTimestamp = Time.realtimeSinceStartup;
                                }
                            }
                            break;
                        case AdType.Rewarded:
                            if (!IsRewardedAdReady())
                            {
                                if (Time.realtimeSinceStartup - lastRewardedAdLoadTimestamp >= EM_Settings.Advertising.AdLoadingInterval)
                                {
                                    LoadRewardedAd();
                                    lastRewardedAdLoadTimestamp = Time.realtimeSinceStartup;
                                }
                            }
                            break;
                        default:
                            break;
                    }         
                }

                yield return new WaitForSeconds(EM_Settings.Advertising.AdCheckingInterval);
            }
        }

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

        #if EM_ADMOB
        private static AdRequest CreateAdMobAdRequest()
        {
            AdRequest.Builder adBuilder = new AdRequest.Builder();

            // Test mode
            if (EM_Settings.Advertising.AdMobEnableTestMode)
            {
                // Add all emulators
                adBuilder.AddTestDevice(AdRequest.TestDeviceSimulator);

                // Add user-specified test devices
                for (int i = 0; i < EM_Settings.Advertising.AdMobTestDeviceIds.Length; i++)
                    adBuilder.AddTestDevice(EM_Settings.Advertising.AdMobTestDeviceIds[i]);
            }

            return adBuilder.Build();
        }
        #endif

        #region Unity Ads Interstitial & RewardedAd callback handlers

        #if UNITY_ADS
        static void UnityAdsInterstitialCallback(ShowResult result)
        {
            switch (result)
            {
                case ShowResult.Finished:
                    Debug.Log("Unity Ads interstitial ad was completed.");
                    // Fire event
                    InterstitialAdCompleted(InterstitialAdNetwork.UnityAds, AdLocation.Default);
                    break;
                case ShowResult.Skipped:
                    Debug.Log("Unity Ads interstitial ad was skipped before reaching the end.");
                    break;
                case ShowResult.Failed:
                    Debug.LogError("Unity Ads interstitial ad failed to be shown.");
                    break;
            }
        }

        static void UnityAdsRewardedAdCallback(ShowResult result)
        {
            switch (result)
            {
                case ShowResult.Finished:
                    Debug.Log("Unity Ads rewarded video ad was completed.");
                    // Fire event
                    RewardedAdCompleted(RewardedAdNetwork.UnityAds, AdLocation.Default);
                    break;
                case ShowResult.Skipped:
                    Debug.Log("Unity Ads rewarded video ad was skipped before reaching the end.");
                    RewardedAdSkipped(RewardedAdNetwork.UnityAds, AdLocation.Default);
                    break;
                case ShowResult.Failed:
                    Debug.LogError("Unity Ads rewarded video ad failed to be shown.");
                    break;
            }
        }
        #endif

        #endregion

        #region Heyzap callback handlers

        #if EM_HEYZAP
        
        void HeyzapBannerAdCallback(string adState, string adTag)
        {
            if (adState == "loaded")
            {
                // Do something when the banner ad is loaded
                Debug.Log("Heyzap banner ad was loaded.");
            }
            if (adState == "error")
            {
                // Do something when the banner ad fails to load (they can fail when refreshing after successfully loading)
                Debug.Log("Heyzap banner ad failed to load.");
            }
            if (adState == "click")
            {
                // Do something when the banner ad is clicked, like pause your game
                Debug.Log("Heyzap banner ad was clicked.");
            }
        }

        void HeyzapInterstitialAdCallback(string adState, string adTag)
        {
            if (adState.Equals("show"))
            {
                // Sent when an ad has been displayed.
                // This is a good place to pause your app, if applicable.
                Debug.Log("Heyzap interstitial ad was shown.");
            }
            else if (adState.Equals("hide"))
            {
                // Sent when an ad has been removed from view.
                // This is a good place to unpause your app, if applicable.
                Debug.Log("Heyzap interstitial ad was closed.");

                // Fire event
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
                Debug.Log("Heyzap interstitial ad failed to show.");
            }
            else if (adState.Equals("available"))
            {
                // Sent when an ad has been loaded and is ready to be displayed,
                //   either because we autofetched an ad or because you called
                //   `Fetch`.
                Debug.Log("Heyzap interstitial ad was loaded.");
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

        void HeyzapRewardedAdCallback(string adState, string adTag)
        {
            if (adState.Equals("incentivized_result_complete"))
            {
                // The user has watched the entire video and should be given a reward.
                Debug.Log("Heyzap rewarded video ad was completed.");

                // Fire event
                RewardedAdCompleted(RewardedAdNetwork.Heyzap, AdLocation.Default);
            }
            else if (adState.Equals("incentivized_result_incomplete"))
            {
                // The user did not watch the entire video and should not be given a reward.
                Debug.Log("Heyzap rewarded video ad was not completed.");

                // Fire event
                RewardedAdSkipped(RewardedAdNetwork.Heyzap, AdLocation.Default);
            }
        }

        #endif

        #endregion

        #region Chartboost callback handlers

        #if EM_CHARTBOOST
        // Chartboost interstitial ad callback handlers
        static void CBDidCacheInterstitial(CBLocation location)
        {
            Debug.Log("Chartboost interstitial ad was loaded successfully.");
        }

        static void CBDidDisplayInterstitial(CBLocation location)
        {
            Debug.Log("Chartboost interstitial ad has been displayed.");
        }

        static void CBDidClickInterstitial(CBLocation location)
        {
            Debug.Log("Chartboost interstitial ad was clicked.");
        }

        static void CBDidCloseInterstitial(CBLocation location)
        {
            Debug.Log("Chartboost interstitial ad was closed.");
        }

        static void CBDidDismissInterstitial(CBLocation location)
        {
            Debug.Log("Chartboost interstitial ad was dismissed.");

            // Fire event
            InterstitialAdCompleted(InterstitialAdNetwork.Chartboost, AdLocation.LocationFromName(location.ToString()));
        }

        static void CBDidFailToLoadInterstitial(CBLocation location, CBImpressionError error)
        {
            Debug.Log("Chartboost interstitial ad failed to load.");
        }

        // Chartboost rewarded ad callback handler
        static void CBDidCacheRewardedVideo(CBLocation location)
        {
            Debug.Log("Chartboost rewarded video ad was loaded successfully.");
        }

        static void CBDidFailToLoadRewardedVideo(CBLocation location, CBImpressionError error)
        {
            Debug.Log("Chartboost rewarded video ad failed to load.");
        }

        static void CBDidCompleteRewardedVideo(CBLocation location, int reward)
        {
            Debug.Log("Chartboost rewarded video ad was completed.");
            isCBRewardedAdCompleted = true;
        }

        static void CBDidClickRewardedVideo(CBLocation location)
        {
            Debug.Log("Chartboost rewarded video ad was clicked.");
        }

        static void CBDidCloseRewardedVideo(CBLocation location)
        {
            Debug.Log("Chartboost rewarded video ad was closed.");

            if (isCBRewardedAdCompleted)
            {
                isCBRewardedAdCompleted = false;

                // Fire event
                RewardedAdCompleted(RewardedAdNetwork.Chartboost, AdLocation.LocationFromName(location.ToString()));
            }
            else
            {
                RewardedAdSkipped(RewardedAdNetwork.Chartboost, AdLocation.LocationFromName(location.ToString()));
            }
        }
        #endif

        #endregion

        #region AdMob callback handlers

        #if EM_ADMOB
        // AdMob banner ad callback handlers
        static void HandleAdMobBannerAdLoaded(object sender, EventArgs args)
        {
            Debug.Log("AdMob banner ad was loaded successfully.");
        }

        static void HandleAdMobBannerAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log("AdMob banner ad failed to load.");
        }

        static void HandleAdMobBannerAdOpened(object sender, EventArgs args)
        {
            Debug.Log("AdMob banner ad was clicked.");
        }

        static void HandleAdMobBannerAdClosed(object sender, EventArgs args)
        {
            Debug.Log("AdMob banner ad was closed.");
        }

        static void HandleAdMobBannerAdLeftApplication(object sender, EventArgs args)
        {
            Debug.Log("HandleAdMobBannerAdLeftApplication event received");
        }

        // AdMob interstitial ad callback handlers
        static void HandleAdMobInterstitialLoaded(object sender, EventArgs args)
        {
            Debug.Log("AdMob interstitial ad was loaded successfully.");
        }

        static void HandleAdMobInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log("AdMob interstitial ad failed to load.");
        }

        static void HandleAdMobInterstitialOpened(object sender, EventArgs args)
        {
            Debug.Log("AdMob interstitial ad was clicked.");
        }

        static void HandleAdMobInterstitialClosed(object sender, EventArgs args)
        {
            Debug.Log("AdMob interstitial ad was closed.");
            isAdMobInterstitialClosed = true;
        }

        static void HandleAdMobInterstitialLeftApplication(object sender, EventArgs args)
        {
            Debug.Log("HandleAdMobInterstitialLeftApplication event received");
        }

        // AdMob rewarded ad callback handlers
        static void HandleAdMobRewardBasedVideoLoaded(object sender, EventArgs args)
        {
            Debug.Log("AdMob rewarded video ad was loaded successfully.");
        }

        static void HandleAdMobRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log("AdMob rewarded video ad failed to load.");
        }

        static void HandleAdMobRewardBasedVideoOpened(object sender, EventArgs args)
        {
            Debug.Log("AdMob rewarded video ad was clicked.");
        }

        static void HandleAdMobRewardBasedVideoStarted(object sender, EventArgs args)
        {
            Debug.Log("AdMob rewarded video ad has started.");
        }

        static void HandleAdMobRewardBasedVideoClosed(object sender, EventArgs args)
        {
            Debug.Log("AdMob rewarded video ad was closed.");
            isAdMobRewardedAdClosed = true;
        }

        static void HandleAdMobRewardBasedVideoRewarded(object sender, Reward args)
        {
            Debug.Log("AdMob rewarded video ad was completed.");
            isAdMobRewardedAdCompleted = true;
        }

        static void HandleAdMobRewardBasedVideoLeftApplication(object sender, EventArgs args)
        {
            Debug.Log("HandleRewardBasedVideoLeftApplication event received");
        }
        #endif

        #endregion

        #region AdColony callback handlers

        #if EM_ADCOLONY
        private void OnAdColonyConfigurationCompleted(List<EM_AdColony.Zone> obj)
        {
            Debug.Log("AdColony configuration completed.");
            isAdColonyInitialized = true;
        }

        private void OnAdColonyRequestAdFailedWithZone(string zoneId)
        {
            Debug.Log("AdColony request ad failed with zoneId: " + zoneId);
        }

        private void OnAdColonyRequestAdCompleted(EM_AdColony.InterstitialAd ad)
        {
            Debug.Log("AdColony request ad complete.");

            var adColonyIds = EM_Settings.Advertising.AdColonyIds;

            if (ad.ZoneId.Equals(adColonyIds.InterstitialAdId))
                adColonyInterstitialAd = ad;
            else if (ad.ZoneId.Equals(adColonyIds.RewardedAdId))
                adColonyRewardedAd = ad;
        }

        private void OnAdColonyAdOpened(EM_AdColony.InterstitialAd ad)
        {
            Debug.Log("AdColony ad opened.");
        }

        private void OnAdColonyAdClosed(EM_AdColony.InterstitialAd ad)
        {
            Debug.Log("AdColony ad closed.");

            var adColonyIds = EM_Settings.Advertising.AdColonyIds;

            if (ad.ZoneId.Equals(adColonyIds.InterstitialAdId))
            {
                // Fire event
                InterstitialAdCompleted(InterstitialAdNetwork.AdColony, AdLocation.Default);
            }
            else if (ad.ZoneId.Equals(adColonyIds.RewardedAdId) && isAdColonyRewardedAdCompleted)
            {
                if (isAdColonyRewardedAdCompleted)
                {
                    isAdColonyRewardedAdCompleted = false;

                    // Fire event
                    RewardedAdCompleted(RewardedAdNetwork.AdColony, AdLocation.Default);
                }
                else
                {
                    RewardedAdSkipped(RewardedAdNetwork.AdColony, AdLocation.Default);
                }
            }
        }

        private void OnAdColonyRewardGranted(string zoneId, bool success, string rewardName, int rewardQuantity)
        {
            Debug.Log("AdColony reward granted.");

            var adColonyIds = EM_Settings.Advertising.AdColonyIds;

            if (zoneId.Equals(adColonyIds.RewardedAdId))
                isAdColonyRewardedAdCompleted = true;
        }
        #endif

        #endregion
    }
}



