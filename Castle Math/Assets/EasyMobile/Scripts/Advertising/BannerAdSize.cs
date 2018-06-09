using System;

#if EM_ADMOB
using GoogleMobileAds.Api;
#endif

namespace EasyMobile
{
    public class BannerAdSize
    {
        public bool IsSmartBanner { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public static readonly BannerAdSize Banner = new BannerAdSize(320, 50);
        public static readonly BannerAdSize MediumRectangle = new BannerAdSize(300, 250);
        public static readonly BannerAdSize IABBanner = new BannerAdSize(468, 60);
        public static readonly BannerAdSize Leaderboard = new BannerAdSize(728, 90);
        public static readonly BannerAdSize SmartBanner = new BannerAdSize(true);

        public BannerAdSize(int width, int height)
        {
            IsSmartBanner = false;
            this.Width = width;
            this.Height = height;
        }

        private BannerAdSize(bool isSmartBanner)
        {
            this.IsSmartBanner = isSmartBanner;
            this.Width = 0;
            this.Height = 0;
        }

        #if EM_ADMOB
        public AdSize ToAdMobAdSize()
        {
            return IsSmartBanner ? AdSize.SmartBanner : new AdSize(Width, Height);
        }
        #endif
    }
}

