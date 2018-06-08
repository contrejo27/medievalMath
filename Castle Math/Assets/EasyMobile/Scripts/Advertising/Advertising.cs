using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using EasyMobile.AdvertisingInternal;

namespace EasyMobile
{
    [AddComponentMenu("")]
    public class Advertising : MonoBehaviour
    {
        public static Advertising Instance { get; private set; }

        //------------------------------------------------------------
        // Events.
        //------------------------------------------------------------

        /// <summary>
        /// Occurs when an interstitial ad is closed.
        /// </summary>
        public static event Action<InterstitialAdNetwork, AdLocation> InterstitialAdCompleted;

        /// <summary>
        /// Occurs when a rewarded ad is skipped (the user didn't complete watching
        /// the ad and therefore is not entitled to the reward).
        /// </summary>
        public static event Action<RewardedAdNetwork, AdLocation> RewardedAdSkipped;

        /// <summary>
        /// Occurs when a rewarded ad completed and the user should be rewarded.
        /// </summary>
        public static event Action<RewardedAdNetwork, AdLocation> RewardedAdCompleted;

        /// <summary>
        /// Occurs when ads have been removed.
        /// </summary>
        public static event Action AdsRemoved;

        //------------------------------------------------------------
        // Internal ad clients.
        //------------------------------------------------------------
        private static AdColonyClientImpl AdColonyClient
        {
            get
            {
                if (sAdColonyClient == null)
                {
                    sAdColonyClient = CreateAdClient(AdNetwork.AdColony) as AdColonyClientImpl;
                    SubscribeAdClientEvents(sAdColonyClient);
                }   
                return sAdColonyClient;
            }
        }

        private static AdMobClientImpl AdMobClient
        {
            get
            {
                if (sAdMobClient == null)
                {
                    sAdMobClient = CreateAdClient(AdNetwork.AdMob) as AdMobClientImpl;
                    SubscribeAdClientEvents(sAdMobClient);
                }   
                return sAdMobClient;
            }
        }

        private static ChartboostClientImpl ChartboostClient
        {
            get
            {
                if (sChartboostClient == null)
                {
                    sChartboostClient = CreateAdClient(AdNetwork.Chartboost) as ChartboostClientImpl;
                    SubscribeAdClientEvents(sChartboostClient);
                }   
                return sChartboostClient;
            }
        }

        private static HeyzapClientImpl HeyzapClient
        {
            get
            {
                if (sHeyzapClient == null)
                {
                    sHeyzapClient = CreateAdClient(AdNetwork.Heyzap) as HeyzapClientImpl;
                    SubscribeAdClientEvents(sHeyzapClient);
                }   
                return sHeyzapClient;
            }
        }

        private static UnityAdsClientImpl UnityAdsClient
        {
            get
            {
                if (sUnityAdsClient == null)
                {
                    sUnityAdsClient = CreateAdClient(AdNetwork.UnityAds) as UnityAdsClientImpl;
                    SubscribeAdClientEvents(sUnityAdsClient);
                }   
                return sUnityAdsClient;
            }
        }

        private static IAdClient DefaultBannerAdClient
        {
            get
            {
                if (sDefaultBannerAdClient == null)
                {
                    switch (Application.platform)
                    {
                        case RuntimePlatform.Android:
                            sDefaultBannerAdClient = SelectAdClient((AdNetwork)EM_Settings.Advertising.AndroidDefaultAdNetworks.bannerAdNetwork);
                            break;
                        case RuntimePlatform.IPhonePlayer:
                            sDefaultBannerAdClient = SelectAdClient((AdNetwork)EM_Settings.Advertising.IosDefaultAdNetworks.bannerAdNetwork);
                            break;
                        default:
                            sDefaultBannerAdClient = SelectAdClient(AdNetwork.None);
                            break;
                    }
                }
                return sDefaultBannerAdClient;
            }
        }

        private static IAdClient DefaultInterstitialAdClient
        {
            get
            {
                if (sDefaultInterstitialAdClient == null)
                {
                    switch (Application.platform)
                    {
                        case RuntimePlatform.Android:
                            sDefaultInterstitialAdClient = SelectAdClient((AdNetwork)EM_Settings.Advertising.AndroidDefaultAdNetworks.interstitialAdNetwork);
                            break;
                        case RuntimePlatform.IPhonePlayer:
                            sDefaultInterstitialAdClient = SelectAdClient((AdNetwork)EM_Settings.Advertising.IosDefaultAdNetworks.interstitialAdNetwork);
                            break;
                        default:
                            sDefaultInterstitialAdClient = SelectAdClient(AdNetwork.None);
                            break;
                    }
                }
                return sDefaultInterstitialAdClient;
            }
        }

        private static IAdClient DefaultRewardedAdClient
        {
            get
            {
                if (sDefaultRewardedAdClient == null)
                {
                    switch (Application.platform)
                    {
                        case RuntimePlatform.Android:
                            sDefaultRewardedAdClient = SelectAdClient((AdNetwork)EM_Settings.Advertising.AndroidDefaultAdNetworks.rewardedAdNetwork);
                            break;
                        case RuntimePlatform.IPhonePlayer:
                            sDefaultRewardedAdClient = SelectAdClient((AdNetwork)EM_Settings.Advertising.IosDefaultAdNetworks.rewardedAdNetwork);
                            break;
                        default:
                            sDefaultRewardedAdClient = SelectAdClient(AdNetwork.None);
                            break;
                    }
                }
                return sDefaultRewardedAdClient;
            }
        }

        // Supported ad clients.
        private static AdColonyClientImpl sAdColonyClient;
        private static AdMobClientImpl sAdMobClient;
        private static ChartboostClientImpl sChartboostClient;
        private static HeyzapClientImpl sHeyzapClient;
        private static UnityAdsClientImpl sUnityAdsClient;

        // Default ad clients for each ad types.
        private static IAdClient sDefaultBannerAdClient;
        private static IAdClient sDefaultInterstitialAdClient;
        private static IAdClient sDefaultRewardedAdClient;

        //------------------------------------------------------------
        // Private data.
        //------------------------------------------------------------

        private static float lastInterstitialAdLoadTimestamp = -1000f;
        private static float lastRewardedAdLoadTimestamp = -1000f;

        // For storing removeAds status.
        private const string AD_REMOVE_STATUS_PPKEY = "EM_REMOVE_ADS";
        private const int AD_ENABLED = 1;
        private const int AD_DISABLED = -1;

        // Auto load ads coroutine.
        private static IEnumerator autoLoadAdsCoroutine;
        private static bool isAutoLoadDefaultAds;

        void Awake()
        {
            if (Instance != null)
                Destroy(this);
            else
                Instance = this;
        }

        void Start()
        {
            // Show Heyzap Test Suite if needed.
            #if EM_HEYZAP
            if (EM_Settings.Advertising.HeyzapShowTestSuite)
                HeyzapClient.ShowTestSuite();
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
        }

        #region Public API

        //------------------------------------------------------------
        // Auto Ad-Loading.
        //------------------------------------------------------------

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

        //------------------------------------------------------------
        // Banner Ads.
        //------------------------------------------------------------

        /// <summary>
        /// Shows a banner ad of the default banner ad network.
        /// </summary>
        /// <param name="position">Position.</param>
        public static void ShowBannerAd(BannerAdPosition position)
        {
            ShowBannerAd(DefaultBannerAdClient, position, BannerAdSize.SmartBanner);
        }

        /// <summary>
        /// Shows a banner ad of the default banner ad network at the specified position and size.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="size">Ad size, applicable for AdMob banner only.</param>
        public static void ShowBannerAd(BannerAdPosition position, BannerAdSize size)
        {
            ShowBannerAd(DefaultBannerAdClient, position, size);
        }

        /// <summary>
        /// Shows a banner ad of the specified ad network at the specified position and size.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="position">Position.</param>
        /// <param name="size">Ad size, applicable for AdMob banner only.</param>
        public static void ShowBannerAd(BannerAdNetwork adNetwork, BannerAdPosition position, BannerAdSize size)
        {
            ShowBannerAd(SelectAdClient((AdNetwork)adNetwork), position, size);
        }

        /// <summary>
        /// Hides banner ad of the default banner ad network.
        /// </summary>
        public static void HideBannerAd()
        {
            HideBannerAd(DefaultBannerAdClient);
        }

        /// <summary>
        /// Hides banner ad of the specified ad network if one is shown, otherwise this method is a no-op.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        public static void HideBannerAd(BannerAdNetwork adNetwork)
        {
            HideBannerAd(SelectAdClient((AdNetwork)adNetwork));
        }

        /// <summary>
        /// Destroys the banner ad of the default banner ad network.
        /// </summary>
        public static void DestroyBannerAd()
        {
            DestroyBannerAd(DefaultBannerAdClient);
        }

        /// <summary>
        /// Destroys the banner ad of the specified ad network.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        public static void DestroyBannerAd(BannerAdNetwork adNetwork)
        {
            DestroyBannerAd(SelectAdClient((AdNetwork)adNetwork));
        }

        //------------------------------------------------------------
        // Interstitial Ads.
        //------------------------------------------------------------

        /// <summary>
        /// Loads the default interstitial ad.
        /// </summary>
        public static void LoadInterstitialAd()
        {
            LoadInterstitialAd(DefaultInterstitialAdClient, AdLocation.Default);
        }

        /// <summary>
        /// Loads an interstitial ad from the specified interstitial ad network at the specified location.
        ///     - For AdColony, AdMob and Heyzap, the location will be ignored. You can pass <see cref="AdLocation.Default"/>.
        ///     - For Chartboost, select one of available locations or create a new location using <see cref="AdLocation.LocationFromName(name)"/>.
        ///     - For Unity Ads, use <see cref="AdLocation.Default"/> or create a new location for the desired zoneId using <see cref="AdLocation.LocationFromName(zoneId)"/>.
        /// If the specified network doesn't support interstitial ads, this method is a no-op.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="location">Location.</param>
        public static void LoadInterstitialAd(InterstitialAdNetwork adNetwork, AdLocation location)
        {
            LoadInterstitialAd(SelectAdClient((AdNetwork)adNetwork), location);
        }

        /// <summary>
        /// Determines whether the default interstitial ad is ready to show.
        /// </summary>
        /// <returns><c>true</c> if the default interstitial ad is ready; otherwise, <c>false</c>.</returns>
        public static bool IsInterstitialAdReady()
        {
            return IsInterstitialAdReady(DefaultInterstitialAdClient, AdLocation.Default);
        }

        /// <summary>
        /// Determines whether an interstitial ad of the specified ad network, at the specified location, is ready to show.
        ///     - For AdColony, AdMob and Heyzap, the location will be ignored. You can pass <see cref="AdLocation.Default"/>.
        ///     - For Chartboost, select one of available locations or create a new location using <see cref="AdLocation.LocationFromName(name)"/>.
        ///     - For Unity Ads, use <see cref="AdLocation.Default"/> or create a new location for the desired zoneId using <see cref="AdLocation.LocationFromName(zoneId)"/>.
        /// If the specified network doesn't support interstitial ads, this method always returns false.
        /// </summary>
        /// <returns><c>true</c> if interstitial ad is ready for the specified location; otherwise, <c>false</c>.</returns>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="location">Location.</param>
        public static bool IsInterstitialAdReady(InterstitialAdNetwork adNetwork, AdLocation location)
        {
            return IsInterstitialAdReady(SelectAdClient((AdNetwork)adNetwork), location);
        }

        /// <summary>
        /// Shows the default interstitial ad.
        /// </summary>
        public static void ShowInterstitialAd()
        {
            ShowInterstitialAd(DefaultInterstitialAdClient, AdLocation.Default);
        }

        /// <summary>
        /// Shows an interstitial ad of the specified ad network, at the specified location.
        ///     - For AdColony, AdMob and Heyzap, the location will be ignored. You can pass <see cref="AdLocation.Default"/>. 
        ///     - For Chartboost, select one of available locations or create a new location using <see cref="AdLocation.LocationFromName(name)"/>.
        ///     - For Unity Ads, use <see cref="AdLocation.Default"/> or create a new location for the desired zoneId using <see cref="AdLocation.LocationFromName(zoneId)"/>.
        /// If the specified network doesn't support interstitial ads, this method is a no-op.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="location">Location.</param>
        public static void ShowInterstitialAd(InterstitialAdNetwork adNetwork, AdLocation location)
        {
            ShowInterstitialAd(SelectAdClient((AdNetwork)adNetwork), location);
        }

        //------------------------------------------------------------
        // Rewarded Ads.
        //------------------------------------------------------------

        /// <summary>
        /// Loads the default rewarded ad.
        /// </summary>
        public static void LoadRewardedAd()
        {
            LoadRewardedAd(DefaultRewardedAdClient, AdLocation.Default);
        }

        /// <summary>
        /// Loads a rewarded ad from the specified ad network, at the specified location.
        ///     - For AdColony, AdMob and Heyzap, the location will be ignored. You can pass <see cref="AdLocation.Default"/>.
        ///     - For Chartboost, select one of available locations or create a new location using <see cref="AdLocation.LocationFromName(name)"/>.
        ///     - For Unity Ads, use <see cref="AdLocation.Default"/> or create a new location for the desired zoneId using <see cref="AdLocation.LocationFromName(zoneId)"/>.
        /// If the specified network doesn't support rewarded ads, this method is a no-op.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="location">Location.</param>
        public static void LoadRewardedAd(RewardedAdNetwork adNetwork, AdLocation location)
        {
            LoadRewardedAd(SelectAdClient((AdNetwork)adNetwork), location);
        }

        /// <summary>
        /// Determines whether the default rewarded ad is ready to show.
        /// </summary>
        /// <returns><c>true</c> if rewarded ad ready is ready; otherwise, <c>false</c>.</returns>
        public static bool IsRewardedAdReady()
        {
            return IsRewardedAdReady(DefaultRewardedAdClient, AdLocation.Default);
        }

        /// <summary>
        /// Determines whether a rewarded ad of the specified ad network, at the specified location, is ready to show.
        ///     - For AdColony, AdMob and Heyzap, the location will be ignored. You can pass <see cref="AdLocation.Default"/>.
        ///     - For Chartboost, select one of available locations or create a new location using <see cref="AdLocation.LocationFromName(name)"/>.
        ///     - For Unity Ads, use <see cref="AdLocation.Default"/> or create a new location for the desired zoneId using <see cref="AdLocation.LocationFromName(zoneId)"/>.
        /// If the specified network doesn't support rewarded ads, this method always returns false.
        /// </summary>
        /// <returns><c>true</c> if rewarded ad is ready; otherwise, <c>false</c>.</returns>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="location">Location.</param>
        public static bool IsRewardedAdReady(RewardedAdNetwork adNetwork, AdLocation location)
        {
            return IsRewardedAdReady(SelectAdClient((AdNetwork)adNetwork), location);
        }

        /// <summary>
        /// Shows the default rewarded ad.
        /// </summary>
        public static void ShowRewardedAd()
        {
            ShowRewardedAd(DefaultRewardedAdClient, AdLocation.Default);
        }

        /// <summary>
        /// Shows a rewarded ad of the specified ad network, at the specified location.
        ///     - For AdColony, AdMob and Heyzap, the location will be ignored. You can pass <see cref="AdLocation.Default"/>.
        ///     - For Chartboost, select one of available locations or create a new location using <see cref="AdLocation.LocationFromName(name)"/>.
        ///     - For Unity Ads, use <see cref="AdLocation.Default"/> or create a new location for the desired zoneId using <see cref="AdLocation.LocationFromName(zoneId)"/>.
        /// If the specified network doesn't support rewarded ads, this method is a no-op.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="location">Location.</param>
        public static void ShowRewardedAd(RewardedAdNetwork adNetwork, AdLocation location)
        { 
            ShowRewardedAd(SelectAdClient((AdNetwork)adNetwork), location);
        }

        //------------------------------------------------------------
        // Ads Removal.
        //------------------------------------------------------------

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
        /// Note that this method uses PlayerPrefs to store the ad removal status with no encryption/scrambling.
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
            if (AdsRemoved != null)
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

        #endregion // Public API

        #region Private Methods

        // This coroutine regularly checks if intersititial and rewarded ads are loaded, if they aren't
        // it will automatically perform loading.
        // If ads were removed, other ads will no longer be loaded except rewarded ads since they are
        // shown under user discretion and therefore can still possibly be used even if ads were removed.
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

        private static void ShowBannerAd(IAdClient client, BannerAdPosition position, BannerAdSize size)
        {
            if (IsAdRemoved())
            {
                Debug.Log("Could not show banner ad: ads were disabled by RemoveAds().");
                return;
            }

            client.ShowBannerAd(position, size);
        }

        private static void HideBannerAd(IAdClient client)
        {
            client.HideBannerAd();
        }

        private static void DestroyBannerAd(IAdClient client)
        {
            client.DestroyBannerAd();
        }

        private static void LoadInterstitialAd(IAdClient client, AdLocation location)
        {
            if (IsAdRemoved())
                return;
            
            client.LoadInterstitialAd(location);
        }

        private static bool IsInterstitialAdReady(IAdClient client, AdLocation location)
        {
            if (IsAdRemoved())
                return false;
            
            return client.IsInterstitialAdReady(location);
        }

        private static void ShowInterstitialAd(IAdClient client, AdLocation location)
        {
            if (IsAdRemoved())
            {
                Debug.Log("Could not show interstitial ad: ads were disabled by RemoveAds().");
                return;
            }

            client.ShowInterstitialAd(location);
        }

        private static void LoadRewardedAd(IAdClient client, AdLocation location)
        {
            client.LoadRewardedAd(location);
        }

        private static bool IsRewardedAdReady(IAdClient client, AdLocation location)
        {
            return client.IsRewardedAdReady(location);
        }

        private static void ShowRewardedAd(IAdClient client, AdLocation location)
        {
            // Note that we don't check if ads were removed because 
            // rewarded ads should still be available after ads removal.
            client.ShowRewardedAd(location);
        }

        static void SubscribeAdClientEvents(IAdClient client)
        {
            if (client == null)
                return;
            
            client.InterstitialAdCompleted += OnInternalInterstitialAdCompleted;
            client.RewardedAdSkipped += OnInternalRewardedAdSkipped;
            client.RewardedAdCompleted += OnInternalRewardedAdCompleted;
        }

        private static void OnInternalInterstitialAdCompleted(InterstitialAdNetwork network, AdLocation location)
        {
            if (InterstitialAdCompleted != null)
                InterstitialAdCompleted(network, location);
        }

        private static void OnInternalRewardedAdSkipped(RewardedAdNetwork network, AdLocation location)
        {
            if (RewardedAdSkipped != null)
                RewardedAdSkipped(network, location);
        }

        private static void OnInternalRewardedAdCompleted(RewardedAdNetwork network, AdLocation location)
        {
            if (RewardedAdCompleted != null)
                RewardedAdCompleted(network, location);
        }

        // Gets the singleton client for the specified network.
        private static IAdClient CreateAdClient(AdNetwork network)
        {
            var settings = EM_Settings.Advertising;

            switch (network)
            {
                case AdNetwork.AdColony:
                    return AdColonyClientImpl.CreateClient(settings);
                case AdNetwork.AdMob:
                    return AdMobClientImpl.CreateClient(settings);
                case AdNetwork.Chartboost:
                    return ChartboostClientImpl.CreateClient(settings);
                case AdNetwork.Heyzap:
                    return HeyzapClientImpl.CreateClient(settings);
                case AdNetwork.UnityAds:
                    return UnityAdsClientImpl.CreateClient(settings);
                case AdNetwork.None:
                    return NoOpClientImpl.CreateClient();
                default:
                    throw new NotImplementedException("No client implemented for the network:" + network.ToString());
            }
        }

        // Selects one of the created ad client corresponding to the specified network.
        private static IAdClient SelectAdClient(AdNetwork network)
        {
            switch (network)
            {
                case AdNetwork.AdColony:
                    return AdColonyClient;
                case AdNetwork.AdMob:
                    return AdMobClient;
                case AdNetwork.Chartboost:
                    return ChartboostClient;
                case AdNetwork.Heyzap:
                    return HeyzapClient;
                case AdNetwork.UnityAds:
                    return UnityAdsClient;
                case AdNetwork.None:
                    return NoOpClientImpl.CreateClient();
                default:
                    throw new NotImplementedException("No client found for the network:" + network.ToString());
            }
        }

        #endregion // Private Methods
    }
}



