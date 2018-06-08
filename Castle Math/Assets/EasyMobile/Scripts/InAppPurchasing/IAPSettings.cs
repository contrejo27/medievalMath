using UnityEngine;
using System.Collections;

namespace EasyMobile
{
    [System.Serializable]
    public class IAPSettings
    {
        public IAPAndroidStore TargetAndroidStore { get { return _targetAndroidStore; } }

        public bool IsValidateAppleReceipt { get { return _validateAppleReceipt; } }

        public bool IsValidateGooglePlayReceipt { get { return _validateGooglePlayReceipt; } }

        public IAPProduct[] Products { get { return _products; } }

        [SerializeField]
        private IAPAndroidStore _targetAndroidStore = IAPAndroidStore.GooglePlay;
        [SerializeField]
        private bool _validateAppleReceipt = true;
        [SerializeField]
        private bool _validateGooglePlayReceipt = true;
        [SerializeField]
        private IAPProduct[] _products;
    }
}
