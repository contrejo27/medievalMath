#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;

namespace EasyMobile.Internal.iOS
{
    internal class UIApplication
    {
        #region Native Methods

        [DllImport("__Internal")]
        private static extern int _InteropUIApplication_GetApplicationIconBadgeNumber();

        [DllImport("__Internal")]
        private static extern void _InteropUIApplication_SetApplicationIconBadgeNumber(int value);

        #endregion // Native Methods

        internal static UIApplication SharedApplication
        {
            get
            {
                if (sSharedApplication == null)
                    sSharedApplication = new UIApplication();

                return sSharedApplication;
            }
        }

        private static UIApplication sSharedApplication;

        private UIApplication()
        {
        }

        internal int GetApplicationIconBadgeNumber()
        {
            return _InteropUIApplication_GetApplicationIconBadgeNumber();
        }

        internal void SetApplicationIconBadgeNumber(int value)
        {
            _InteropUIApplication_SetApplicationIconBadgeNumber(value);
        }
    }
}
#endif