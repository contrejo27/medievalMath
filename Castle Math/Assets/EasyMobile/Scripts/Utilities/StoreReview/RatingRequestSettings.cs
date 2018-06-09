using UnityEngine;
using System.Collections;

namespace EasyMobile
{
    [System.Serializable]
    public class RatingRequestSettings
    {
        public RatingDialogContent DefaultRatingDialogContent { get { return _defaultRatingDialogContent; } }

        public uint MinimumAcceptedStars { get { return _minimumAcceptedStars; } }

        public string SupportEmail { get { return _supportEmail; } }

        public string IosAppId { get { return _iosAppId; } }

        public uint AnnualCap { get { return _annualCap; } }

        public uint DelayAfterInstallation { get { return _delayAfterInstallation; } }

        public uint CoolingOffPeriod { get { return _coolingOffPeriod; } }

        public bool IgnoreConstraintsInDevelopment { get { return _ignoreContraintsInDevelopment; } }

        // Appearance
        [SerializeField]
        private RatingDialogContent _defaultRatingDialogContent = RatingDialogContent.Default;

        // Behaviour
        [SerializeField][Range(0, 5)]
        private uint _minimumAcceptedStars = 4;
        [SerializeField]
        private string _supportEmail;
        [SerializeField]
        private string _iosAppId;

        // Display constraints
        [SerializeField][Range(3, 100)]
        private uint _annualCap = 12;
        [SerializeField][Range(0, 365)]
        private uint _delayAfterInstallation = 10;
        [SerializeField][Range(0, 365)]
        private uint _coolingOffPeriod = 10;
        [SerializeField]
        private bool _ignoreContraintsInDevelopment = false;
    }

    [System.Serializable]
    public class RatingDialogContent
    {
        // Placeholders for replacable strings.
        public const string PRODUCT_NAME_PLACEHOLDER = "$PRODUCT_NAME";

        public readonly static RatingDialogContent Default = new RatingDialogContent();

        public string Title { get { return _title; } }

        public string Message { get { return _message; } }

        public string LowRatingMessage { get { return _lowRatingMessage; } }

        public string HighRatingMessage { get { return _highRatingMessage; } }

        public string PostponeButtonText { get { return _postponeButtonText; } }

        public string RefuseButtonText { get { return _refuseButtonText; } }

        public string RateButtonText { get { return _rateButtonText; } }

        public string CancelButtonText { get { return _cancelButtonText; } }

        public string FeedbackButtonText { get { return _feedbackButtonText; } }

        [SerializeField]
        private string _title = "Rate " + PRODUCT_NAME_PLACEHOLDER;
        [SerializeField]
        private string _message = "How would you rate " + PRODUCT_NAME_PLACEHOLDER + "?";
        [SerializeField]
        private string _lowRatingMessage = "That's bad. Would you like to give us some feedback instead?";
        [SerializeField]
        private string _highRatingMessage = "Awesome! Let's do it!";
        [SerializeField]
        private string _postponeButtonText = "Not Now";
        [SerializeField]
        private string _refuseButtonText = "Don't Ask Again";
        [SerializeField]
        private string _rateButtonText = "Rate Now!";
        [SerializeField]
        private string _cancelButtonText = "Cancel";
        [SerializeField]
        private string _feedbackButtonText = "Send Feedback";

        private RatingDialogContent()
        {
        }

        public RatingDialogContent(
            string title,
            string message,
            string lowRatingMessage,
            string highRatingMessage,
            string postponeButtonText,
            string refuseButtonText,
            string rateButtonText,
            string cancelButtonText,
            string feedbackButtonText)
        {
            this._title = title == null ? "" : title;
            this._message = message == null ? "" : message;
            this._lowRatingMessage = lowRatingMessage == null ? "" : lowRatingMessage;
            this._highRatingMessage = highRatingMessage == null ? "" : highRatingMessage;
            this._postponeButtonText = postponeButtonText == null ? "" : postponeButtonText;
            this._refuseButtonText = refuseButtonText == null ? "" : refuseButtonText;
            this._rateButtonText = rateButtonText == null ? "" : rateButtonText;
            this._cancelButtonText = cancelButtonText == null ? "" : cancelButtonText;
            this._feedbackButtonText = feedbackButtonText == null ? "" : feedbackButtonText;
        }
    }
}

