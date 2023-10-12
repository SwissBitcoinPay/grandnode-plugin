using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Payments.SwissBitcoinPay;
using Payments.SwissBitcoinPay.Models;
using Org.BouncyCastle.Utilities;
using System.Net.Http;

namespace Payments.SwissBitcoinPay.Services
{
    public class SwissBitcoinPayService
    {

        public static bool CheckSecretKey(string key, string message, string signature)
        {
            var msgBytes = HMACSHA256.HashData(Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(message));
            string hashString = string.Empty;
            foreach (byte x in msgBytes)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return (hashString == signature);
        }

        public string CreateInvoice(SwissBitcoinPaySettings settings, PaymentDataModel paymentData)
        {

            try
            {
                var invoice = new SwissBitcoinPayInvoiceModel()
				{
                    title = paymentData.Description,
                    description = $"{paymentData.BuyerName} | Order : {paymentData.OrderID}",
                    unit = paymentData.CurrencyCode,
					amount = paymentData.Amount,
                    email = paymentData.BuyerEmail,
                    emailLanguage = paymentData.Lang,
                    redirectAfterPaid = paymentData.RedirectionURL,
                    webhook = paymentData.WebHookURL,
                    delay = 60,
                    onChain = settings.AcceptOnChain,
                    extra = new SwissBitcoinPayExtraModel() {
                        customNote = $"Order {paymentData.OrderID}",
                        storeId = paymentData.StoreID
                    }
                };
				var invoiceJson = JsonConvert.SerializeObject(invoice, Formatting.None);

				string sUrl = settings.ApiUrl.EndsWith("/") ? settings.ApiUrl : settings.ApiUrl + "/";
                var client = new HttpClient() {
                    BaseAddress = new Uri(sUrl)
                };
                var webRequest = new HttpRequestMessage(HttpMethod.Post, "checkout") {
                    Content = new StringContent(invoiceJson, Encoding.UTF8, "application/json"),
                };
                webRequest.Headers.Add("api-key", settings.ApiKey);

                string sRep;
                using (var rep = client.SendAsync(webRequest).Result)
                {
                    rep.EnsureSuccessStatusCode();
                    using (var rdr = new StreamReader(rep.Content.ReadAsStream()))
                    {
                        sRep = rdr.ReadToEnd();
                    }
                }

                dynamic JsonRep = JsonConvert.DeserializeObject<dynamic>(sRep);
				return JsonRep.checkoutUrl;

            }
            catch
            {
                throw;
            }

		}
    }
}