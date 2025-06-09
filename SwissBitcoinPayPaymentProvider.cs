using Grand.Business.Core.Enums.Checkout;
using Grand.Business.Core.Interfaces.Catalog.Products;
using Grand.Business.Core.Interfaces.Checkout.Orders;
using Grand.Business.Core.Interfaces.Checkout.Payments;
using Grand.Business.Core.Interfaces.Common.Directory;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Business.Core.Utilities.Checkout;
using Grand.Domain.Orders;
using Grand.Domain.Payments;
using Grand.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Payments.SwissBitcoinPay.Models;
using Payments.SwissBitcoinPay.Services;

namespace Payments.SwissBitcoinPay
{
    public class SwissBitcoinPayPaymentProvider : IPaymentProvider
    {
        private readonly ITranslationService _translationService;
        private readonly SwissBitcoinPaySettings _swissBitcoinPaySettings;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductService _productService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ILogger<SwissBitcoinPayPaymentProvider> _logger;

        public SwissBitcoinPayPaymentProvider(
            IHttpContextAccessor httpContextAccessor,
            ITranslationService translationService,
            IProductService productService,
            IServiceProvider serviceProvider,
            IWorkContextAccessor workContextAccessor,
            SwissBitcoinPaySettings swissBitcoinPaySettings,
            ILogger<SwissBitcoinPayPaymentProvider> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _translationService = translationService;
            _productService = productService;
            _serviceProvider = serviceProvider;
            _workContextAccessor = workContextAccessor;
            _swissBitcoinPaySettings = swissBitcoinPaySettings;
            _logger = logger;
        }


        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        public string LogoURL => "/Plugins/Payments.SwissBitcoinPay/logo.jpg";

        public string ConfigurationUrl => "/Admin/SwissBitcoinPay/Configure";

        public string SystemName => "Payments.SwissBitcoinPay";

        public string FriendlyName => _translationService.GetResource("Payments.SwissBitcoinPay.FriendlyName");

        public int Priority => _swissBitcoinPaySettings.DisplayOrder;

        public IList<string> LimitedToStores => new List<string>();

        public IList<string> LimitedToGroups => new List<string>();


        Task IPaymentProvider.CancelPayment(PaymentTransaction paymentTransaction)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <returns>Result</returns>
        public async Task<bool> CanRePostRedirectPayment(PaymentTransaction paymentTransaction)
        {
            if (paymentTransaction == null)
                throw new ArgumentNullException(nameof(paymentTransaction));

            return await Task.FromResult(true);
        }

        public async Task<bool> SupportCapture()
        {
            return await Task.FromResult(false);
        }

        public async Task<CapturePaymentResult> Capture(PaymentTransaction paymentTransaction)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return await Task.FromResult(result);
        }

        public async Task<string> Description()
        {
            return await Task.FromResult(_translationService.GetResource("Plugins.Payments.SwissBitcoinPay.PaymentMethodDescription"));
        }

        public async Task<double> GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            if (_swissBitcoinPaySettings.AdditionalFee <= 0)
                return _swissBitcoinPaySettings.AdditionalFee;

            double result;
            if (_swissBitcoinPaySettings.AdditionalFeePercentage)
            {
                //percentage
                var orderTotalCalculationService = _serviceProvider.GetRequiredService<IOrderCalculationService>();
                var subtotal = await orderTotalCalculationService.GetShoppingCartSubTotal(cart, true);
                result = (float)subtotal.subTotalWithDiscount * (float)_swissBitcoinPaySettings.AdditionalFee / 100f;
            }
            else
            {
                result = _swissBitcoinPaySettings.AdditionalFee;
            }

            if (!(result > 0)) return result;
            var currencyService = _serviceProvider.GetRequiredService<ICurrencyService>();
            var workContext = _serviceProvider.GetRequiredService<IWorkContext>();
            result = await currencyService.ConvertFromPrimaryStoreCurrency(result, workContext.WorkingCurrency);

            return result;
        }

        public Task<string> GetControllerRouteName()
        {
            return Task.FromResult("Plugin.PaymentSwissBitcoinPay");
        }

        public async Task<bool> HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return await Task.FromResult(false);
        }

        public async Task<PaymentTransaction> InitPaymentTransaction()
        {
            return await Task.FromResult<PaymentTransaction>(null);
        }

        public async Task<ProcessPaymentResult> ProcessPayment(PaymentTransaction paymentTransaction)
        {
            var result = new ProcessPaymentResult();
            return await Task.FromResult(result);
        }

        public Task PostProcessPayment(PaymentTransaction paymentTransaction)
        {
            //nothing
            return Task.CompletedTask;
        }

        public async Task<string> PostRedirectPayment(PaymentTransaction paymentTransaction)
        {
            try
            {
                var myStore = _workContextAccessor.WorkContext.CurrentStore;
                var myCustomer = _workContextAccessor.WorkContext.CurrentCustomer;
                var lang = _workContextAccessor.WorkContext.WorkingLanguage;
                var langCode = (lang == null) ? "en" : lang.UniqueSeoCode;

                var apiService = new SwissBitcoinPayService();
                var sUrl = apiService.CreateInvoice(_swissBitcoinPaySettings, new PaymentDataModel() {
                    CurrencyCode = paymentTransaction.CurrencyCode,
                    Amount = (decimal)paymentTransaction.TransactionAmount,
                    BuyerEmail = myCustomer.Email ?? myCustomer.Addresses.First().Email,
                    BuyerName = myCustomer.BillingAddress.FirstName + " " + myCustomer.BillingAddress.LastName,
                    OrderID = paymentTransaction.OrderGuid.ToString(),
                    StoreID = myStore.Id,
                    CustomerID = myCustomer.Id,
                    Description = "From " + myStore.Name,
                    RedirectionURL = myStore.Url + "checkout/completed",
                    Lang = langCode,
                    WebHookURL = myStore.Url + "PaymentSwissBitcoinPay/Process"
                });

               // _httpContextAccessor.HttpContext?.Response.Redirect(sUrl);
               return sUrl;
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return string.Empty;
                //result.AddError(ex.Message);
            }


        }

        public async Task<RefundPaymentResult> Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return await Task.FromResult(result);
        }

        public async Task<PaymentTransaction> SavePaymentInfo(IDictionary<string, string> model)
        {
            return await Task.FromResult<PaymentTransaction>(null);
        }

        public async Task<bool> SkipPaymentInfo()
        {
            return await Task.FromResult(false);
        }


        public async Task<bool> SupportPartiallyRefund()
        {
            return await Task.FromResult(false);
        }

        public async Task<bool> SupportRefund()
        {
            return await Task.FromResult(false);
        }

        public async Task<bool> SupportVoid()
        {
            return await Task.FromResult(false);
        }

        public async Task<VoidPaymentResult> Void(PaymentTransaction paymentTransaction)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return await Task.FromResult(result);
        }

        public async Task<IList<string>> ValidatePaymentForm(IDictionary<string, string> model)
        {
            var warnings = new List<string>();

            /*if (form["Agree"] == "false")
            {
                var checkAgree = Task.Run(() => _localizationService.GetResourceAsync("Plugins.Payments.SwissBitcoinPay.CheckAgree")).Result;
                warnings.Add(checkAgree);
            }*/
            return await Task.FromResult(new List<string>());
        }

    }
}
