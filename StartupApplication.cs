using Grand.Business.Core.Interfaces.Checkout.Payments;
using Grand.Infrastructure;
using Grand.Web.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Payments.SwissBitcoinPay
{
    public class StartupApplication : IStartupApplication
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IPaymentProvider, SwissBitcoinPayPaymentProvider>();
        }

        public int Priority => 10;
        public void Configure(IApplicationBuilder application, IWebHostEnvironment webHostEnvironment)
        {

        }

        public void Configure(WebApplication application, IWebHostEnvironment webHostEnvironment)
        {

        }

        public bool BeforeConfigure => false;
    }

}
