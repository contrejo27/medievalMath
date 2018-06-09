using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using UnityEngine.SceneManagement;
using EasyMobile;

namespace EasyMobile.Demo
{
    public class AdvertisingDemo : MonoBehaviour
    {
        public GameObject curtain;
        public GameObject isAutoLoadInfo;
        public GameObject isAdRemovedInfo;
        public Text defaultBannerAdNetwork;
        public Text defaultInterstitialAdNetwork;
        public Text defaultRewardedAdNetwork;
        public GameObject isInterstitialAdReadyInfo;
        public GameObject isRewardedAdReadyInfo;
        public DemoUtils demoUtils;

        void OnEnable()
        {
            Advertising.RewardedAdSkipped += OnRewardedAdSkipped;
            Advertising.RewardedAdCompleted += OnRewardedAdCompleted;
            Advertising.InterstitialAdCompleted += OnInterstitialAdCompleted;    
        }

        void OnDisable()
        {
            Advertising.RewardedAdSkipped -= OnRewardedAdSkipped;
            Advertising.RewardedAdCompleted -= OnRewardedAdCompleted;
            Advertising.InterstitialAdCompleted -= OnInterstitialAdCompleted;  
        }

        void OnInterstitialAdCompleted(InterstitialAdNetwork arg1, AdLocation arg2)
        {
            NativeUI.Alert("Interstitial Ad Completed", "Interstitial ad has been closed.");
        }

        void OnRewardedAdCompleted(RewardedAdNetwork arg1, AdLocation arg2)
        {
            NativeUI.Alert("Rewarded Ad Completed", "The rewarded ad has completed, this is when you should reward the user.");
        }

        void OnRewardedAdSkipped(RewardedAdNetwork arg1, AdLocation arg2)
        {
            NativeUI.Alert("Rewarded Ad Skipped", "The rewarded ad was skipped, and the user shouldn't get the reward.");
        }

        void Start()
        {
            curtain.SetActive(!EM_Settings.IsAdModuleEnable);

            AdSettings.DefaultAdNetworks defaultNetworks = new AdSettings.DefaultAdNetworks(BannerAdNetwork.None, InterstitialAdNetwork.None, RewardedAdNetwork.None);

            #if UNITY_ANDROID
            defaultNetworks = EM_Settings.Advertising.AndroidDefaultAdNetworks;
            #elif UNITY_IOS
            defaultNetworks = EM_Settings.Advertising.IosDefaultAdNetworks;
            #endif

            defaultBannerAdNetwork.text = "Default banner ad network: " + defaultNetworks.bannerAdNetwork.ToString();
            defaultInterstitialAdNetwork.text = "Default interstitial ad network: " + defaultNetworks.interstitialAdNetwork.ToString();
            defaultRewardedAdNetwork.text = "Default rewarded ad network: " + defaultNetworks.rewardedAdNetwork.ToString();

        }

        void Update()
        {
            // Check if autoLoad is enabled.
            if (Advertising.IsAutoLoadDefaultAds())
            {
                demoUtils.DisplayBool(isAutoLoadInfo, true, "Auto load default ads: ON");
            }
            else
            {
                demoUtils.DisplayBool(isAutoLoadInfo, false, "Auto load default ads: OFF");
            }

            // Check if ads were removed.
            if (Advertising.IsAdRemoved())
            {
                demoUtils.DisplayBool(isAdRemovedInfo, false, "Ads were removed");
            }
            else
            {
                demoUtils.DisplayBool(isAdRemovedInfo, true, "Ads are enabled");
            }

            // Check if interstitial ad is ready.
            if (Advertising.IsInterstitialAdReady())
            {
                demoUtils.DisplayBool(isInterstitialAdReadyInfo, true, "isInterstitialAdReady: TRUE");
            }
            else
            {
                demoUtils.DisplayBool(isInterstitialAdReadyInfo, false, "isInterstitialAdReady: FALSE");
            }

            // Check if rewarded ad is ready.
            if (Advertising.IsRewardedAdReady())
            {
                demoUtils.DisplayBool(isRewardedAdReadyInfo, true, "isRewardedAdReady: TRUE");
            }
            else
            {
                demoUtils.DisplayBool(isRewardedAdReadyInfo, false, "isRewardedAdReady: FALSE");
            }
        }

        /// <summary>
        /// Shows the default banner ad at the bottom of the screen.
        /// </summary>
        public void ShowBannerAd()
        {
            if (Advertising.IsAdRemoved())
            {
                NativeUI.Alert("Alert", "Ads were removed.");
                return;
            }
            Advertising.ShowBannerAd(BannerAdPosition.Bottom);
        }

        /// <summary>
        /// Hides the default banner ad.
        /// </summary>
        public void HideBannerAd()
        {
            Advertising.HideBannerAd();
        }

        /// <summary>
        /// Loads the interstitial ad.
        /// </summary>
        public void LoadInterstitialAd()
        {
            if (Advertising.IsAutoLoadDefaultAds())
            {
                NativeUI.Alert("Alert", "autoLoadDefaultAds is currently enabled. Ads will be loaded automatically in background without you having to do anything.");
            }

            Advertising.LoadInterstitialAd();
        }

        /// <summary>
        /// Shows the interstitial ad.
        /// </summary>
        public void ShowInterstitialAd()
        {
            if (Advertising.IsAdRemoved())
            {
                NativeUI.Alert("Alert", "Ads were removed.");
                return;
            }

            if (Advertising.IsInterstitialAdReady())
            {
                Advertising.ShowInterstitialAd();
            }
            else
            {
                NativeUI.Alert("Alert", "Interstitial ad is not loaded.");
            }
        }

        /// <summary>
        /// Loads the rewarded ad.
        /// </summary>
        public void LoadRewardedAd()
        {
            if (Advertising.IsAutoLoadDefaultAds())
            {
                NativeUI.Alert("Alert", "autoLoadDefaultAds is currently enabled. Ads will be loaded automatically in background without you having to do anything.");
            }

            Advertising.LoadRewardedAd();
        }

        /// <summary>
        /// Shows the rewarded ad.
        /// </summary>
        public void ShowRewardedAd()
        {
            if (Advertising.IsRewardedAdReady())
            {
                Advertising.ShowRewardedAd();
            }
            else
            {
                NativeUI.Alert("Alert", "Rewarded ad is not loaded.");
            }
        }

        /// <summary>
        /// Removes the ads.
        /// </summary>
        public void RemoveAds()
        {
            Advertising.RemoveAds();
            NativeUI.Alert("Alert", "Ads were removed. Future ads won't be shown except rewarded ads.");
        }

        /// <summary>
        /// Resets the remove ads.
        /// </summary>
        public void ResetRemoveAds()
        {
            Advertising.ResetRemoveAds();
            NativeUI.Alert("Alert", "Remove Ads status was reset. Ads will be shown normally.");
        }
    }
}

