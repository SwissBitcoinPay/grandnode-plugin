using Grand.Infrastructure.ModelBinding;
using Grand.Infrastructure.Models;


namespace Payments.SwissBitcoinPay.Models
{
    public class ConfigurationModel : BaseModel
    {

        public string StoreScope { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.SwissBitcoinPay.ApiUrl")]
        //[Url]
        public string ApiUrl { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.SwissBitcoinPay.ApiKey")]
        public string ApiKey { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.SwissBitcoinPay.ApiSecret")]
        public string ApiSecret { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.SwissBitcoinPay.AcceptOnChain")]
        public bool AcceptOnChain { get; set; }

        public bool OverrideForStore { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.SwissBitcoinPay.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }


        [GrandResourceDisplayName("Plugins.Payments.SwissBitcoinPay.AdditionalFee")]
        public double AdditionalFee { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.SwissBitcoinPay.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }


    }

}