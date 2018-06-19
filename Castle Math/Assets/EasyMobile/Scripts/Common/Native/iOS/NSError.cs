#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;

namespace EasyMobile.Internal.iOS
{
    internal class NSError : iOSInteropObject
    {
        [DllImport("__Internal")]
        private static extern int _InteropNSError_Code(HandleRef self);

        [DllImport("__Internal")]
        private static extern int _InteropNSError_Domain(
            HandleRef self, [In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

        [DllImport("__Internal")]
        private static extern int _InteropNSError_LocalizedDescription(
            HandleRef self, [In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

        [DllImport("__Internal")]
        private static extern int _InteropNSError_LocalizedRecoverySuggestion(
            HandleRef self, [In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

        [DllImport("__Internal")]
        private static extern int _InteropNSError_LocalizedFailureReason(
            HandleRef self, [In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

        internal NSError(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        internal static NSError FromPointer(IntPtr pointer)
        {
            if (pointer.Equals(IntPtr.Zero))
            {
                return null;
            }
            return new NSError(pointer);
        }

        internal int Code
        {
            get { return _InteropNSError_Code(SelfPtr()); }
        }

        internal string Domain
        {
            get
            {
                return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                    _InteropNSError_Domain(SelfPtr(), strBuffer, strLen));
            }
        }

        internal string LocalizedDescription
        {
            get
            {
                return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                    _InteropNSError_LocalizedDescription(SelfPtr(), strBuffer, strLen));
            }
        }

        internal string LocalizedRecoverySuggestion
        {
            get
            {
                return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                    _InteropNSError_LocalizedRecoverySuggestion(SelfPtr(), strBuffer, strLen));
            }
        }

        internal string LocalizedFailureReason
        {
            get
            {
                return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                    _InteropNSError_LocalizedFailureReason(SelfPtr(), strBuffer, strLen));
            }
        }
    }
}
#endif