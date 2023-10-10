using Grand.Business.Core.Extensions;
using Grand.Business.Core.Interfaces.Common.Configuration;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Infrastructure.Plugins;
using Payments.SwissBitcoinPay;
using System.Threading.Tasks;

namespace Payments.SwissBitcoinPay
{
    /// <summary>
    /// Swiss bitcoin Pay payment processor
    /// </summary
    public class SwissBitcoinPayPaymentPlugin : BasePlugin, IPlugin
    {
        private readonly ITranslationService _translationService;
        private readonly ILanguageService _languageService;
        private readonly ISettingService _settingService;

        public SwissBitcoinPayPaymentPlugin(
            ITranslationService translationService,
            ILanguageService languageService,
            ISettingService settingService)
        {
            _translationService = translationService;
            _languageService = languageService;
            _settingService = settingService;
        }

        public override string ConfigurationUrl()
        {
            return "/Admin/SwissBitcoinPay/Configure";
        }

        public async override Task Install()
        {
            await _settingService.SaveSetting(new SwissBitcoinPaySettings {
                ApiUrl = "https://api.swiss-bitcoin-pay.ch"
            });

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Payments.SwissBitcoinPay.FriendlyName", "Swiss Bitcoin Pay");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.AdditionalFee", "Additional fee");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.AdditionalFee.Hint", "The additional fee.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.AdditionalFeePercentage", "Additional fee. Use percentage");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.Instructions", "Enter your data below to configure this plugin :");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.PaymentMethodDescription", "Pay in Bitcoin with Swiss Bitcoin Pay");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.PaymentMethodDescription2", "After completing the order you will be redirected to Swiss Bitcoin Pay, where you can make the Bitcoin payment for your order.");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.ApiKey", "API Key");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.ApiKey.Hint", "The API Key value generated in your Swiss Bitcoin Pay account");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.ApiSecret", "API Secret");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.ApiSecret.Hint", "The Api Secret value generated in your Swiss Bitcoin Pay account");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.AcceptOnChain", "Accept OnChain payments");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.AcceptOnChain.Hint", "Accept Bitcoin OnChain payments?");
            await base.Install();
        }

        public async override Task Uninstall()
        {
            //settings
            await _settingService.DeleteSetting<SwissBitcoinPaySettings>();

            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.AdditionalFee");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.AdditionalFee.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.AdditionalFeePercentage");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.AdditionalFeePercentage.Hint");


            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.Instructions");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.PaymentMethodDescription");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.PaymentMethodDescription2");

            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.ApiKey");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.ApiKey.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.ApiSecret");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.ApiSecret.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.AcceptOnChain");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.SwissBitcoinPay.AcceptOnChain.Hint");
            await base.Uninstall();
        }
    }
}
