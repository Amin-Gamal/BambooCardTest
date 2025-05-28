using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.ProductAttributeSearch.Areas.Admin.Controllers;

namespace Nop.Plugin.Misc.ProductAttributeSearch.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Nop.Web.Areas.Admin.Controllers.ProductAttributeController, BambooProductAttributeController>();
            services.AddScoped<IBambooProductAttributeService, BambooProductAttributeService>();
        }

        public int Order => 3000;
    }
}