using Grand.Business.Core.Interfaces.Common.Configuration;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Business.Core.Interfaces.Common.Security;
using Grand.Business.Core.Interfaces.Common.Stores;
using Grand.Domain.Common;
using Grand.Domain.Customers;
using Grand.Domain.Permissions;
using Grand.Infrastructure;
using Grand.Web.Common.Controllers;
using Grand.Web.Common.Filters;
using Grand.Web.Common.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.SwissBitcoinPay.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.SwissBitcoinPay.Controllers
{
    [AuthorizeAdmin]
    [Area("Admin")]
    [PermissionAuthorize(PermissionSystemName.PaymentMethods)]
    public class SwissBitcoinPayController : BasePaymentController
    {
        #region Fields

        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ITranslationService _translationService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public SwissBitcoinPayController(IStoreService storeService,
            ISettingService settingService,
            ITranslationService translationService,
            IPermissionService permissionService)
        {
            _storeService = storeService;
            _settingService = settingService;
            _translationService = translationService;
            _permissionService = permissionService;
        }

        #endregion

        #region Methods

        private async Task<string> GetActiveStore()
        {
            var stores = await _storeService.GetAllStores();
            if (stores.Count > 0)
                return stores.FirstOrDefault()?.Id;
            else
                return string.Empty;

        }

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.Authorize(StandardPermission.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await GetActiveStore();
            var swissBitcoinPaySettings = await _settingService.LoadSetting<SwissBitcoinPaySettings>(storeScope);

            var model = new ConfigurationModel
            {
                ApiUrl = swissBitcoinPaySettings.ApiUrl ?? "",
                ApiKey = swissBitcoinPaySettings.ApiKey ?? "",
                ApiSecret = swissBitcoinPaySettings.ApiSecret ?? "",
                AdditionalFee = swissBitcoinPaySettings.AdditionalFee,
                AcceptOnChain = swissBitcoinPaySettings.AcceptOnChain,
                AdditionalFeePercentage = swissBitcoinPaySettings.AdditionalFeePercentage,
                StoreScope = storeScope
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.Authorize(StandardPermission.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //load settings for a chosen store scope
            var storeScope = await GetActiveStore();
            var swissBitcoinPaySettings = await _settingService.LoadSetting<SwissBitcoinPaySettings>(storeScope);

            //save settings
            swissBitcoinPaySettings.ApiUrl = model.ApiUrl.Trim();
            swissBitcoinPaySettings.ApiKey = model.ApiKey.Trim();
            swissBitcoinPaySettings.ApiSecret = model.ApiSecret.Trim();
            swissBitcoinPaySettings.AcceptOnChain = model.AcceptOnChain;
            swissBitcoinPaySettings.AdditionalFee = model.AdditionalFee;
            swissBitcoinPaySettings.AdditionalFeePercentage = model.AdditionalFeePercentage;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */

            await _settingService.SaveSetting(swissBitcoinPaySettings, storeScope);

            //now clear settings cache
            await _settingService.ClearCache();

            Success(_translationService.GetResource("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion


    }
}