using Grand.Infrastructure.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Payments.SwissBitcoinPay
{
    public class EndpointProvider : IEndpointProvider
    {
        public void RegisterEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("Plugin.PaymentSwissBitcoinPay",
                 "Plugins/PaymentSwissBitcoinPay/PaymentInfo",
                 new { controller = "PaymentSwissBitcoinPay", action = "PaymentInfo", area = "" }
            );
        }
        public int Priority => 0;

    }
}
