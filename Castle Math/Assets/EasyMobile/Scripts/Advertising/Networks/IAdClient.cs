using UnityEngine;
using System;
using System.Collections;

namespace EasyMobile.AdvertisingInternal
{
    internal interface IAdClient
    {
        //------------------------------------------------------------
        // General Info.
        //------------------------------------------------------------
        AdNetwork Network { get; }

        bool SupportBannerAds { get; }

        bool SupportInterstitialAds { get; }

        bool SupportRewardedAds { get; }

        //------------------------------------------------------------
        // Init.
        //------------------------------------------------------------

        void Init(AdSettings settings);

        bool IsReady();

        //------------------------------------------------------------
        // Banner Ads.
        //------------------------------------------------------------

        void ShowBannerAd(BannerAdPosition position, BannerAdSize size);

        void HideBannerAd();

        void DestroyBannerAd();

        //------------------------------------------------------------
        // Interstitial Ads.
        //------------------------------------------------------------

        event Action<InterstitialAdNetwork, AdLocation> InterstitialAdCompleted;

        void LoadInterstitialAd(AdLocation location);

        bool IsInterstitialAdReady(AdLocation location);

        void ShowInterstitialAd(AdLocation location);

        //------------------------------------------------------------
        // Rewarded Ads.
        //------------------------------------------------------------

        event Action<RewardedAdNetwork, AdLocation> RewardedAdSkipped;

        event Action<RewardedAdNetwork, AdLocation> RewardedAdCompleted;

        void LoadRewardedAd(AdLocation location);

        bool IsRewardedAdReady(AdLocation location);

        void ShowRewardedAd(AdLocation location);

    }
}