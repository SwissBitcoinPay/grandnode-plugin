using Grand.Domain.Configuration;

namespace Payments.SwissBitcoinPay
{
    /// <summary>
    /// Represents settings of SBP payment plugin
    /// </summary>
    public class SwissBitcoinPaySettings : ISettings
    {
        /// <summary>
        /// The url of your the Swiss Bitcoin Pay API
        /// </summary>
        public string ApiUrl { get; set; }

        /// <summary>
        /// Your Swiss Bitcoin Pay API Key given in your Swiss Bitcoin Pay dashbord
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The Api Secret value set in your Swiss Bitcoin Pay dashbord
        /// </summary>
        public string ApiSecret { get; set; }

        public bool AcceptOnChain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }
        /// <summary>
        /// Additional fee
        /// </summary>
        public double AdditionalFee { get; set; }

        public int DisplayOrder { get; set; }

    }
}