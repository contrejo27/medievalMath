using UnityEngine;
using System;
using System.Collections;

namespace EasyMobile.AdvertisingInternal
{
    internal class NoOpClientImpl : IAdClient
    {
        #pragma warning disable 0067
        public event Action<InterstitialAdNetwork, AdLocation> InterstitialAdCompleted;

        public event Action<RewardedAdNetwork, AdLocation> RewardedAdSkipped;

        public event Action<RewardedAdNetwork, AdLocation> RewardedAdCompleted;
        #pragma warning restore 0067

        // Singleton.
        private static NoOpClientImpl sInstance;

        private NoOpClientImpl()
        {
        }

        /// <summary>
        /// Creates and initializes the singleton client.
        /// </summary>
        /// <returns>The client.</returns>
        public static NoOpClientImpl CreateClient()
        {
            if (sInstance == null)
            {
                sInstance = new NoOpClientImpl();
            }
            return sInstance;
        }

        //------------------------------------------------------------
        // General Info.
        //------------------------------------------------------------

        public AdNetwork Network { get { return AdNetwork.None; } }

        public bool SupportBannerAds { get { return false; } }

        public bool SupportInterstitialAds { get { return false; } }

        public bool SupportRewardedAds { get { return false; } }

        //------------------------------------------------------------
        // Init.
        //------------------------------------------------------------

        public void Init(AdSettings settings)
        {
        }

        public bool IsReady()
        {
            return false;
        }

        //------------------------------------------------------------
        // Banner Ads.
        //------------------------------------------------------------

        public void ShowBannerAd(BannerAdPosition position, BannerAdSize size)
        {
        }

        public void HideBannerAd()
        {
        }

        public void DestroyBannerAd()
        {
        }

        //------------------------------------------------------------
        // Interstitial Ads.
        //------------------------------------------------------------

        public void LoadInterstitialAd(AdLocation location)
        {
        }

        public bool IsInterstitialAdReady(AdLocation location)
        {
            return false;
        }

        public void ShowInterstitialAd(AdLocation location)
        {
        }

        //------------------------------------------------------------
        // Rewarded Ads.
        //------------------------------------------------------------

        public void LoadRewardedAd(AdLocation location)
        {
        }

        public bool IsRewardedAdReady(AdLocation location)
        {
            return false;
        }

        public void ShowRewardedAd(AdLocation location)
        {
        }
    }
}