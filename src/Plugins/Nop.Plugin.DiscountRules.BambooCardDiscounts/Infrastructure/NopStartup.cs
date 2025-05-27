using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.DiscountRules.BambooCardDiscounts.Services;

namespace Nop.Plugin.DiscountRules.BambooCardDiscounts.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IBambooCardDiscountsService, BambooCardDiscountsService>();
        }

        public int Order => 3000;
    }
}