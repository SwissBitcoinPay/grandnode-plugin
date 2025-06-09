using Grand.Business.Core.Interfaces.Checkout.Orders;
using Grand.Business.Core.Interfaces.Checkout.Payments;
using Grand.Business.Core.Interfaces.Common.Configuration;
using Grand.Domain.Orders;
using Grand.Domain.Payments;
using Grand.Web.Common.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Payments.SwissBitcoinPay;
using Payments.SwissBitcoinPay.Services;

namespace SwissBitcoinPay.Controllers
{
    public class PaymentSwissBitcoinPayController : BasePaymentController
    {
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly ILogger<SwissBitcoinPayPaymentProvider> _logger;
        private readonly IPaymentTransactionService _paymentTransactionService;

        public PaymentSwissBitcoinPayController(IOrderService orderService,
            ISettingService settingService,
            IPaymentTransactionService? paymentTransactionService,
            ILogger<SwissBitcoinPayPaymentProvider> logger)
         {
                    _settingService = settingService;
                    _orderService = orderService;
                    _logger = logger;
                    _paymentTransactionService = paymentTransactionService;
         }


        [HttpPost]
        public async Task<IActionResult> Process([FromHeader(Name = "sbp-sig")] string SwissBtcPaySig)
        {
            try
            {
                string jsonStr = await new StreamReader(Request.Body).ReadToEndAsync();
                dynamic jsonData = JsonConvert.DeserializeObject(jsonStr);
                var SwissBtcPaySecret = SwissBtcPaySig.Split('=')[1];

                string Description = jsonData.description;
                var tblDescription = Description.Split(" | ");
                string OrderGuid = tblDescription[1].Split(" : ")[1];
                string StoreID = jsonData.extra.storeId;

                if (String.IsNullOrEmpty(OrderGuid) || String.IsNullOrEmpty(StoreID))
                {
                    _logger.LogError("Missing fields in request");
                    return StatusCode(StatusCodes.Status422UnprocessableEntity);
                }

                bool IsPaid = jsonData.isPaid;
                bool IsExpired = jsonData.isExpired;


                var swissBitcoinPaySettings = await _settingService.LoadSetting<SwissBitcoinPaySettings>(StoreID);

                if (!SwissBitcoinPayService.CheckSecretKey(swissBitcoinPaySettings.ApiSecret, jsonStr, SwissBtcPaySecret))
                    throw (null);
                var order = await _orderService.GetOrderByGuid(new Guid(OrderGuid));
                if (order == null)
                {
                    _logger.LogError("Order not found");
                    return StatusCode(StatusCodes.Status422UnprocessableEntity);
                }

                if (IsPaid)
                {
                    order.PaymentStatusId = PaymentStatus.Paid;
                    try
                    {
                        var paymentTransaction = await _paymentTransactionService.GetOrderByGuid(order.OrderGuid);
                        paymentTransaction.PaidAmount = order.PaidAmount;
                        //paymentTransaction.AuthorizationTransactionId = invoiceData.Id;
                        paymentTransaction.TransactionStatus = Grand.Domain.Payments.TransactionStatus.Paid;
                        await _paymentTransactionService.UpdatePaymentTransaction(paymentTransaction);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Process() - Update PaymentTransaction: " + ex.Message);
                    }
                }
                if (IsExpired)
                {
                    if (order.PaymentStatusId != PaymentStatus.Paid)
                        order.PaymentStatusId = PaymentStatus.Voided;
                }

                await _orderService.InsertOrderNote(new OrderNote {
                    OrderId = order.Id,
                    Note = "PaymentStatus: " + order.PaymentStatusId.ToString(),
                    DisplayToCustomer = true,
                    CreatedOnUtc = DateTime.UtcNow
                });

                await _orderService.UpdateOrder(order);
                return StatusCode(StatusCodes.Status200OK); //new EmptyResult() ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }

        public IActionResult PaymentInfo()
        {
            return View();
        }

    }
}
