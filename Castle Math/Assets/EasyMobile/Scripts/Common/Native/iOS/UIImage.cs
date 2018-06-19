#if UNITY_IOS
using System;
using System.Runtime.InteropServices;

namespace EasyMobile.Internal.iOS
{
    internal class UIImage : iOSInteropObject
    {
        [DllImport("__Internal")]
        private static extern float _InteropUIImage_Scale(HandleRef self);

        [DllImport("__Internal")]
        private static extern float _InteropUIImage_Width(HandleRef self);

        [DllImport("__Internal")]
        private static extern float _InteropUIImage_Height(HandleRef self);

        [DllImport("__Internal")]
        private static extern int _InteropUIImage_GetPNGData(
            HandleRef self, [In, Out] /* from(unsigned char *) */ byte[] buffer, int byteCount);

        [DllImport("__Internal")]
        private static extern int _InteropUIImage_GetJPEGData(
            HandleRef self, float compressionQuality, [In, Out] /* from(unsigned char *) */ byte[] buffer, int byteCount);

        private float? mScale = null;
        private float? mWidth = null;
        private float? mHeight = null;

        internal UIImage(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        internal static UIImage FromPointer(IntPtr pointer)
        {
            if (pointer.Equals(IntPtr.Zero))
            {
                return null;
            }
            return new UIImage(pointer);
        }

        internal float Scale
        {
            get
            {
                if (mScale == null)
                    mScale = _InteropUIImage_Scale(SelfPtr());
                return mScale.Value;
            }
        }

        internal float Width
        {
            get
            {
                if (mWidth == null)
                    mWidth = _InteropUIImage_Width(SelfPtr());
                return mWidth.Value;
            }
        }

        internal float Height
        {
            get
            {
                if (mHeight == null)
                    mHeight = _InteropUIImage_Height(SelfPtr());
                return mHeight.Value;
            }
        }

        internal byte[] GetPNGData()
        {
            return PInvokeUtil.GetNativeArray<byte>((buffer, length) =>
                _InteropUIImage_GetPNGData(SelfPtr(), buffer, length));
        }

        internal byte[] GetJPEGData(float compressionQuality)
        {
            compressionQuality = UnityEngine.Mathf.Clamp(compressionQuality, 0, 1);
            return PInvokeUtil.GetNativeArray<byte>((buffer, length) =>
                _InteropUIImage_GetJPEGData(SelfPtr(), compressionQuality, buffer, length));
        }
    }
}
#endif
