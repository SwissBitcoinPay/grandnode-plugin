using Grand.Business.Common.Services.Localization;
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
    public class SwissBitcoinPayPaymentPlugin(
            ITranslationService translationService,
            ILanguageService languageService,
            ISettingService settingService,
            IPluginTranslateResource pluginTranslateResource) : BasePlugin, IPlugin
    {

        public override string ConfigurationUrl()
        {
            return "/Admin/SwissBitcoinPay/Configure";
        }

        public async override Task Install()
        {
            await settingService.SaveSetting(new SwissBitcoinPaySettings {
                ApiUrl = "https://api.swiss-bitcoin-pay.ch"
            });

            await pluginTranslateResource.AddOrUpdatePluginTranslateResource("Payments.SwissBitcoinPay.FriendlyName", "Swiss Bitcoin Pay");
            await pluginTranslateResource.AddOrUpdatePluginTranslateResource("Plugins.Payments.SwissBitcoinPay.AdditionalFee", "Additional fee");
            await pluginTranslateResource.AddOrUpdatePluginTranslateResource("Plugins.Payments.SwissBitcoinPay.AdditionalFee.Hint", "The additional fee.");
            await pluginTranslateResource.AddOrUpdatePluginTranslateResource("Plugins.Payments.SwissBitcoinPay.AdditionalFeePercentage", "Additional fee. Use percentage");
            await pluginTranslateResource.AddOrUpdatePluginTranslateResource("Plugins.Payments.SwissBitcoinPay.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            await pluginTranslateResource.AddOrUpdatePluginTranslateResource("Plugins.Payments.SwissBitcoinPay.Instructions", "Enter your data below to configure this plugin :");
            await pluginTranslateResource.AddOrUpdatePluginTranslateResource("Plugins.Payments.SwissBitcoinPay.PaymentMethodDescription", "Pay in Bitcoin with Swiss Bitcoin Pay");
            await pluginTranslateResource.AddOrUpdatePluginTranslateResource("Plugins.Payments.SwissBitcoinPay.PaymentMethodDescription2", "After completing the order you will be redirected to Swiss Bitcoin Pay, where you can make the Bitcoin payment for your order.");
            await pluginTranslateResource.AddOrUpdatePluginTranslateResource("Plugins.Payments.SwissBitcoinPay.ApiKey", "API Key");
            await pluginTranslateResource.AddOrUpdatePluginTranslateResource("Plugins.Payments.SwissBitcoinPay.ApiKey.Hint", "The API Key value generated in your Swiss Bitcoin Pay account");
            await pluginTranslateResource.AddOrUpdatePluginTranslateResource("Plugins.Payments.SwissBitcoinPay.ApiSecret", "API Secret");
            await pluginTranslateResource.AddOrUpdatePluginTranslateResource("Plugins.Payments.SwissBitcoinPay.ApiSecret.Hint", "The Api Secret value generated in your Swiss Bitcoin Pay account");
            await pluginTranslateResource.AddOrUpdatePluginTranslateResource("Plugins.Payments.SwissBitcoinPay.AcceptOnChain", "Accept OnChain payments");
            await pluginTranslateResource.AddOrUpdatePluginTranslateResource("Plugins.Payments.SwissBitcoinPay.AcceptOnChain.Hint", "Accept Bitcoin OnChain payments?");
            await base.Install();
        }

        public async override Task Uninstall()
        {
            //settings
            await settingService.DeleteSetting<SwissBitcoinPaySettings>();

            await pluginTranslateResource.DeletePluginTranslationResource("Plugins.Payments.SwissBitcoinPay.AdditionalFee");
            await pluginTranslateResource.DeletePluginTranslationResource("Plugins.Payments.SwissBitcoinPay.AdditionalFee.Hint");
            await pluginTranslateResource.DeletePluginTranslationResource("Plugins.Payments.SwissBitcoinPay.AdditionalFeePercentage");
            await pluginTranslateResource.DeletePluginTranslationResource("Plugins.Payments.SwissBitcoinPay.AdditionalFeePercentage.Hint");
            await pluginTranslateResource.DeletePluginTranslationResource("Plugins.Payments.SwissBitcoinPay.Instructions");
            await pluginTranslateResource.DeletePluginTranslationResource("Plugins.Payments.SwissBitcoinPay.PaymentMethodDescription");
            await pluginTranslateResource.DeletePluginTranslationResource("Plugins.Payments.SwissBitcoinPay.PaymentMethodDescription2");
            await pluginTranslateResource.DeletePluginTranslationResource("Plugins.Payments.SwissBitcoinPay.ApiKey");
            await pluginTranslateResource.DeletePluginTranslationResource("Plugins.Payments.SwissBitcoinPay.ApiKey.Hint");
            await pluginTranslateResource.DeletePluginTranslationResource("Plugins.Payments.SwissBitcoinPay.ApiSecret");
            await pluginTranslateResource.DeletePluginTranslationResource("Plugins.Payments.SwissBitcoinPay.ApiSecret.Hint");
            await pluginTranslateResource.DeletePluginTranslationResource("Plugins.Payments.SwissBitcoinPay.AcceptOnChain");
            await pluginTranslateResource.DeletePluginTranslationResource("Plugins.Payments.SwissBitcoinPay.AcceptOnChain.Hint");
            await base.Uninstall();
        }
    }
}
