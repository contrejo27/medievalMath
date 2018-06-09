using System;

#if EM_ADMOB
using GoogleMobileAds.Api;
#endif

#if EM_HEYZAP
using Heyzap;
#endif

namespace EasyMobile
{
    // For easy display in the inspector
    public enum BannerAdPosition
    {
        Top,
        Bottom,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    // Extension methods to convert to network-specific banner positions.
    static class BannerAdPositionMethods
    {
        #if EM_ADMOB
        public static AdPosition ToAdMobAdPosition(this BannerAdPosition pos)
        {
            switch (pos)
            {
                case BannerAdPosition.Top:
                    return AdPosition.Top;
                case BannerAdPosition.Bottom:
                    return AdPosition.Bottom;
                case BannerAdPosition.TopLeft:
                    return AdPosition.TopLeft;
                case BannerAdPosition.TopRight:
                    return AdPosition.TopRight;
                case BannerAdPosition.BottomLeft:
                    return AdPosition.BottomLeft;
                case BannerAdPosition.BottomRight:
                    return AdPosition.BottomRight;
                default:
                    return AdPosition.Top;
            }
        }
        #endif

        #if EM_HEYZAP
        public static string ToHeyzapAdPosition(this BannerAdPosition pos)
        {
            switch (pos)
            {
                case BannerAdPosition.TopLeft:
                case BannerAdPosition.TopRight:
                case BannerAdPosition.Top:
                    return HZBannerShowOptions.POSITION_TOP;
                case BannerAdPosition.BottomLeft:
                case BannerAdPosition.BottomRight:
                case BannerAdPosition.Bottom:
                    return HZBannerShowOptions.POSITION_BOTTOM;
                default:
                    return HZBannerShowOptions.POSITION_TOP;
            }
        }
        #endif
    }
}

