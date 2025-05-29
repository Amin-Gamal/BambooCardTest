using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.CheckoutGiftMessage.Controllers;
using Nop.Plugin.Widgets.CheckoutGiftMessage.Filters;

namespace Nop.Plugin.Widgets.CheckoutGiftMessage.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add<CartActionFilter>();
            });

            services.Configure<RazorViewEngineOptions>(o =>
            {
                o.ViewLocationExpanders.Add(new GiftMessageSearchViewEngine());
            });

            services.AddScoped<Nop.Web.Controllers.CheckoutController, BambooCheckoutController>();
        }

        public int Order => 3000;
    }
}