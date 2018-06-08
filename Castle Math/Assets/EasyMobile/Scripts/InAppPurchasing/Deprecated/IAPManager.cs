using UnityEngine;
using System.Collections;
using System;

#if EM_UIAP
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
#endif

namespace EasyMobile
{
    [AddComponentMenu("")][System.Obsolete("This class was deprecated in Easy Mobile Pro 1.2.0. Please use EasyMobile.InAppPurchasing instead.")]
    public class IAPManager : MonoBehaviour
    {
        public static IAPManager Instance { get; private set; }

        // Suppress the "Event is never used" warnings.
        #pragma warning disable 0067
        public static event Action InitializeSucceeded;
        public static event Action InitializeFailed;
        public static event Action<IAPProduct> PurchaseCompleted;
        public static event Action<IAPProduct> PurchaseFailed;
        // Restore events are fired on iOS or MacOSX only
        public static event Action RestoreCompleted;
        public static event Action RestoreFailed;
        #pragma warning restore 0067

        #if EM_UIAP
        /// <summary>
        /// The underlying UnityIAP's ConfigurationBuilder used in this module.
        /// </summary>
        /// <value>The builder.</value>
        public static ConfigurationBuilder Builder { get { return _builder; } }

        /// <summary>
        /// The underlying UnityIAP's IStoreController used in this module.
        /// </summary>
        /// <value>The store controller.</value>
        public static IStoreController StoreController { get { return _storeController; } }

        /// <summary>
        /// The underlying UnityIAP's IExtensionProvider used in this module. Use it to access
        /// store-specific extended functionality.
        /// </summary>
        /// <value>The store extension provider.</value>
        public static IExtensionProvider StoreExtensionProvider { get { return _storeExtensionProvider; } }

        // The ConfigurationBuilder
        private static ConfigurationBuilder _builder;

        // The Unity Purchasing system
        private static IStoreController _storeController;

        // The store-specific Purchasing subsystems
        private static IExtensionProvider _storeExtensionProvider;

        // Store listener to handle purchasing events
        private static StoreListener _storeListener = new StoreListener();
        #endif

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        void Start()
        {
            #if EM_UIAP
            // If we haven't set up the Unity Purchasing reference
            if (_storeController == null)
            {
                // Begin to configure our connection to Purchasing
                InitializePurchasing();
            }
            #endif
        }

        /// <summary>
        /// Initializes the in-app purchasing service.
        /// </summary>
        public static void InitializePurchasing()
        {
            #if EM_UIAP
            if (IsInitialized())
            {
                return;
            }

            // Create a builder, first passing in a suite of Unity provided stores.
            _builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            // Add products
            foreach (IAPProduct pd in EM_Settings.InAppPurchasing.Products)
            {                
                if (pd.StoreSpecificIds != null && pd.StoreSpecificIds.Length > 0)
                {
                    // Add store-specific id if any
                    IDs storeIDs = new IDs();

                    foreach (IAPProduct.StoreSpecificId sId in pd.StoreSpecificIds)
                    {
                        storeIDs.Add(sId.id, new string[] { GetStoreName(sId.store) });
                    }

                    // Add product with store-specific ids
                    _builder.AddProduct(pd.Id, GetProductType(pd.Type), storeIDs);
                }
                else
                {
                    // Add product using store-independent id
                    _builder.AddProduct(pd.Id, GetProductType(pd.Type));
                }
            }

            // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
            // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
            UnityPurchasing.Initialize(_storeListener, _builder);
            #else
            Debug.Log("InitializePurchasing FAILED: IAP module is not enabled.");
            #endif
        }

        /// <summary>
        /// Determines whether UnityIAP is initialized. All further actions like purchasing
        /// or restoring can only be done if UnityIAP is initialized.
        /// </summary>
        /// <returns><c>true</c> if initialized; otherwise, <c>false</c>.</returns>
        public static bool IsInitialized()
        {
            #if EM_UIAP
            // Only say we are initialized if both the Purchasing references are set.
            return _storeController != null && _storeExtensionProvider != null;
            #else
            return false;
            #endif
        }

        /// <summary>
        /// Purchases the specified product.
        /// </summary>
        /// <param name="product">Product.</param>
        public static void Purchase(IAPProduct product)
        {
            if (product != null && product.Id != null)
            {
                PurchaseWithId(product.Id);
            }
            else
            {
                Debug.Log("Purchase FAILED: Either the product or its id is invalid.");
            }
        }

        /// <summary>
        /// Purchases the product with specified name.
        /// </summary>
        /// <param name="productName">Product name.</param>
        public static void Purchase(string productName)
        {
            IAPProduct pd = GetIAPProductByName(productName);

            if (pd != null && pd.Id != null)
            {
                PurchaseWithId(pd.Id);
            }
            else
            {
                Debug.Log("PurchaseWithName FAILED: Not found product with name: " + productName + " or its id is invalid.");
            }
        }

        /// <summary>
        /// Purchases the product with specified ID.
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        public static void PurchaseWithId(string productId)
        {
            #if EM_UIAP
            if (IsInitialized())
            {
                Product product = _storeController.products.WithID(productId);

                if (product != null && product.availableToPurchase)
                {
                    Debug.Log("Purchasing product asychronously: " + product.definition.id);

                    // Buy the product, expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
                    _storeController.InitiatePurchase(product);
                }
                else
                {
                    Debug.Log("BuyProductID FAILED: either product not found or not available for purchase.");
                }
            }
            else
            {
                // Purchasing has not succeeded initializing yet.
                Debug.Log("BuyProductID FAILED: In-App Purchasing is not initialized.");
            }
            #else
            Debug.Log("PurchaseWithId FAILED: IAP module is not enabled.");
            #endif
        }

        /// <summary>
        /// Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google Play.
        /// Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        /// This method only has effect on iOS and MacOSX apps.
        /// </summary>
        public static void RestorePurchases()
        {
            #if EM_UIAP
            if (!IsInitialized())
            {
                Debug.Log("RestorePurchases FAILED: In-App Purchasing is not initialized.");
                return;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = _storeExtensionProvider.GetExtension<IAppleExtensions>();

                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
                // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result) =>
                    {
                        // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                        // no purchases are available to be restored.
                        Debug.Log("RestorePurchases result: " + result);

                        if (result)
                        {
                            // Fire restore complete event.
                            if (RestoreCompleted != null)
                                RestoreCompleted();
                        }
                        else
                        {
                            // Fire event failed event.
                            if (RestoreFailed != null)
                                RestoreFailed();
                        }
                    });
            }
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("RestorePurchases FAILED: Not supported on this platform: " + Application.platform.ToString());
            }
            #else
            Debug.Log("RestorePurchases FAILED: IAP module is not enabled.");
            #endif
        }

        /// <summary>
        /// Determines whether the product with the specified name is owned.
        /// A product is consider owned if it has a receipt. If receipt validation
        /// is enabled, it is also required that this receipt passes the validation check.
        /// Note that this method is mostly useful with non-consumable products.
        /// Consumable products' receipts are not persisted between app restarts,
        /// therefore their ownership only pertains in the session they're purchased.
        /// In the case of subscription products, this method only checks if a product has been purchased before,
        /// it doesn't check if the subscription has been expired or canceled. 
        /// </summary>
        /// <returns><c>true</c> if the product has a receipt and that receipt is valid (if receipt validation is enabled); otherwise, <c>false</c>.</returns>
        /// <param name="productId">Product name.</param>
        public static bool IsProductOwned(string productName)
        {
            #if EM_UIAP
            if (!IsInitialized())
            {
                Debug.Log("IsProductOwned FAILED: In-App Purchasing is not initialized..");
                return false;
            }

            IAPProduct iapProduct = GetIAPProductByName(productName);

            if (iapProduct == null)
            {
                Debug.Log("IsProductOwned FAILED: Not found product with name: " + productName);
                return false;
            }

            Product pd = _storeController.products.WithID(iapProduct.Id);

            if (pd.hasReceipt)
            {
                bool isValid = true; // presume validity if not validate receipt.

                if (IsReceiptValidationEnabled())
                {
                    IPurchaseReceipt[] purchaseReceipts;
                    isValid = ValidateReceipt(pd.receipt, out purchaseReceipts);
                }
                    
                return isValid;
            }
            else
            {
                return false;
            }

            #else
            Debug.Log("IsProductOwned FAILED: IAP module is not enabled.");
            return false;
            #endif
        }

        /// <summary>
        /// Fetches a new Apple App receipt from their servers.
        /// Note that this will prompt the user for their password.
        /// </summary>
        /// <param name="successCallback">Success callback.</param>
        /// <param name="errorCallback">Error callback.</param>
        public static void RefreshAppleAppReceipt(Action<string> successCallback, Action errorCallback)
        {
            #if EM_UIAP
            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                Debug.Log("RefreshAppleAppReceipt is only available on iOS.");
                return;
            }

            if (!IsInitialized())
            {
                Debug.Log("RefreshAppleAppReceipt FAILED: In-App Purchasing is not initialized.");
                return;
            }

            _storeExtensionProvider.GetExtension<IAppleExtensions>().RefreshAppReceipt(
                (receipt) =>
                {
                    // This handler is invoked if the request is successful.
                    // Receipt will be the latest app receipt.
                    if (successCallback != null)
                        successCallback(receipt);
                },
                () =>
                {
                    // This handler will be invoked if the request fails,
                    // such as if the network is unavailable or the user
                    // enters the wrong password.
                    if (errorCallback != null)
                        errorCallback();
                });

            #else
            Debug.Log("RefreshAppleAppReceipt FAILED: IAP module is not enabled.");
            return;
            #endif
        }

        /// <summary>
        /// Gets all IAP products declared in module settings.
        /// </summary>
        /// <returns>The all IAP products.</returns>
        public static IAPProduct[] GetAllIAPProducts()
        {
            return EM_Settings.InAppPurchasing.Products;
        }

        /// <summary>
        /// Gets the IAP product declared in module settings with the specified name.
        /// </summary>
        /// <returns>The IAP product.</returns>
        /// <param name="productName">Product name.</param>
        public static IAPProduct GetIAPProductByName(string productName)
        {
            foreach (IAPProduct pd in EM_Settings.InAppPurchasing.Products)
            {
                if (pd.Name.Equals(productName))
                    return pd;
            }

            return null;
        }

        /// <summary>
        /// Gets the IAP product declared in module settings with the specified identifier.
        /// </summary>
        /// <returns>The IAP product.</returns>
        /// <param name="pId">P identifier.</param>
        public static IAPProduct GetIAPProductById(string productId)
        {
            foreach (IAPProduct pd in EM_Settings.InAppPurchasing.Products)
            {
                if (pd.Id.Equals(productId))
                    return pd;
            }

            return null;
        }

        #if EM_UIAP

        #region Module-enable only methods

        /// <summary>
        /// Gets the product registered with UnityIAP stores by its name. This method returns
        /// a Product object, which contains more information than an IAPProduct
        /// object, whose main purpose is for displaying.
        /// </summary>
        /// <returns>The product.</returns>
        /// <param name="productName">Product name.</param>
        public static Product GetProduct(string productName)
        {
            if (!IsInitialized())
            {
                Debug.Log("GetProduct FAILED: In-App Purchasing is not initialized.");
                return null;
            }

            IAPProduct iapProduct = GetIAPProductByName(productName);

            if (iapProduct == null)
            {
                Debug.Log("GetProduct FAILED: Not found product with name: " + productName);
                return null;
            }

            return _storeController.products.WithID(iapProduct.Id);
        }

        /// <summary>
        /// Gets the product localized data provided by the stores.
        /// </summary>
        /// <returns>The product localized data.</returns>
        /// <param name="productId">Product name.</param>
        public static ProductMetadata GetProductLocalizedData(string productName)
        {            
            if (!IsInitialized())
            {
                Debug.Log("GetProductLocalizedData FAILED: In-App Purchasing is not initialized.");
                return null;
            }

            IAPProduct iapProduct = GetIAPProductByName(productName);

            if (iapProduct == null)
            {
                Debug.Log("GetProductLocalizedData FAILED: Not found product with name: " + productName);
                return null;
            }

            return _storeController.products.WithID(iapProduct.Id).metadata;    
        }

        /// <summary>
        /// Gets the parsed Apple InAppPurchase receipt for the specified product.
        /// This method only works if receipt validation is enabled.
        /// </summary>
        /// <returns>The Apple In App Purchase receipt.</returns>
        /// <param name="productName">Product name.</param>
        public static AppleInAppPurchaseReceipt GetAppleIAPReceipt(string productName)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return GetPurchaseReceipt(productName) as AppleInAppPurchaseReceipt;
            }
            else
            {
                Debug.Log("GetAppleProductReceipt is only available on iOS.");
                return null;
            }
        }

        /// <summary>
        /// Gets the parsed Google Play receipt for the specified product.
        /// This method only works if receipt validation is enabled.
        /// </summary>
        /// <returns>The Google Play receipt.</returns>
        /// <param name="productName">Product name.</param>
        public static GooglePlayReceipt GetGooglePlayReceipt(string productName)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return GetPurchaseReceipt(productName) as GooglePlayReceipt;
            }
            else
            {
                Debug.Log("GetGooglePlayReceipt is only available on Android.");
                return null;
            }
        }

        /// <summary>
        /// Gets the parsed purchase receipt for the product.
        /// This method only works if receipt validation is enabled.
        /// </summary>
        /// <returns>The purchase receipt.</returns>
        /// <param name="productName">Product name.</param>
        public static IPurchaseReceipt GetPurchaseReceipt(string productName)
        {
            if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
            {
                Debug.Log("GetPurchaseReceipt is only available on Android and iOS.");
                return null;
            }

            if (!IsInitialized())
            {
                Debug.Log("GetPurchaseReceipt FAILED: In-App Purchasing is not initialized.");
                return null;
            }

            IAPProduct iapProduct = GetIAPProductByName(productName);

            if (iapProduct == null)
            {
                Debug.Log("GetPurchaseReceipt FAILED: Not found product with name: " + productName);
                return null;
            }

            Product pd = _storeController.products.WithID(iapProduct.Id);

            if (!pd.hasReceipt)
            {
                Debug.Log("GetPurchaseReceipt FAILED: this product doesn't have a receipt.");
                return null;
            }

            if (!IsReceiptValidationEnabled())
            {
                Debug.Log("GetPurchaseReceipt FAILED: please enable receipt validation.");
                return null;
            }

            IPurchaseReceipt[] purchaseReceipts;
            if (!ValidateReceipt(pd.receipt, out purchaseReceipts))
            {
                Debug.Log("GetPurchaseReceipt FAILED: the receipt of this product is invalid.");
                return null;
            }

            foreach (var r in purchaseReceipts)
            {
                if (r.productID.Equals(pd.definition.id))
                    return r;
            }

            // If we reach here, there's no receipt with the matching productID
            return null;
        }

        /// <summary>
        /// Gets the Apple App receipt. This method only works if receipt validation is enabled.
        /// </summary>
        /// <returns>The Apple App receipt.</returns>
        public static AppleReceipt GetAppleAppReceipt()
        { 
            if (!IsInitialized())
            {
                Debug.Log("GetAppleAppReceipt FAILED: In-App Purchasing is not initialized.");
                return null;
            }

            if (!EM_Settings.InAppPurchasing.IsValidateAppleReceipt)
            {
                Debug.Log("GetAppleAppReceipt FAILED: Please enable Apple receipt validation.");
                return null;
            }

            // Note that the code is disabled in the editor for it to not stop the EM editor code (due to ClassNotFound error)
            // from recreating the dummy AppleTangle class if they were inadvertently removed.
            #if UNITY_IOS && !UNITY_EDITOR
            // Get a reference to IAppleConfiguration during IAP initialization.
            var appleConfig = _builder.Configure<IAppleConfiguration>();
            var receiptData = System.Convert.FromBase64String(appleConfig.appReceipt);
            AppleReceipt receipt = new AppleValidator(AppleTangle.Data()).Validate(receiptData);
            return receipt;
            #else
            Debug.Log("GetAppleAppReceipt is only available on iOS.");
            return null;
            #endif
        }

        /// <summary>
        /// Gets the name of the store.
        /// </summary>
        /// <returns>The store name.</returns>
        /// <param name="store">Store.</param>
        public static string GetStoreName(IAPStore store)
        {
            switch (store)
            {
                case IAPStore.AmazonApps:
                    return AmazonApps.Name;
                case IAPStore.AppleAppStore:
                    return AppleAppStore.Name;
                case IAPStore.GooglePlay:
                    return GooglePlay.Name;
                case IAPStore.MacAppStore:
                    return MacAppStore.Name;
                case IAPStore.SamsungApps:
                    return SamsungApps.Name;
                case IAPStore.WindowsStore:
                    return WindowsStore.Name;
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the type of the product.
        /// </summary>
        /// <returns>The product type.</returns>
        /// <param name="pType">P type.</param>
        public static ProductType GetProductType(IAPProductType pType)
        {
            switch (pType)
            {
                case IAPProductType.Consumable:
                    return ProductType.Consumable;
                case IAPProductType.NonConsumable:
                    return ProductType.NonConsumable;
                case IAPProductType.Subscription:
                    return ProductType.Subscription;
                default:
                    return ProductType.Consumable;
            }
        }

        /// <summary>
        /// Converts to UnityIAP AndroidStore.
        /// </summary>
        /// <returns>The android store.</returns>
        /// <param name="store">Store.</param>
        public static AndroidStore GetAndroidStore(IAPAndroidStore store)
        {
            switch (store)
            {
                case IAPAndroidStore.AmazonAppStore:
                    return AndroidStore.AmazonAppStore;
                case IAPAndroidStore.GooglePlay:
                    return AndroidStore.GooglePlay;
                case IAPAndroidStore.SamsungApps:
                    return AndroidStore.SamsungApps;
                case IAPAndroidStore.NotSpecified:
                    return AndroidStore.NotSpecified;
                default:
                    return AndroidStore.NotSpecified;
            }
        }

        /// <summary>
        /// Converts to UnityIAP AppStore.
        /// </summary>
        /// <returns>The app store.</returns>
        /// <param name="store">Store.</param>
        public static AppStore GetAppStore(IAPAndroidStore store)
        {
            switch (store)
            {
                case IAPAndroidStore.AmazonAppStore:
                    return AppStore.AmazonAppStore;
                case IAPAndroidStore.GooglePlay:
                    return AppStore.GooglePlay;
                case IAPAndroidStore.SamsungApps:
                    return AppStore.SamsungApps;
                case IAPAndroidStore.NotSpecified:
                    return AppStore.NotSpecified;
                default:
                    return AppStore.NotSpecified;
            }
        }

        #endregion

        #region IStoreListener implementation

        private class StoreListener : IStoreListener
        {
            public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
            {
                // Purchasing has succeeded initializing. Collect our Purchasing references.
                Debug.Log("In-App Purchasing OnInitialized: PASS");

                // Overall Purchasing system, configured with products for this application.
                _storeController = controller;

                // Store specific subsystem, for accessing device-specific store features.
                _storeExtensionProvider = extensions;

                // Fire event
                if (InitializeSucceeded != null)
                    InitializeSucceeded();
            }

            public void OnInitializeFailed(InitializationFailureReason error)
            {
                // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
                Debug.Log("In-App Purchasing OnInitializeFailed. InitializationFailureReason:" + error);

                // Fire event
                if (InitializeFailed != null)
                    InitializeFailed();
            }

            public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
            {
                // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
                // this reason with the user to guide their troubleshooting actions.
                Debug.Log(string.Format("Purchase product FAILED: Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));

                // Fire purchase failure event
                if (PurchaseFailed != null)
                    PurchaseFailed(GetIAPProductById(product.definition.id));
            }

            public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
            {
                Debug.Log("Processing purchase of product: " + args.purchasedProduct.transactionID);
                
                bool validPurchase = true;  // presume validity if not validate receipt

                if (IsReceiptValidationEnabled())
                {
                    IPurchaseReceipt[] purchaseReceipts;
                    validPurchase = ValidateReceipt(args.purchasedProduct.receipt, out purchaseReceipts, true);
                }
        
                IAPProduct pd = GetIAPProductById(args.purchasedProduct.definition.id);

                if (validPurchase)
                {
                    Debug.Log("Product purchase completed.");
                    
                    // Fire purchase success event
                    if (PurchaseCompleted != null)
                        PurchaseCompleted(pd);
                }
                else
                {
                    Debug.Log("Purchase FAILED: Invalid receipt.");

                    // Fire purchase failure event
                    if (PurchaseFailed != null)
                        PurchaseFailed(pd);
                }

                return PurchaseProcessingResult.Complete;
            }
        }

        /// <summary>
        /// Determines if receipt validation is enabled.
        /// </summary>
        /// <returns><c>true</c> if is receipt validation enabled; otherwise, <c>false</c>.</returns>
        private static bool IsReceiptValidationEnabled()
        {
            bool canValidateReceipt = false;    // disable receipt validation by default

            if (Application.platform == RuntimePlatform.Android)
            {
                // On Android, receipt validation is only available for Google Play store
                canValidateReceipt = EM_Settings.InAppPurchasing.IsValidateGooglePlayReceipt;
                canValidateReceipt &= (GetAndroidStore(EM_Settings.InAppPurchasing.TargetAndroidStore) == AndroidStore.GooglePlay);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer ||
                     Application.platform == RuntimePlatform.OSXPlayer ||
                     Application.platform == RuntimePlatform.tvOS)
            {
                // Receipt validation is also available for Apple app stores
                canValidateReceipt = EM_Settings.InAppPurchasing.IsValidateAppleReceipt;
            }

            return canValidateReceipt;
        }

        /// <summary>
        /// Validates the receipt. Works with receipts from Apple stores and Google Play store only.
        /// Always returns true for other stores.
        /// </summary>
        /// <returns><c>true</c>, if the receipt is valid, <c>false</c> otherwise.</returns>
        /// <param name="receipt">Receipt.</param>
        /// <param name="logReceiptContent">If set to <c>true</c> log receipt content.</param>
        private static bool ValidateReceipt(string receipt, out IPurchaseReceipt[] purchaseReceipts, bool logReceiptContent = false)
        {
            purchaseReceipts = new IPurchaseReceipt[0];   // default the out parameter to an empty array   

            // Does the receipt has some content?
            if (string.IsNullOrEmpty(receipt))
            {
                Debug.Log("Receipt Validation: receipt is null or empty.");
                return false;
            }

            bool isValidReceipt = true; // presume validity for platforms with no receipt validation.
            // Unity IAP's receipt validation is only available for Apple app stores and Google Play store.   
            #if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_TVOS
        
            byte[] googlePlayTangleData = null;
            byte[] appleTangleData = null;

            // Here we populate the secret keys for each platform.
            // Note that the code is disabled in the editor for it to not stop the EM editor code (due to ClassNotFound error)
            // from recreating the dummy AppleTangle and GoogleTangle classes if they were inadvertently removed.

            #if UNITY_ANDROID && !UNITY_EDITOR
            googlePlayTangleData = GooglePlayTangle.Data();
            #endif

            #if (UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_TVOS) && !UNITY_EDITOR
            appleTangleData = AppleTangle.Data();
            #endif
        
            // Prepare the validator with the secrets we prepared in the Editor obfuscation window.
            #if UNITY_5_6_OR_NEWER
            var validator = new CrossPlatformValidator(googlePlayTangleData, appleTangleData, Application.identifier);
            #else
            var validator = new CrossPlatformValidator(googlePlayTangleData, appleTangleData, Application.bundleIdentifier);
            #endif
        
            try
            {
                // On Google Play, result has a single product ID.
                // On Apple stores, receipts contain multiple products.
                var result = validator.Validate(receipt);

                // If the validation is successful, the result won't be null.
                if (result == null)
                {
                    isValidReceipt = false; 
                }
                else
                {
                    purchaseReceipts = result;

                    // For informational purposes, we list the receipt(s)
                    if (logReceiptContent)
                    {
                        Debug.Log("Receipt contents:");
                        foreach (IPurchaseReceipt productReceipt in result)
                        {
                            if (productReceipt != null)
                            {
                                Debug.Log(productReceipt.productID);
                                Debug.Log(productReceipt.purchaseDate);
                                Debug.Log(productReceipt.transactionID);
                            }
                        }
                    }
                }
            }
            catch (IAPSecurityException)
            {
                Debug.Log("Receipt Validation: Invalid receipt.");
                isValidReceipt = false;
            }
            #endif
        
            return isValidReceipt;
        }

        #endregion

        #endif  // #if EM_UIAP
    }
}